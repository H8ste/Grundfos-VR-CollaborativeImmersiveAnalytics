using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myCharacter;
    public int characterValue;


    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {

            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
        }

    }


    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        characterValue = whichCharacter;
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);

    }


}
