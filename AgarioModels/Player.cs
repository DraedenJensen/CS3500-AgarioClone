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
/// File Contents: This file contains the Player class, which inherits from the GameObject class. Represents a single player in the Assignment 8 game.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgarioModels
{
    /// <summary>
    /// Represents a single player in the Assignment 8 game.
    /// </summary>
    public class Player : GameObject
    {
        /// <summary>
        /// The name of the player that user gave to server.
        /// </summary>
        public string Name { get; set; }
    }
}
