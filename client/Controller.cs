using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;

namespace Client
{
    internal class Controller
    {
        NetClient client;
        
        int xinput = 0;
        int yinput = 0;

        sbyte x_byte = 0;
        sbyte y_byte = 0;
        public void handle_player_input()
        {



            KeyboardState keyState = Keyboard.GetState();

            // use arrows or dpad to move avatar
            // >
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyState.IsKeyDown(Keys.Left))
                xinput = -1;
            // <
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyState.IsKeyDown(Keys.Right))
                xinput = 1;
            // v
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || keyState.IsKeyDown(Keys.Up))
                yinput = -1;
            // ^
            if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || keyState.IsKeyDown(Keys.Down))
                yinput = 1;

            if (xinput != 0 || yinput != 0)
            {
                //
                // If there's input; send it to server
                //
                NetOutgoingMessage om = client.CreateMessage();
                om.Write(xinput); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity // What's a more efficient way ?
                om.Write(yinput);
                om.Write(x_byte);
                client.SendMessage(om, NetDeliveryMethod.Unreliable);
            }
        }
    }
}
