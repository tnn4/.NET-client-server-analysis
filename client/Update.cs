using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal partial class Update : IUpdateable
    {

        protected GameTime _game_time;

        public Update(GameTime game_time)
        {
            _game_time = game_time;
        }

        void IUpdateable.Update(GameTime game_time)
        {
            throw new NotImplementedException();
        }
    }

    internal partial class Update
    {
        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}
