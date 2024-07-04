using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public TextMeshProUGUI playerNameTextPrefab;
    public TextMeshProUGUI playerScoreTextPrefab;
    public Transform playerNameDisplayParent;
    public Transform playerScoreDisplayParent;

    private Dictionary<int, TextMeshProUGUI> playerNameTexts = new Dictionary<int, TextMeshProUGUI>();
    private Dictionary<int, TextMeshProUGUI> playerScoreTexts = new Dictionary<int, TextMeshProUGUI>();
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            DisplayPlayerName(player);
            UpdateScoreForNewPlayer(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayPlayerName(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerName(otherPlayer);
    }

    void DisplayPlayerName(Player player)
    {
        TextMeshProUGUI playerNameTextInstance = Instantiate(playerNameTextPrefab, playerNameDisplayParent);
        playerNameTextInstance.text = player.NickName;
        playerNameTexts.Add(player.ActorNumber, playerNameTextInstance);

        TextMeshProUGUI playerScoreTextInstance = Instantiate(playerScoreTextPrefab, playerScoreDisplayParent);
        playerScoreTextInstance.text = "Score: 0";
        playerScoreTexts.Add(player.ActorNumber, playerScoreTextInstance);
    }

    void RemovePlayerName(Player player)
    {
        if (playerNameTexts.ContainsKey(player.ActorNumber))
        {
            Destroy(playerNameTexts[player.ActorNumber].gameObject);
            playerNameTexts.Remove(player.ActorNumber);
        }

        if (playerScoreTexts.ContainsKey(player.ActorNumber))
        {
            Destroy(playerScoreTexts[player.ActorNumber].gameObject);
            playerScoreTexts.Remove(player.ActorNumber);
        }
    }

    
    public void ShowScore(Player player, int scoreToAdd)
    {
        string playerName = player.NickName;
        if (playerScores.ContainsKey(playerName))
        {
            playerScores[playerName] += scoreToAdd;
        }
        else
        {
            playerScores[playerName] = scoreToAdd;
        }

        photonView.RPC("UpdateScoreUI", RpcTarget.All, player.ActorNumber, playerScores[playerName]);
    }

    [PunRPC]
    void UpdateScoreUI(int actorNumber, int newScore)
    {
        if (playerScoreTexts.ContainsKey(actorNumber))
        {
            playerScoreTexts[actorNumber].text = $"Score: {newScore}";
        }
    }
    
    private void UpdateScoreForNewPlayer(Player newPlayer)
    {
        string playerName = newPlayer.NickName;
        if (playerScores.ContainsKey(playerName))
        {
            photonView.RPC("UpdateScoreUI", newPlayer, newPlayer.ActorNumber, playerScores[playerName]);
        }
    }


    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
