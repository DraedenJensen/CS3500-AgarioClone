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
/// File Contents: This file contains the Drawable class, which is used to draw the game for Assignment 8.
/// </summary>

using AgarioModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
//using System.Drawing;

namespace ClientGUI
{
    internal class Drawable : IDrawable
    {
        private float leftX;
        private float rightX;

        private float topY;
        private float bottomY;
        
        public int PlayerRank { get; set; }   
        public int PlayerMass { get; set; }
        private int oldMass;

        public int Kills { get; set; }

        public World World { get; set; }

        /// <summary>
        /// Initializes a new instance of the Drawable class with the specified world.
        /// </summary>
        /// <param name="world"></param>
        public Drawable(World world)
        {
            World = world;
            PlayerMass = 150;
        }

        /// <summary>
        /// Draws the game objects (foods and players) on the canvas, within a specified dirty rectangle.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="dirtyRect"></param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawBackgroundGray(canvas, dirtyRect);

            Player player = World.GetOurPlayer(out float x, out float y);
            int zoomWidth = (int)((player.Mass / 100) + 498.5);

            lock (World.Foods)
            {
                foreach (Food food in World.Foods)
                {
                    DrawItemZoom(canvas, zoomWidth, x, y, food);
                }
            }

            lock (World.Players)
            {
                List<Player> sortedPlayers = World.Players.OrderBy(player => player.Radius).ToList();

                foreach (Player p in sortedPlayers)
                {
                    DrawItemZoom(canvas, zoomWidth, x, y, p);
                }
            }

            DrawStats(canvas, player);
        }

        /// <summary>
        /// Fills the background gray.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="dirtyRect"></param>
        private void DrawBackgroundGray(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.LightGray;
            canvas.FillRectangle(dirtyRect);
        }

        /// <summary>
        /// Displays player's mass, kill count, and current position in top left corner.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="player"></param>
        private void DrawStats(ICanvas canvas, Player player)
        {
            if (player.Mass != 0)
            {
                PlayerRank = World.GetOurPlayerRank();//Added for assignment 9 to keep track of our players rank throughout the game

                oldMass = PlayerMass;
                PlayerMass = (int)player.Mass;

                if (PlayerMass - oldMass > 140)
                {
                    Kills++;
                }
            }
            canvas.DrawString($"Mass: {PlayerMass}   Kills: {Kills}   Position: {(int)player.X}, {(int)player.Y}", 20, 30, HorizontalAlignment.Left);

        }

        /// <summary>
        /// Draws a specific game object (food or player) on the canvas, adjusting its position and size based on zoom level.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="item"></param>
        private void DrawItemZoom(ICanvas canvas, int zoomWidth, float x, float y, GameObject item)
        {
            leftX = x - zoomWidth;
            rightX = x + zoomWidth;

            bottomY = y - zoomWidth;
            topY = y + zoomWidth;

            if ((item.X + item.Radius) > leftX && (item.X - item.Radius) < rightX)
            {
                if ((item.Y + item.Radius) > bottomY && (item.Y - item.Radius) < topY)
                {
                    float ratio = (item.X - leftX) / (rightX - leftX);
                    float xToDraw = 1200 * ratio;

                    ratio = (item.Y - bottomY) / (topY - bottomY);
                    float yToDraw = 600 * ratio;

                    canvas.FillColor = Color.FromInt(item.ARGBColor);

                    canvas.FillCircle(xToDraw, yToDraw, item.Radius);

                    if (item is Player)
                    {
                        Player p = item as Player;
                        canvas.DrawString(p.Name, xToDraw, yToDraw - (p.Radius) - 10, HorizontalAlignment.Center);
                    }
                }
            }
        }
    }
}
