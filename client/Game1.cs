using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;

namespace client
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Texture2D[] textures;
		Dictionary<long, Vector2> positions = new Dictionary<long, Vector2>();
		NetClient client;
		int port_wrong = 142142; // screwed up here, no wonder it gave ArgumentOutOfRangeException
		int port_good = 14242;

        #region Game1
        public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

			client = new NetClient(config);
			client.Start();
		}
		#endregion

		#region Init
		protected override void Initialize()
		{
			Console.WriteLine("Client initialized.");
			client.DiscoverLocalPeers(port_good);
			// Console.WriteLine($"Found peer at {port}");
			base.Initialize();
		}
        #endregion

        #region LoadContent
        protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			textures = new Texture2D[5];
			for (int i = 0; i < 5; i++)
				textures[i] = Content.Load<Texture2D>("c" + (i + 1));
		}
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
		{
			//
			// Collect input
			//
			int xinput = 0;
			int yinput = 0;

			/*
			SByte x_in_byte = 0; //SByte/signed byte range = [-128, 127]
			SByte y_in_byte = 0;
			*/

			KeyboardState keyState = Keyboard.GetState();

			// exit game if escape or Back is pressed
			if (keyState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

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
				client.SendMessage(om, NetDeliveryMethod.Unreliable);
			}

			// read messages
			NetIncomingMessage msg_in;
			// NetOutgoingMessage msg_out;
			while ((msg_in = client.ReadMessage()) != null)
			{
				switch (msg_in.MessageType)
				{
					case NetIncomingMessageType.DiscoveryResponse:
						// just connect to first server discovered
						client.Connect(msg_in.SenderEndPoint);
						break;
					case NetIncomingMessageType.Data:
						// server sent a position update
						long who = msg_in.ReadInt64();
						int x = msg_in.ReadInt32();
						int y = msg_in.ReadInt32();
						positions[who] = new Vector2(x, y);
						break;
				}
			}

			base.Update(gameTime);
		}
		#endregion
		
		#region Draw
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

			// draw all players
			foreach (var kvp in positions)
			{
				// use player unique identifier to choose an image
				int num = Math.Abs((int)kvp.Key) % textures.Length;

				// draw player
				spriteBatch.Draw(textures[num], kvp.Value, Color.White);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	#endregion
        protected override void OnExiting(object sender, EventArgs args)
		{
			client.Shutdown("bye");

			base.OnExiting(sender, args);
		}
	}
}

