using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Graphics
{
    
    internal class DrawSquare : Draw, IDrawable
    {
        Texture2D whiteRect;
        
        public DrawSquare(GameTime gt, SpriteBatch sb, GraphicsDeviceManager gdm) : base(gt,sb,gdm)
        {
            load();
        }

        void IDrawable.Draw(GameTime game_time)
        {
            draw(game_time);
        }
        public void draw(GameTime game_time)
        {

        }
        public void draw_test()
        {

        }

        public void drawWhiteRect()
        {
            _sb.Begin();
            _sb.Draw(whiteRect, new Rectangle(10, 10, 100, 100), Color.White);
            _sb.End();
        }

        public void drawSquare16x16(Vector2 position)
        {

        }

        /// <summary>
        /// load assets
        /// </summary>
        public void load()
        {
            make_white_rect(1,1);
        }

        public void make_white_rect(int x, int y)
        {
            whiteRect = new Texture2D(_gd, x, y);
            whiteRect.SetData(new[] { Color.White });
        }
    }
}
