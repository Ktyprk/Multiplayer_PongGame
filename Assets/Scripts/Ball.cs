using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviourPun, IPunObservable
{
    public Transform startPos;
    private string lastScoringPlayerNickname;
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private PhotonView pw;

    private Vector2 networkPosition;
    private Vector2 velocity;
    private float lag;
    
    private bool gameStarted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pw = GetComponent<PhotonView>();
        col = GetComponent<CircleCollider2D>();
        networkPosition = rb.position;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject.");
        }
    }

    [PunRPC]
    public void StartBall()
    {
        StartCoroutine(StartBallCoroutine());
    }

    private IEnumerator StartBallCoroutine()
    {
        
        rb.velocity = Vector2.zero;
        col.enabled = false;
        transform.position = startPos.position;
        GameManager.instance.ballCountdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            GameManager.instance.ballCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        rb.velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 5f;
        col.enabled = true;
        yield return new WaitForSeconds(1f);
        GameManager.instance.ballCountdownText.gameObject.SetActive(false);

        if (!gameStarted)
        {
            GameManager.instance.StartGameCountdown();
            gameStarted = true;
        }
    }

    private void FixedUpdate()
    {
        if (!pw.IsMine)
        {
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, velocity.magnitude * Time.fixedDeltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players
            stream.SendNext(rb.position);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Receive data from other players
            networkPosition = (Vector2)stream.ReceiveNext();
            velocity = (Vector2)stream.ReceiveNext();

            // Calculate lag
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += velocity * lag;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (pw.IsMine)
        {
            PhotonView otherPV = other.gameObject.GetComponent<PhotonView>();
            if (otherPV != null && otherPV.IsMine)
            {
                Vector2 collisionForce = other.relativeVelocity * other.rigidbody.mass;
                rb.AddForce(collisionForce, ForceMode2D.Impulse);
                lastScoringPlayerNickname = pw.Controller.NickName;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pw.IsMine)
        {
            if (other.CompareTag("Goal1"))
            {
                Player scoringPlayer = GetPlayerByIndex(1);
                if (scoringPlayer != null)
                {
                    lastScoringPlayerNickname = scoringPlayer.NickName;
                    GameManager.instance.ShowScore(scoringPlayer, 1);
                }
                pw.RPC("ResetBall", RpcTarget.All);
            }
            else if (other.CompareTag("Goal2"))
            {
                Player scoringPlayer = GetPlayerByIndex(0);
                if (scoringPlayer != null)
                {
                    lastScoringPlayerNickname = scoringPlayer.NickName;
                    GameManager.instance.ShowScore(scoringPlayer, 1);
                }
                pw.RPC("ResetBall", RpcTarget.All);
            }
        }
    }

    private Player GetPlayerByIndex(int index)
    {
        if (PhotonNetwork.PlayerList.Length > index)
        {
            return PhotonNetwork.PlayerList[index];
        }
        return null;
    }

    [PunRPC]
    public void ResetBall()
    {
        rb.velocity = Vector2.zero;
        transform.position = startPos.position;
        StartCoroutine(StartBallCoroutine());
    }
}
