using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject[] AvatarHolder;
    public Text connectingText;

    public Material[] AvatarMaterials;

    private bool hasConnected = false;
    private bool[] hasAddedAvatarMaterial = new bool[5] { false, false, false, false, false };

    private void Awake()
    {
        lobby = this; // Creates the singleton, lives withing the Mian menu scene
    }

    // Start is called before the first frame update
    void Start()
    {
        //  Debug.Log("Started search for  photon lobby");
        PhotonNetwork.ConnectUsingSettings(); // Connects to Master photon server.
    }

    public override void OnConnectedToMaster()
    {
        Destroy(GameObject.FindObjectOfType<SpawnPlotController>().gameObject);
        // ""Hides"" "connecting" text and shows avatars
        hasConnected = true;

        // !!!AVATAR
        // foreach (GameObject avatar in AvatarHolder)
        // {
        //     avatar.SetActive(true);
        // }

        //  Debug.Log("Player has connected to the Photon MasterServer");
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PlayerInfo.PI != null)
        {
            PlayerInfo.PI.mySelectedCharacter = 0;
            PlayerPrefs.SetInt("MyCharacter", 0);
        }


        //Debug.Log("my character" + 0);
        OnJoinRoomButton();

    }

    public void OnJoinRoomButton()
    {
        // Debug.Log("Joining room");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Debug.Log("tried to join a room but failed");
        CreateRoom();
    }

    void CreateRoom()
    {
        // Debug.Log("tried to create a room");
        int randomRoomName = Random.Range(0, 1000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("room" + randomRoomName, roomOps);
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("We are now  in a room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Debug.Log("Tried to create room but failed, there must be already a room with the same name");
        CreateRoom();
    }

    public void OnleaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasConnected)
        {

            if (AvatarHolder != null)
            {
                for (int avatarIndex = 0; avatarIndex < AvatarHolder.Length; avatarIndex++)
                {
                    if (!hasAddedAvatarMaterial[avatarIndex])
                    {

                        //changes color of that avatar
                        if (AvatarHolder[avatarIndex].transform.childCount >= 2)
                        {
                            // Debug.Log("Changed color of avatar " + avatarIndex);
                            hasAddedAvatarMaterial[avatarIndex] = true;

                            // SkinnedMeshRenderer rend = AvatarHolder[avatarIndex].transform.GetChild(2).GetChild(1).GetComponent<SkinnedMeshRenderer>();
                            // Debug.Log(rend.material.name);
                            // Debug.Log(rend.sharedMaterial.shader.GetPropertyType(3).ToString());
                            // // rend.sharedMaterial.shader.GetPropertyType(3):
                            // rend.material.SetColor(3, new Color32(255, 0, 0, 255));
                            // Debug.Log(rend.material.GetColor(3));

                            // AvatarHolder[avatarIndex].GetComponent<OvrAvatar>().ControllerShader = 
                            // rend.material.SetColor("_BaseColor", new Color32(255, 0, 0, 1));

                        }
                    }
                }
                Destroy(connectingText);
            }
        }
    }
}
