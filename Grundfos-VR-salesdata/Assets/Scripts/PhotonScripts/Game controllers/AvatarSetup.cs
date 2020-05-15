using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;
    public int characterValue;
    public GameObject leftController;
    public GameObject rigthController;


    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {

            PV.RPC("RPC_AddCharacter", RpcTarget.OthersBuffered, PlayerInfo.PI.mySelectedCharacter);
            // PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, 0);

        }


    }


    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        // characterValue = whichCharacter;
        // myCharacter = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Cylinder"), GameSetup.GS.spawnPoints[0].position, Quaternion.identity);
        // myCharacter.transform.SetParent(transform, false);
        // // myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        // // Instantiate(leftController, transform.position, transform.rotation, transform);
        // // Instantiate(rigthController, transform.position, transform.rotation, transform);
        // GameObject leftController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "QuestLeftController"), GameSetup.GS.spawnPoints[0].position, Quaternion.identity);
        // leftController.transform.SetParent(transform, false);
        // GameObject rigthController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "QuestRightController"), GameSetup.GS.spawnPoints[0].position, Quaternion.identity);
        // rigthController.transform.SetParent(transform, false);
    }


}
