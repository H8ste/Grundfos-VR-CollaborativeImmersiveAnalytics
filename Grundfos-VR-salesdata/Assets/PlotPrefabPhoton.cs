using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlotPrefabPhoton : MonoBehaviour
{
    private PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine == false && PhotonNetwork.IsConnected == true)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
