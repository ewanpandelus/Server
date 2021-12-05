using GameServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    public static void WelcomeReceived()
    {
        using(Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.id);
            packet.Write(UIManager.instance.inputField.text);
            SendTCPData(packet);
        }
    }
    public static void PlayerPosition(float elaspedTime)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerPosition))
        { 
            packet.Write(GameManager.players[Client.instance.id].transform.position);
            packet.Write(elaspedTime);
            SendTCPData(packet);
        }
    }
    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.id].transform.rotation);

            SendTCPData(_packet);
        }
    }
    public static void PlayerRotation(float _rotationX)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerRotation))
        {
            packet.Write(Client.instance.id);
            packet.Write(_rotationX);
            SendTCPData(packet);
        }
    }



}


