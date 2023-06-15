using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowardAgarioStepOne
{
    internal class WorldDrawable : IDrawable
    {
        public WorldModel Model { get; set; }

        public WorldDrawable(WorldModel model)
        {
            Model = model;
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.DrawCircle(Model.X, Model.Y, Model.Radius);
        }
    }
}
