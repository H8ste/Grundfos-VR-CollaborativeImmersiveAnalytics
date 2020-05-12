using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject battleButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this; // Creates the singleton, lives withing the Mian menu scene


    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started search for  photon lobby");
        PhotonNetwork.ConnectUsingSettings(); // Connects to Master photon server.

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon MasterServer");
        PhotonNetwork.AutomaticallySyncScene = true;
        battleButton.SetActive(true);

    }

    public void OnBattleButtonClicked()
    {
        Debug.Log("Battle button clicked");

        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();


    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {


        Debug.Log("tried to join a room but failed");
        CreateRoom();

    }

    void CreateRoom()
    {
        Debug.Log("tried to create a room");
        int randomRoomName = Random.Range(0, 1000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("room" + randomRoomName, roomOps);



    }
    public override void OnJoinedRoom()
    {

        Debug.Log("We are now  in a room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create room but failed, there must be already a room with the same name");
        CreateRoom();

    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();


    }

    // Update is called once per frame
    void Update()
    {

    }
}
