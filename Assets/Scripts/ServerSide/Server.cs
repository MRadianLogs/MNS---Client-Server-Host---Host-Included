using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxNumPlayers { get; private set; } //The max num of players able to join the server.
    public static int Port { get; private set; } //The port to start the server on(have connections for the server use that port).

    private static TcpListener tcpListener; //An object that listens for TCPClient connections trying to connect to the server. These clients use TCP (Transmission Control Protocol).
    private static UdpClient udpListener; //An object that listens for UDP data packets being sent to the server. This client uses UDP (User Datagram Protocol).

    public static Dictionary<int, ServerClient> clients = new Dictionary<int, ServerClient>(); //The list of clients connected to the server, stored with an int identifier.

    public delegate void PacketHandler(int clientOrigin, Packet packet); //A data type that represents/references a method that handles a packet, and keeps track of what client sent the packet.
    public static Dictionary<int, PacketHandler> packetHandlers; //A list of packet handlers, each of which handles a specific packet type, stored with an int identifier.

    /// <summary>
    /// Start the network server with a max number of players that can connect to the server and a port num to use with the server.
    /// </summary>
    /// <param name="maxNumPlayers"></param>
    /// <param name="portNum"></param>
    public static void Start(int maxNumPlayers, int portNum)
    {
        MaxNumPlayers = maxNumPlayers;
        Port = portNum;

        Debug.Log("Starting Server...");
        InitializeServerData(); //Calls method to prepare client list slots and server packet handlers.

        tcpListener = new TcpListener(IPAddress.Any, Port);

        tcpListener.Start();//Starts listening for incoming connections.
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); //Waits for new connection in different thread. When new connection made, calls TCPConnectCallback method.

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port: {Port}.");

    }

    /// <summary>
    /// Stop the server by stopping any TCP connections and UDP data packets. The server will no longer accept any connections or data.
    /// </summary>
    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }

    /// <summary>
    /// The method called whenever a new connection has been started. It creates a new TCP client to represent the connecting client. It then begins waiting for new connections. 
    /// If the server has room for more clients, the new client is connected.
    /// </summary>
    /// <param name="result"></param>
    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient newClient = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); //Waits for more connections.

        Debug.Log($"Incoming connection from {newClient.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxNumPlayers; i++) //Go through list of clients to find open spot.
        {
            if (clients[i].tcp.socket == null) //If that spot is open, 
            {
                clients[i].tcp.Connect(newClient); //assign the new client to that client instance/spot.
                return;
            }
        }

        Debug.Log($"{newClient.Client.RemoteEndPoint} failed to connect: Server full!"); //If this line is reached, it means the server is full and the new client was not connected.
    }

    /// <summary>
    /// This method is called when the server receives data sent using UDP. It identifies who sent the data, determines if the client who sent the data needs to be setup, then 
    /// determines whether or not to handle the data.
    /// </summary>
    /// <param name="result"></param>
    private static void UDPReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (data.Length < 4) //If data size is less than 4, it means not enough data was received?
            {
                return;
            }

            using (Packet packet = new Packet(data)) //Create, fill, and handle a packet from the recieved data.
            {
                int clientId = packet.ReadInt(); //Get client id from first part of packet.

                if (clientId == 0) //We dont have a client at 0, so if the clientID is 0, don't bother handling that data.
                {
                    return;
                }

                if (clients[clientId].udp.endPoint == null) //If received data from a client that doesnt have UDP setup yet,
                {
                    clients[clientId].udp.Connect(clientEndPoint); //Setup/connect client UDP class. Allows it to send/receive UDP data.
                    return;
                }

                if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString()) //If the id attached to the packet, which represents who sent the packet, has the endpoint(IP address) that matches the endpoint which sent the data, handle the data.
                {
                    clients[clientId].udp.HandleData(packet);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error receiving UDP data: {e}");
        }
    }

    /// <summary>
    /// A method used to send data, in the form of a packet, over to a client, specified by their endpoint.
    /// </summary>
    /// <param name="clientEndPoint"></param>
    /// <param name="packet"></param>
    public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
    {
        try
        {
            if (clientEndPoint != null)
            {
                udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error sending data to {clientEndPoint} via UDP: {e}");
        }
    }

    /// <summary>
    /// Setup the server's client list with the proper amount of slots, set by MaxNumPlayers, and setup the server's packet handlers.
    /// </summary>
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxNumPlayers; i++)
        {
            clients.Add(i, new ServerClient(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived},
                { (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived},
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement}
            };
        Debug.Log("Initialized packet handlers.");
    }
}
