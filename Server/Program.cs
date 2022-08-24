using System;
using System.Threading;

using Lidgren.Network;
using Server;

namespace XnaGameServer
{
	class Program
	{
		static void Main(string[] args)
		{
			Server.Server server = new Server.Server();
			server.init_server();
			server.run_server();
		}
		static void Main_(string[] args)
		{
			// Configuration
			NetPeerConfiguration config = new NetPeerConfiguration("xnaapp");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 14242;
			// You can simulate latency with netpeer configuration
			// config.SimulatedRandomLatency = 1;// simulated latency in seconds
			

			// Create and Start Server
			NetServer server = new NetServer(config);
			server.Start();

			// schedule initial sending of position updates
			double nextSendUpdates = NetTime.Now;

			// Server Loop
			// run until escape is pressed
			while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
			{
				NetIncomingMessage msg;

				while ((msg = server.ReadMessage()) != null) // get client messages and read them
				{
					// Figure out what to do depending on the message type
					switch (msg.MessageType)
					{
						case NetIncomingMessageType.DiscoveryRequest:
							//
							// Server received a discovery request from a client; send a discovery response (with no extra data attached)
							//
							server.SendDiscoveryResponse(null, msg.SenderEndPoint);
							break;
						// Debug Verbose
						case NetIncomingMessageType.VerboseDebugMessage:
						
						// Debug
						case NetIncomingMessageType.DebugMessage:
						
						case NetIncomingMessageType.WarningMessage:
						
						// Error
						case NetIncomingMessageType.ErrorMessage:
							//
							// Just print diagnostic messages to console
							//
							Console.WriteLine(msg.ReadString());
							break;
						
						// Client Status changed
						case NetIncomingMessageType.StatusChanged:
							NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
							if (status == NetConnectionStatus.Connected)
							{
								//
								// A new player just connected!
								//
								Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

								// randomize his position and store in connection tag
								msg.SenderConnection.Tag = new int[] {
									NetRandom.Instance.Next(10, 100),
									NetRandom.Instance.Next(10, 100)
								};
							}

							break;
						
						// Do logic from client data input 
						case NetIncomingMessageType.Data:
							//
							// The client sent input to the server
							
							//
							// Original code: read ints = 4 bytes
							int xinput = msg.ReadInt32();
							int yinput = msg.ReadInt32();

							// Read 1 signed byte
							/*
							sbyte xinput2 = msg.ReadSByte();
							sbyte yinput2 = msg.ReadSByte();
							*/

							int[] pos = msg.SenderConnection.Tag as int[];
							// SByte[] pos = msg.SenderConnection.Tag as SByte[];

							// fancy movement logic goes here; we just append input to position
							pos[0] += xinput;
							pos[1] += yinput;

							break;
					}

					//
					// send position updates 30 times per second // NOTE: allow updates per second(ticks) to be configured
					//
					double now = NetTime.Now;
					if (now > nextSendUpdates)
					{
						// Yes, it's time to send position updates

						// for each player...
						foreach (NetConnection player in server.Connections)
						{
							// ... send information about every other player (actually including self)
							foreach (NetConnection otherPlayer in server.Connections)
							{
								// send position update about 'otherPlayer' to 'player'
								NetOutgoingMessage om = server.CreateMessage();

								// write who this position is for
								om.Write(otherPlayer.RemoteUniqueIdentifier);

								if (otherPlayer.Tag == null)
									otherPlayer.Tag = new int[2];

								int[] pos = otherPlayer.Tag as int[];
								om.Write(pos[0]);
								om.Write(pos[1]);

								// send message
								server.SendMessage(om, player, NetDeliveryMethod.Unreliable);
							}
						}

						// schedule next update
						nextSendUpdates += (1.0 / 30.0);
					}
				}

				// sleep to allow other processes to run smoothly
				Thread.Sleep(1);
			}

			server.Shutdown("Server shutting down");
		}
	}
}