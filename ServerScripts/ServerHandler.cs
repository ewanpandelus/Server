using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandler 
{
    public static void WelcomeReceived(int clientNumber, Packet packet)
    {
        int clientID = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"The connecting client is {Server.connectedClients[clientNumber].tcp.socket.Client.RemoteEndPoint}, with username {username}... \n" +
            $"They are player {clientNumber}.");
        if (clientID != clientNumber)
        {
            Debug.Log("Client has claimed the wrong ID :(");
        }
        Server.connectedClients[clientNumber].SendIntoGame(username);
    }
 
   public static void PlayerPosition(int _clientNumber, Packet _packet)
   {
        if (Server.connectedClients[_clientNumber] != null)
        {
            Vector3 _position = _packet.ReadVector3();
            float _elapsedTime = _packet.ReadFloat();
            Server.connectedClients[_clientNumber].player.SetPosition(_position, _elapsedTime);
        }
   }
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

       // Server.connectedClients[_fromClient].player.SetInput(_inputs, _rotation);
    }
    public static void PlayerRotation(int _clientNumber, Packet _packet)
    {
        if (Server.connectedClients[_clientNumber] != null)
        {
            Server.connectedClients[_clientNumber].player.SetRotation(_packet.ReadFloat());
        }
    }
}


