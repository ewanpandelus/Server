using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int,PlayerManager>();
    public static Dictionary<int, GameObject> playerObjects = new Dictionary<int, GameObject>();
    public GameObject serverGameObjectPrefab;
    public GameObject localGameObjectPrefab;
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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject player;
        if(_id == Client.instance.id)
        {
            player = Instantiate(localGameObjectPrefab, _position, _rotation);
        }
        else
        {
            player = Instantiate(serverGameObjectPrefab, _position, _rotation);
           // player.transform.Rotate(Vector3.up, -90);
        }
        player.GetComponent<PlayerManager>().id = _id;
        player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, player.GetComponent<PlayerManager>());
        playerObjects.Add(_id, player);
    }
}
