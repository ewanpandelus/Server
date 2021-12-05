using GameServer;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
  
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message from server: {msg}.");
        Client.instance.id = id;
        ClientSend.WelcomeReceived();
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }
   
    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }
    public static void PlayerPosition(Packet _packet)
    {
        try
        {
            int _id = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();
            GameManager.players[_id].transform.position = position;
        }
        catch
        {
            Debug.Log("Player position sending before player is in game");
        }
    
    }
    public static void PlayerRotation(Packet _packet)
    {
        try
        {
            int _id = _packet.ReadInt();
            float rotation = _packet.ReadFloat();
            if (rotation != 0)
            {
                GameManager.playerObjects[_id].transform.GetChild(1).transform.rotation = new Quaternion(rotation, 0, 0, 0);
            }
        
        }
        catch
        {
            Debug.Log("Player rotatio sending before player is in game");
        }
    }
   

}
