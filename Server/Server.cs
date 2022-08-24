
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Lidgren.Network;

namespace Server;

/// <summary>
/// 
/// </summary>
internal class Server
{
    NetPeerConfiguration _config;
    NetServer server;
    private int _port = 14242;
    private int _sleep_time = 10;
    private int _tick_rate = 30;
    private double _tick_time;
    double nextSendUpdates;
    Position_sbyte _pos_sbyte;

    public Server()
    {
        Console.WriteLine("Constructor called");
        // NetPeerConfiguration _config = new NetPeerConfiguration("xnaapp"); // this was dropped after instantiation
        _config = new NetPeerConfiguration("xnaapp");
        if (_config == null)
        {
            Console.WriteLine("ERROR: config null");
            return;
        }
        server = new NetServer(_config);
        if (server == null)
        {
            Console.WriteLine("ERROR: config null");
            return;
        }

    }

    public void init_server()
    {
        // Configuration
        _config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
        _config.Port = 14242;
        _tick_time = (1 / _tick_rate);
        // You can simulate latency with netpeer configuration
        // config.SimulatedRandomLatency = 1;// simulated latency in seconds
    }

    public void run_server()
    {
        server.Start();
        // schedule initial sending of position updates
        double nextSendUpdates = NetTime.Now;
        server_loop();
    }

    private void server_loop()
    {
        // Server Loop
        // run until escape is pressed
        while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
        {
            NetIncomingMessage msg;

            // get client messages and read them
            while( (msg=server.ReadMessage()) != null)
            {
                respond_to_msg_type(msg);
            }

            run_tick();

            // sleep to allow other processes to run smoothly
            Thread.Sleep(_sleep_time);
        }
    }

    private void respond_to_msg_type(NetIncomingMessage msg) {
        // Figure out what to do depending on the message type
        switch (msg.MessageType)
        {
            // Server received a discovery request from a client;
            // send a discovery response (with no extra data attached)
            case NetIncomingMessageType.DiscoveryRequest:
                server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                break;
            // Debug Verbose
            case NetIncomingMessageType.VerboseDebugMessage:
            // Debug
            case NetIncomingMessageType.DebugMessage:
            // Warning
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
                    // A new player just connected!
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

                // The client sent input to the server
                _pos_sbyte.x = 0;
                _pos_sbyte.y = 0;
                // ints = 4 bytes
                int xinput = msg.ReadInt32();
                int yinput = msg.ReadInt32();
                msg.ReadSByte();

                int[] pos = msg.SenderConnection.Tag as int[];

                // fancy movement logic goes here; we just append input to position
                pos[0] += xinput;
                pos[1] += yinput;
                // pos[2] += yinput; this is legal
                break;
        }


    }

    private void run_tick()
    {
#if DEBUG
        // Debug.WriteLine("tick()");
        Console.WriteLine("tick()");
#endif
        //
        // send position updates 30 times per second // NOTE: allow updates per second(ticks) to be configured
        //
        double now = NetTime.Now;
        if (now > nextSendUpdates)
        {
            // Yes, it's time to send position updates for each player...
            // om = outmessage
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
            nextSendUpdates += (1.0 / 30.0); // next update at 0.034 seconds or 34 ms
        }
    }

    public void set_port(int port)
    {
        _port = port;
    }

    public void shutdown()
    {
        server.Shutdown("Server shutting down");
    }

}

public struct Position_sbyte
{
    public sbyte x
    {
        set;
        get;
    }
    public sbyte y
    {
        set;
        get;
    }
}