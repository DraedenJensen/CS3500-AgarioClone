/// <summary>
/// Authors : Draeden Jensen & Derek Kober
/// Date : 4/13/2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Draeden Jensen & Derek Kober - This work may not 
///            be copied for use in Academic Coursework.
///
/// Draeden Jensen & Derek Kober, certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents: This file sets up the GUI, which is used to show the game for Assignment 8.
/// </summary>

using AgarioModels;
using Communications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using System;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace ClientGUI
{
    /// <summary>
    /// The MainPage class represents the main page of the game, handling user input and game events.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private World world;
        private Drawable draw;
        private Networking client;
        private ILogger<MainPage> logger;
        private Timer timer;
        Thread timerThread;
        Thread thread;
        private int ourPositionX;
        private int ourPositionY;
         
        private static string connectionString; //assignment 9
        private string starttime; //assignment 9
        private static string ourPlayerName; //assignment 9


        /// <summary>
        /// Initializes a new instance of the MainPage class with the specified logger.
        /// </summary>
        /// <param name="logger">The logger instance to be used for logging.</param>
        public MainPage(ILogger<MainPage> logger)
        {
            this.logger = logger;
            logger.LogInformation("Program started successfully");

            InitializeComponent();
        }

        /// <summary>
        /// Handles the Connect button click event, initializing and connecting the game client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectClicked(object sender, EventArgs e)
        {
            try
            {
                if (PlayerName.Text is null)
                {
                    ErrorBox.Text = "Player name cannot be empty";
                }
                else
                {
                    if (PlayerName.Text.Length == 0)
                    {
                        ErrorBox.Text = "Player name cannot be empty";
                    }
                    else
                    {
                        if (client is null)
                        {
                            client = new(NullLogger.Instance, onConnect, onDisconnect, onMessage, '\n');
                            client.Connect(ServerAddress.Text, 11000);

                            logger.LogInformation("Initial connection established");
                        }

                        WelcomeScreen.IsVisible = false;
                        WorldScreen.IsVisible = true;

                        ConnectToDatabase();// assignment 9
                        ourPlayerName = PlayerName.Text;// assignment 9

                        client.Send(String.Format(Protocols.CMD_Start_Game, ourPlayerName));
                        logger.LogInformation($"Game session started - player name {PlayerName.Text}");

                        if (world is null)
                        {
                            world = new(logger);
                            draw = new(world);
                        }
                        draw.Kills = 0;
                        PlaySurface.Drawable = draw;

                        timerThread = new(CreateTimer);
                        timerThread.Start();

                        thread = new(WaitForMessages);
                        thread.Start();
                    }
                }
            }
            catch (Exception)
            {
                ErrorBox.Text = "Couldn't connect to server";
                logger.LogError("Server connection failed");
            }
        }

        /// <summary>
        /// Connects our snake game to the database from assignment 9.
        /// </summary>
        private void ConnectToDatabase()
        {

            //Log start time for DB
            starttime = DateTime.Now.ToString();

            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<MainPage>();
            IConfigurationRoot Configuration = builder.Build();
            var SelectedSecrets = Configuration.GetSection("WebServerSecrets");

            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = SelectedSecrets["ServerName"],
                InitialCatalog = SelectedSecrets["ServerDBName"],
                UserID = SelectedSecrets["ServerDBUsername"],
                Password = SelectedSecrets["ServerDBPassword"],
                ConnectTimeout = 15,
                Encrypt = false
            }.ConnectionString;
        }

        /// <summary>
        /// Sends the player's high score, best rank, start and end time of the game to database from assignmnet 9.
        /// Called when user's player dies. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="highmass"></param>
        /// <param name="highrank"></param>
        private void InsertGameInfo(object name, object starttime, string endtime, object highmass, object highrank)
        {
            
            try
            {
                using SqlConnection con = new(connectionString);
                con.Open();

                using SqlCommand command = new SqlCommand($@"INSERT INTO dbo.Games (playerName, StartTime, EndTime, MaxMass, MaxRank)
                                                          VALUES ('{name}', '{starttime}', '{endtime}', {highmass}, {highrank});", con);

                using SqlDataReader reader = command.ExecuteReader();

                reader.Read();
            }
            catch (SqlException exception)
            {
                Console.WriteLine($"Error in SQL connection: {exception.Message}");
            }
        }


        /// <summary>
        /// Indefinitely waits for incoming messages from the server.
        /// </summary>
        private void WaitForMessages()
        {
            client.AwaitMessagesAsync();
        }

        /// <summary>
        /// Handles the pointer (mouse) position change event and updates the player's position accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointerChanged (object sender, PointerEventArgs e)
        {
            Point mousePosition = (Point)e.GetPosition((View)sender);
            
            Player player = draw.World.GetOurPlayer(out float x, out float y);
            int zoomWidth = (int)((player.Mass / 10) + 485);

            ourPositionX = ConvertX((float)mousePosition.X, zoomWidth, x);
            ourPositionY = ConvertY((float)mousePosition.Y, zoomWidth, y);
        }

        /// <summary>
        /// Tapping the mount on the screen can be used to split the user's player. 
        /// The player splits in the directtion where the mouse is when it's tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointerTapped(object sender, TappedEventArgs e)
        {
            Point mousePosition = (Point)e.GetPosition((View)sender);

            Player player = draw.World.GetOurPlayer(out float x, out float y);
            int zoomWidth = (int)((player.Mass / 10) + 485);

            float tappedPositionX = ConvertX((float)mousePosition.X, zoomWidth, x);
            float tappedPositionY = ConvertY((float)mousePosition.Y, zoomWidth, y);

            client.Send(String.Format(Protocols.CMD_Split, tappedPositionX, tappedPositionY));
            logger.LogDebug("Split command sent");
        }

        /// <summary>
        /// Converts the given x-coordinate of the pointer (mouse) position to the game world's x-coordinate.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="zoomWidth"></param>
        /// <param name="playerX"></param>
        /// <returns>The converted x-coordinate in the game world.</returns>
        private int ConvertX(float mouseX, int zoomWidth, float playerX)
        {
            float leftX = playerX - zoomWidth;
            float rightX = playerX + zoomWidth;

            float temp = mouseX / 1200;
            temp *= (rightX - leftX);

            temp += leftX;

            return (int)temp;
        }

        /// <summary>
        /// Converts the given y-coordinate of the pointer (mouse) position to the game world's y-coordinate.
        /// </summary>
        /// <param name="mouseY"></param>
        /// <param name="zoomWidth"></param>
        /// <param name="playerY"></param>
        /// <returns>The converted y-coordinate in the game world.</returns>
        private int ConvertY(float mouseY, int zoomWidth, float playerY)
        {
            float bottomY = playerY - zoomWidth;
            float topY = playerY + zoomWidth;

            float temp = mouseY / 600;
            temp *= (topY - bottomY);

            temp += bottomY;

            return (int)temp;
        }

        /// <summary>
        /// Creates a timer to update GUI display at 30 FPS.
        /// </summary>
        private void CreateTimer()
        {
            timer = new(new TimerCallback(GameStep), null, 0, 33);
            logger.LogDebug("Timer started");
        }

        /// <summary>
        /// Executes a game step by sending the updated player position to the server and invalidating the game surface.
        /// </summary>
        /// <param name="state"></param>
        private void GameStep(object state)
        {
            if (draw.World.GameOver)
            {
             
                Dispatcher.Dispatch(() =>
                {
                    WorldScreen.IsVisible = false;
                    WelcomeScreen.IsVisible = true;
                    GameInfo.Text = $"You died!\nYou killed {draw.Kills} players.\nYour final Mass: {draw.PlayerMass}";
                        ErrorBox.Text = "Game Over";
                        draw.World.GameOver = false;

                    // high score, best rank, as well as the start and end time of the game
                    string endtime = DateTime.Now.ToString(); // assignment 9
                    InsertGameInfo(ourPlayerName, starttime, endtime, draw.PlayerMass, draw.PlayerRank); // assignment 9
                });
            }
            else
            {
                
                client.Send(String.Format(Protocols.CMD_Move, ourPositionX, ourPositionY));

                PlaySurface.Invalidate();
            }
        }

        /// <summary>
        /// Handles incoming messages from the server and processes game commands.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        private void onMessage(Networking channel, string message)
        {
            if (message.Contains(Protocols.CMD_Player_Object))
            {
                draw.World.CommandPlayerObject(long.Parse(message.Substring(message.IndexOf('}') + 1)));
            }
            if (message.Contains(Protocols.CMD_Food))
            {
                draw.World.CommandFood(message.Substring(message.IndexOf('}') + 1));
            }
            if (message.Contains(Protocols.CMD_Update_Players))
            {
                draw.World.CommandPlayers(message.Substring(message.IndexOf('}') + 1));
            }
            if (message.Contains(Protocols.CMD_Dead_Players))
            {
                draw.World.CommandDeadPlayers(message.Substring(message.IndexOf('}') + 1));
            }
            if (message.Contains(Protocols.CMD_Eaten_Food))
            {
                draw.World.CommandEatenFood(message.Substring(message.IndexOf('}') + 1));
            }
            if (message.Contains(Protocols.CMD_HeartBeat))
            {
                logger.LogTrace("Heartbeat command received");
            }
        }

        /// <summary>
        /// Handles the connection event (no functionality needed).
        /// </summary>
        /// <param name="channel"></param>
        private void onConnect(Networking channel) { }

        /// <summary>
        /// Handles when client disconects from server (no functionality needed).
        /// </summary>
        /// <param name="channel"></param>
        private void onDisconnect(Networking channel) { }
    }
}