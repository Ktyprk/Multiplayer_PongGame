using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviourPun, IPunObservable
{
    public Transform startPos;
    private String lastScoringPlayerNickname;
    private Rigidbody2D rb;
    private PhotonView pw, otherPV;

    private Vector2 networkPosition;
    private Vector2 velocity;
    private float lag;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pw = GetComponent<PhotonView>();
        networkPosition = rb.position;
    }

    [PunRPC]
    public void StartBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPos.position;
        StartCoroutine(StartBallCoroutine());
    }
    
    private IEnumerator StartBallCoroutine()
    {
        GameManager.instance.ballCountdownText.gameObject.SetActive(true);
        
        for (int i = 3; i > 0; i--)
        {
            GameManager.instance.ballCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        GameManager.instance.ballCountdownText.text = "Başladı!";
        yield return new WaitForSeconds(1f);
        GameManager.instance.ballCountdownText.gameObject.SetActive(false);

        GameManager.instance.StartGameCountdown();
    }

    private void FixedUpdate()
    {
        if (pw.IsMine)
        {
            ProcessLocalInput();
        }
        else
        {
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, velocity.magnitude * Time.fixedDeltaTime);
        }
    }

    private void ProcessLocalInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        rb.AddForce(movement);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Diğer oyunculara veri gönder
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Diğer oyunculardan veri al
            networkPosition = (Vector2)stream.ReceiveNext();
            velocity = (Vector2)stream.ReceiveNext();

            // Gecikmeyi hesapla
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += velocity * lag;
        }
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
