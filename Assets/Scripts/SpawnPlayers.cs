using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    
    private bool hasSpawned = false; 

    public string playerName;
    private void Start()
    {
        if (!hasSpawned)
        {
            SpawnPlayer();
        }
    }
    
    Transform spawnPoint;
    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPoint = spawnPoints[0];
        }
        else
        {
            spawnPoint = spawnPoints[1];
        }

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);

        playerName = PlayerPrefs.GetString("PlayerNickname");
    }
}