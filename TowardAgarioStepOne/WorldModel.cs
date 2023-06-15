using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TowardAgarioStepOne
{
    internal class WorldModel
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }

        private Vector2 direction;

        public WorldModel() 
        {
            X = 100;
            Y = 100;
            Radius = 20;

            //Change direction to affect speed
            direction = new(2, 1);
        }

        public void AdvanceGameOneStep()
        {
            X += direction.X;
            Y += direction.Y;

            if (X > 800 || X < 0)
            {
                direction.X *= -1;
            }
            if (Y > 800 || Y < 0)
            {
                direction.Y *= -1;
            }
        }

        public Vector2 GetDirection()
        {
            return direction;
        }
    }
}
