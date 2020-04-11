using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    // instance
    public static NetworkManager instance;

    void Awake()
    {
        // if an instance already exists and it's not this one - destroy us
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            // set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {   

        // We need to connect to Photon.
        PhotonNetwork.ConnectUsingSettings();
    }

   


    // When we want to create a room, we'll call the CreateRoom function, 
    //sending over a name for the room.
    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

   
    //When we want to join a room, we'll call the JoinRoom function, 
    //sending over a name for the room.
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }


    //When we want to change a scene, we're not going to use Unity's default system. 
    //Instead we're going to use Photon's and this is because 
    //they have some added features to stop sending messages between scene changes
    //and other features with multiplayer in mind.
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

}
