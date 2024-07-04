using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] private FloatingJoystick joystick;
    private PhotonView photonView;
    private Rigidbody2D rb;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (joystick != null)
            {
                Vector3 direction = Vector3.up * joystick.Vertical + Vector3.right * joystick.Horizontal;
                rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
            }
        }
    }
}
