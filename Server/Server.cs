
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Lidgren.Network;
using Util;

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
    private int _tick_rate = 30; // n frames per second
    private double _tick_time;
    double nextSendUpdates;
    Position_sbyte _pos_sbyte;
    SByteBufferxy _bxy;

    public Server()
    {
        _tick_time = (1.0 / _tick_rate);
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

            // The client sent input to the server
            // Do logic from client data input 
            case NetIncomingMessageType.Data:

                on_data_received();

                // ints = 4 bytes
                int xinput = msg.ReadInt32();
                int yinput = msg.ReadInt32();
                //sbyte x_in_b = msg.ReadSByte();
                //sbyte y_in_b = msg.ReadSByte();

                int[] pos = msg.SenderConnection.Tag as int[];
                //sbyte[] pos = msg.SenderConnection.Tag as sbyte[];
                // fancy movement logic goes here; we just append input to position
                pos[0] += xinput;
                pos[1] += yinput;
 
                //pos[0] += x_in_b;
                //pos[1] += y_in_b;
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
        // send position updates 30 times per second 
        // NOTE: allow updates per second(ticks) to be configured
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
                    NetOutgoingMessage outBox = server.CreateMessage();

                    // write who this position is for
                    outBox.Write(otherPlayer.RemoteUniqueIdentifier);

                    if (otherPlayer.Tag == null)
                        otherPlayer.Tag = new int[2]; //int[2]

                    int[] pos = otherPlayer.Tag as int[];

                    outBox.Write(pos[0]);
                    outBox.Write(pos[1]);

                    //sbyte[] pos_b = otherPlayer.Tag as sbyte[];
                    //om.Write(pos_b[0]);
                    //om.Write(pos_b[1]);
                    // send message
                    server.SendMessage(outBox, player, NetDeliveryMethod.Unreliable);
                }
            }

            // schedule next update
            // nextSendUpdates += (1.0 / 30.0); // e.g. next update in 0.034 seconds or 34 ms
            nextSendUpdates += _tick_time;
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

    public void on_data_received()
    {

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