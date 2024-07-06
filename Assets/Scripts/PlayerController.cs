using System;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    public float speed = 5f;
    [SerializeField] private FloatingJoystick joystick;
    private PhotonView photonView;
    private Rigidbody2D rb;
    
    public String playerNameText;
 
    private void Start()
    {
        joystick = GameObject.Find("Floating Joystick").GetComponent<FloatingJoystick>();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
        playerNameText = photonView.Controller.NickName;
    }
    
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
             if (joystick != null)
             {
                Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical).normalized;
                rb.velocity = direction * speed;
             }
            // float moveHorizontal = Input.GetAxis("Horizontal");
            // float moveVertical = Input.GetAxis("Vertical");
            //
            // // Create a movement vector
            // Vector3 direction = new Vector3(moveHorizontal, moveVertical, 0f);
            //
            // // Move the Rigidbody
            // rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
            
        }
    }
}
