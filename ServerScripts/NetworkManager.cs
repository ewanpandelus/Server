using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerPrefab;
    private int count = 0;
    //Save players here and relay to all when each person connects
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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Server.Start(10, 44818);
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player2 InstantiatePlayer()
    {
        if(count == 0)
        {
            count++;
            return Instantiate(playerPrefab, new Vector3(12.33f, 2.085f, 0), playerPrefab.transform.rotation).GetComponent<Player2>();
        }
        else { return Instantiate(playerPrefab, new Vector3(-5f, 2.085f, 0), playerPrefab.transform.rotation).GetComponent<Player2>(); }

    }
  

}
