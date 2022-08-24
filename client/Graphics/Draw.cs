using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Client
{
    internal partial class Draw : IDrawable
    {
       private GameTime _game_time;

        public Draw(GameTime game_time)
        {
            _game_time = game_time;
        }
        void IDrawable.Draw(GameTime game_time)
        {
            throw new NotImplementedException();
        }
    }

    internal partial class Draw : IDrawable
    {
        public int DrawOrder => throw new NotImplementedException();

        public bool Visible => throw new NotImplementedException();

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}
