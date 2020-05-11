using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // room info PV;
    public static PhotonRoom room;
    private PhotonView PV;

    // public bool isGameLoaded;
    public int currentScene;
    public int multiplayScene;

    //Player[] photonPlayers;
    //public int myNumberInRoom;
    //public int playersInGame;
    //

    private void Awake()
    {

        // set up singleton
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {


            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();

    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {

        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }


    void Start()
    {
        PV = GetComponent<PhotonView>();

    }

    public override void OnJoinedRoom()
    {

        Debug.Log("We are now  in a room");
        base.OnJoinedRoom();
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartGame();



        // photonPlayers=PhotonNetwork.PlayerList;
        // playersInRoom = photonPlayers.Length;
        //myNumberInRoom = playersInRoom;
        //PhotonNetwork.NickName = myNumberInRoom.ToString();

    }
    void StartGame()
    {
        // if(!PhotonNetwork.IsMasterClient)
        //  return ;

        PhotonNetwork.LoadLevel(multiplayScene);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == multiplayScene)
        {
            CreatePlayer();
        }

    }
    private void CreatePlayer()
    {


        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

}
