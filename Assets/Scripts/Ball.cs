using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviourPun
{
    public Transform startPos;
   
    private String lastScoringPlayerNickname;
    
    private Rigidbody2D rb;
    private PhotonView pw, otherPV;

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
       
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (pw.IsMine)
        {
            otherPV = other.gameObject.GetComponent<PhotonView>();
            if (otherPV != null && otherPV.IsMine)
            {
                lastScoringPlayerNickname = pw.Controller.NickName;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pw.IsMine && other.CompareTag("Goal"))
        {
            if (otherPV != null)
            {
                Player scoringPlayer = otherPV.Owner;
                if (scoringPlayer != null)
                {
                    lastScoringPlayerNickname = scoringPlayer.NickName;
                    GameManager.instance.ShowScore(scoringPlayer, 1);
                }
            }
            pw.RPC("ResetBall", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPos.position;
    }
}
