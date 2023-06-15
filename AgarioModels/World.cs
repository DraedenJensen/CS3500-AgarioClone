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
/// File Contents: This file contains the World class, which is used to store the state of the game for Assignment 8
/// </summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FileLogger;
using Microsoft.Extensions.Logging;

namespace AgarioModels
{
    public class World
    {
        private readonly int Width = 5000;
        private readonly int Height = 5000;

        public HashSet<Player> Players { get; set; }
        public HashSet<Food> Foods { get; set; }
        public long OurPlayer{ get; set; }
        public bool GameOver { get; set; }
        private ILogger logger;

        bool firstFood;

        /// <summary>
        /// Initializes the World object.
        /// </summary>
        /// <param name="logger"></param>
        public World(ILogger logger)
        {
            this.logger = logger;
            Players = new();
            Foods = new();

            firstFood = true;
            GameOver = false;
        }

        /// <summary>
        /// Returns the player object that is currently being controlled by the user.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Player GetOurPlayer (out float x, out float y)
        {
            x = 0;
            y = 0;

            foreach (Player player in Players)
            {
                if (player.ID == OurPlayer)
                {
                    x = player.X;
                    y = player.Y;

                    return player;
                }
            }

            return new Player();
        }

        /// <summary>
        /// Sed the ID of the user's player object given by the server.
        /// </summary>
        /// <param name="id"></param>
        public void CommandPlayerObject(long id)
        {
            OurPlayer = id;
        }

        /// <summary>
        /// Processes the JSON string sent by the server to update the food locations in the game.
        /// </summary>
        /// <param name="foodListJson"></param>
        public void CommandFood(string foodListJson)
        {
            lock (Foods)
            {
                if (firstFood)
                {
                    Foods = JsonSerializer.Deserialize<HashSet<Food>>(foodListJson) ?? new();
                    firstFood = false;
                    logger.LogInformation($"food command received - initial food populated");
                }
                else
                {
                    HashSet<Food> foodsToAdd = JsonSerializer.Deserialize<HashSet<Food>>(foodListJson) ?? new();
                    foreach (Food food in foodsToAdd)
                    {
                        Foods.Add(food);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the JSON string sent by the server to update the player locations in the game.
        /// </summary>
        /// <param name="playerListJson"></param>
        public void CommandPlayers(string playerListJson)
        {
            lock (Players)
            {
                int oldPlayers = Players.Count;
                Players = JsonSerializer.Deserialize<HashSet<Player>>(playerListJson) ?? new();
                if (oldPlayers != Players.Count)
                {
                    logger.LogInformation($"Player command received - {Players.Count} players in current session");
                }
            }
        }

        /// <summary>
        /// Processes the JSON string sent by the server to update players who have died in the game.
        /// </summary>
        /// <param name="playerListJson"></param>
        public void CommandDeadPlayers(string playerListJson)
        { 
            HashSet<long> playersToRemove = JsonSerializer.Deserialize<HashSet<long>>(playerListJson) ?? new();
            if (playersToRemove.Count > 0 )
            {
                logger.LogDebug($"Dead player command received - {playersToRemove.Count} players to update");
            }

            foreach (Player player in Players)
            {
                if (playersToRemove.Contains(player.ID))
                {
                    Players.Remove(player);
                    if (player.ID == OurPlayer)
                    {
                        logger.LogInformation("Player died");
                        GameOver = true;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the JSON string sent by the server to update the food that has been eaten, and should be removed from the display.
        /// </summary>
        /// <param name="foodListJson"></param>
        public void CommandEatenFood (string foodListJson)
        {
            HashSet<long> foodsToRemove = JsonSerializer.Deserialize<HashSet<long>>(foodListJson) ?? new();
            logger.LogTrace($"Eaten food command received - {foodsToRemove.Count} foods to update");

            foreach (Food food in Foods)
            {
                if (foodsToRemove.Contains(food.ID))
                {
                    Foods.Remove(food);
                }
            }
        }

        /// <summary>
        /// Calculates the current rank of ourplayer in the game based on mass.
        /// Larger players are ranked higher than smaller players.
        /// </summary>
        /// <returns></returns>
        public int GetOurPlayerRank()
        {
            List<Player> sortedPlayers = Players.OrderByDescending(player => player.Radius).ToList();
            int rank = 1;
            foreach (Player player in sortedPlayers)
            {
                if (player.ID == OurPlayer)
                {
                    break;
                }
                rank++;
            }
            
            return rank;

        }
    }
}
