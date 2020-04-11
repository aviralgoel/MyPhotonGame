using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{   
    [HideInInspector]
    public int playerID;

    [Header("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rb;
    public Player photonPlayer;

    // called when the player object is instantiated
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        playerID = player.ActorNumber;

        GameManager.instance.players[playerID - 1] = this;

        // give the first player the hat
        // give the first player the hat
        if (playerID == 1)
            GameManager.instance.GiveHat(playerID, true);
        // if this isn't our local player, disable physics as that's
        // controlled by the user and synced to all other clients
        if (!photonView.IsMine)
            rb.isKinematic = true;
    }


    // Start is called before the first frame update
    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(curHatTime >= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, playerID);
            }
        }
        if(photonView.IsMine)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }
            if(hatObject.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }

        }
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;
        rb.velocity = new Vector3(x, rb.velocity.y, z);
       
    }
    private void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
        {
            if (!photonView.IsMine)
                return;

            // did we hit another player?
            if (collision.gameObject.CompareTag("Player"))
            {
                // do they have the hat?
                if (GameManager.instance.GetPlayer(collision.gameObject).playerID == GameManager.instance.playerWithHat)
                {
                    // can we get the hat?
                    if (GameManager.instance.CanGetHat())
                    {
                        // give us the hat
                        GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, playerID, false);
                    }
                }
            }
        }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if (stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }
}
