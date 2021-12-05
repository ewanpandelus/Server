using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendDataTCP(int client, Packet packet)
    {
        packet.WriteLength();
        Server.connectedClients[client].tcp.SendData(packet);
    }
    private static void SendDataUDP(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.connectedClients[_toClient].udp.SendData(_packet);
    }
    private static void SendDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 0; i < Server.MaxPlayers; i++)
        {
            try
            {
                Server.connectedClients[i].udp.SendData(packet);
            }
            catch
            {
                Debug.Log(i);
            }
          
        }
    }
    private static void SendDataToAllButOne(int client, Packet packet)
    {
        packet.WriteLength();
        for (int i = 0; i < Server.MaxPlayers; i++)
        {
            if (i != client && Server.connectedClients[i] != null)
            {
                Server.connectedClients[i].udp.SendData(packet);
            }

        }
    }
    public static void Welcome(int client, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(client);

            SendDataTCP(client, packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player2 _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(_player.id);
            packet.Write(_player.username);
            packet.Write(_player.transform.position);
            packet.Write(_player.transform.rotation);

            SendDataUDP(_toClient, packet);
        }
    }
    public static void PlayerPosition(Player2 _player)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerPosition))
        {
            packet.Write(_player.id);
            packet.Write(_player.transform.position);
            SendDataToAllButOne(_player.id, packet);
        }
    }
    public static void PlayerRotation(Player2 _player, float _rotationX)
    {
        using (Packet packet = new Packet((int)ServerPackets.PlayerRotation))
        {
            packet.Write(_player.id);
            packet.Write(_rotationX);
            SendDataToAllButOne(_player.id, packet);
        }
    }


}

