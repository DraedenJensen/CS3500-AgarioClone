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
/// File Contents: This file contains the GameObject class, which is the base class for the Player and Food classes. Represents a single object in the Assignment 8 game.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AgarioModels
{
    /// <summary>
    /// Represents a single object in the Assignment 8 game. Food and Players inherit from this class.
    /// </summary>
    public class GameObject
    {
        public long ID { get; set; }

        private Vector2 Location; 
        public float X { get; set; } 
        public float Y { get; set; } 
        public int ARGBColor { get; set; } 
        public float Mass { get; set; }

        /// <summary>
        /// Returns the radius of the object based on its mass
        /// </summary>
        public float Radius
        {
            get { return (float)Math.Sqrt(Mass / Math.PI); }
        }

    }


}
