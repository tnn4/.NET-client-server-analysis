using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Client
{
    internal partial class Draw : IDrawable
    {
        protected GameTime _game_time;
        protected SpriteBatch _sb;
        protected GraphicsDeviceManager _gdm;
        protected GraphicsDevice _gd;

        public Draw(){}
        public Draw(GameTime game_time, SpriteBatch sb, GraphicsDeviceManager gdm)
        {
            _game_time = game_time;
            _sb = sb;
            _gdm = gdm;
            _gd = _gdm.GraphicsDevice;
    }
        void IDrawable.Draw(GameTime game_time)
        {
            // throw new NotImplementedException();
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
