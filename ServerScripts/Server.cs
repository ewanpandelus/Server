using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
public class Server 
{
   
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> connectedClients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int fromClient, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;
    public static TcpListener tcpListener;
    private static UdpClient udpListener;


    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        Port = _port;

        Debug.Log("Starting server...");
        InitialiseServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);
    }
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                /*if (_clientId == 0)
                {
                    return;
                }*/

                if (connectedClients[_clientId].udp.endPoint == null)
                {
                    connectedClients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (connectedClients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    connectedClients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error receiving UDP data: {_ex}");
        }
    }
    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }
    public static void Stop()
    {
         tcpListener.Stop();
    }
    private static void TCPConnectCallback(IAsyncResult _asyncResult)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(_asyncResult);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Debug.Log($"Incoming connection from: {client.Client.RemoteEndPoint}");
        for (int i = 0; i < MaxPlayers; i++)
        {
            if (connectedClients[i].tcp.socket == null)
            {
                connectedClients[i].tcp.Connect(client);
                return;
            }
        }
    }

    /// <summary>Sends a packet to the specified endpoint via UDP.</summary>
    /// <param name="_clientEndPoint">The endpoint to send the packet to.</param>
    /// <param name="_packet">The packet to send.</param>
  
    private static void InitialiseServerData()
    {
        for (int i = 0; i < MaxPlayers; i++)
        {
            connectedClients.Add(i, new Client(i));
        }
        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandler.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandler.PlayerMovement },
                { (int)ClientPackets.playerPosition, ServerHandler.PlayerPosition },
                { (int)ClientPackets.playerRotation, ServerHandler.PlayerRotation }
            };
        Debug.Log("Packets initialised");
    }
    
}
