using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public Transform startPos;
    private int player1Score = 0;
    private int player2Score = 0;
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    
    private Rigidbody2D rb;
    private PhotonView pw;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pw = GetComponent<PhotonView>();
    }
    
    [PunRPC]
    public void StartBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPos.position;
        
        ShowScore();
    }
    
    public void ShowScore()
    {
        player1ScoreText.text = PhotonNetwork.PlayerList[0].NickName + " : " + player1Score.ToString();
        player2ScoreText.text = PhotonNetwork.PlayerList[1].NickName + " : " + player2Score.ToString();;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(pw.IsMine)
        {
            if (other.CompareTag("Goal1"))
            {
                player2Score++;
                ShowScore();
                pw.RPC("ResetBall", RpcTarget.All);
            }
            else if (other.CompareTag("Goal2"))
            {
                player1Score++;
                ShowScore();
                pw.RPC("ResetBall", RpcTarget.All);
            }
        }
    }
    
    [PunRPC]
    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPos.position;
    }
}
