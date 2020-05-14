using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{

    public GameObject XRPrefab;
    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        //   Debug.Log(GameSetup.GS.spawnPoints.Length);
        // int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
        int spawnPicker = 0;
        if (PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.GS.spawnPoints[spawnPicker].position,
             GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
            Instantiate(XRPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
