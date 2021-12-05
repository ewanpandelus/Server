using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using GameServer;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int bufferSize = 1024;

    public string IP = "127.0.0.1";
    public int port = 44818;
    public int id;
  

    public TCP tcp;
    public UDP udp;
    private bool connected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Two Clients! Eeek, destroying one of them");
            Destroy(this);
        }
    }
    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }
    public void ConnectToServer()
    {
        InitialiseClientData();
        connected = true;
        tcp.Connect();
    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }
    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.IP), instance.port);
        }
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.id);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    // TODO: disconnect
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                // TODO: disconnect
            }
        }
        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }
    }


    public class TCP
    {
        public TcpClient socket;
        private NetworkStream dataStream;
        private Packet receivedData;
        private byte[] receivedBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            receivedBuffer = new byte[bufferSize];
            socket.BeginConnect(instance.IP, instance.port, ConnectCallback, socket);
        }
        private void Disconnect()
        {
            instance.Disconnect();
            dataStream = null;
            receivedData = null;
            receivedBuffer = null;
            socket = null;

        }
        private void ConnectCallback(IAsyncResult _asyncResult)
        {
            socket.EndConnect(_asyncResult);

            if (!socket.Connected)
            {
                return;
            }
            dataStream = socket.GetStream();
            receivedData = new Packet();
            dataStream.BeginRead(receivedBuffer, 0, bufferSize, ReceiveCallback, null);
        }
        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    dataStream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = dataStream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receivedBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                dataStream.BeginRead(receivedBuffer, 0, bufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
            }
        }
        private bool HandleData(byte[] data)
        {
            int packetLength = 0;
            receivedData.SetBytes(data);
            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }
            while(packetLength>0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);
                    }
                });
                packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (packetLength <= 1)
            {
                return true;
            }
            return false;
       
        }
    }

    private static void InitialiseClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandler.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandler.SpawnPlayer },
            { (int)ServerPackets.PlayerPosition, ClientHandler.PlayerPosition },
            { (int)ServerPackets.PlayerRotation, ClientHandler.PlayerRotation },
        };
        Debug.Log("Packets initialised");
    }
    private void Disconnect()
    {
        if (connected)
        {
            connected = false;
            tcp.socket.Close();
      

            Debug.Log("Disconnected from server.");
        }
    }



}
