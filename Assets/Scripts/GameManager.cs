using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public TextMeshProUGUI playerNameTextPrefab;
    public TextMeshProUGUI playerScoreTextPrefab;
    public TextMeshProUGUI ballCountdownText;
    public TextMeshProUGUI gameCountdownText;
    public Transform playerNameDisplayParent;
    public Transform playerScoreDisplayParent;
    public Ball ball; 
    public GameObject winScreen, loseScreen;

    private Dictionary<int, TextMeshProUGUI> playerNameTexts = new Dictionary<int, TextMeshProUGUI>();
    private Dictionary<int, TextMeshProUGUI> playerScoreTexts = new Dictionary<int, TextMeshProUGUI>();
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();
    
    private bool gameStarted = false;
    
    [SerializeField] private float gameTime = 10f;

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

        CheckAndStartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayPlayerName(newPlayer);
        UpdateScoreForNewPlayer(newPlayer);

        CheckAndStartGame();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerName(otherPlayer);
        // StopGame();
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

    void CheckAndStartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartGame()
    {
        ball.StartBall();
    }

    public void StartGameCountdown()
    {
        StartCoroutine(GameCountdownCoroutine());
    }

    private IEnumerator GameCountdownCoroutine()
    {
        gameCountdownText.gameObject.SetActive(true);
        float countdown = gameTime; 

        while (countdown > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(countdown);
            gameCountdownText.text = time.ToString(@"mm\:ss");
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        gameCountdownText.text = "Game Over!";
        yield return new WaitForSeconds(1f);
        gameCountdownText.gameObject.SetActive(false);
        
        EndGame();
    }
    
    private void EndGame()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        string localPlayerName = localPlayer.NickName;
        
        int localPlayerScore = playerScores.ContainsKey(localPlayerName) ? playerScores[localPlayerName] : 0;
        int opponentScore = 0;

        foreach (var kvp in playerScores)
        {
            if (kvp.Key != localPlayerName)
            {
                opponentScore = kvp.Value;
                break;
            }
        }

        if (localPlayerScore > opponentScore)
        {
            winScreen.SetActive(true);
        }
        else
        {
            loseScreen.SetActive(true);
        }
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
