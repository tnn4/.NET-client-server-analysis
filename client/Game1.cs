using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Lidgren.Network;
using Client;
using Util;

namespace Client
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{

		Graphics_ _graphics = new Graphics_();
		Textures _textures = new Textures();
		Dictionary<long, Vector2> positions = new Dictionary<long, Vector2>();
		NetClient client;
		SByteBufferxy _bxy;

		int server_port = 14242;

        #region Game1
        public Game1()
		{
			_graphics._gdm = new GraphicsDeviceManager(this);
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
			client.DiscoverLocalPeers(server_port);
			Console.WriteLine($"Server started on port: {server_port}");
			// Console.WriteLine($"Found peer at {port}");
			base.Initialize();
		}
        #endregion

        #region LoadContent
        protected override void LoadContent()
		{
			_graphics._sb = new SpriteBatch(GraphicsDevice);
			_textures.textures_list = new List<Texture2D>();
			
			for (int i = 0; i < 5; i++)
				_textures.textures_list.Add( Content.Load<Texture2D>("sprites/characters/c" + (i + 1)) );

			_textures.textures_list.Add(Content.Load<Texture2D>( "sprites/characters/player" ));
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

			//sbyte x_byte = 0;
			//sbyte y_byte = 0;

			// Controller.handle_player_input();
			KeyboardState keyState = Keyboard.GetState();

			// exit game if escape or Back is pressed
			if (keyState.IsKeyDown(Keys.Escape) 
				|| GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// use arrows or dpad to move avatar
			// >
			if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed
				|| keyState.IsKeyDown(Keys.Left)
				|| keyState.IsKeyDown(Keys.A))
				//_bxy.x = -1;
				xinput = -1;
			// <
			if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed
				|| keyState.IsKeyDown(Keys.Right)
				|| keyState.IsKeyDown(Keys.D))
				//_bxy.x = 1;
				xinput = 1;
			// v
			if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed
				|| keyState.IsKeyDown(Keys.Up)
				|| keyState.IsKeyDown(Keys.W))
				//_bxy.y = -1;
				yinput = -1;
			// ^
			if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed
				|| keyState.IsKeyDown(Keys.Down)
				|| keyState.IsKeyDown(Keys.S))
				//_bxy.y = 1;
				yinput = 1;

            // Send to server
			// If there's input; send it to server
            if (xinput != 0 || yinput != 0)
			{
				NetOutgoingMessage outBox = client.CreateMessage();
				outBox.Write(xinput); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity // What's a more efficient way ?
				outBox.Write(yinput);
				// om.Write(_bxy.x);
				// om.Write(_bxy.y);
				client.SendMessage(outBox, NetDeliveryMethod.Unreliable);
			}

			// Read Messages
			NetIncomingMessage inBox;
			// NetOutgoingMessage msg_out;
			while ((inBox = client.ReadMessage()) != null)
			{
				switch (inBox.MessageType)
				{
					case NetIncomingMessageType.DiscoveryResponse:
						// just connect to first server discovered
						client.Connect(inBox.SenderEndPoint);
						break;
					case NetIncomingMessageType.Data:
						// server sent a position update
						long who = inBox.ReadInt64();
						int x = inBox.ReadInt32();
						int y = inBox.ReadInt32();

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

			_graphics._sb.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
			// draw all players
			foreach (var kvp in positions)
			{
				// use player unique identifier to choose an image
				int num = Math.Abs((int)kvp.Key) % _textures.textures_list.Count;

				// draw player
				_graphics._sb.Draw(_textures.textures_list[num], kvp.Value, Color.White);
			}
			_graphics._sb.End();

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

