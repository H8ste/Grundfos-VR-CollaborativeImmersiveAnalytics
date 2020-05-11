using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class BUttonTest : MonoBehaviour


{
    // Start is called before the first frame update

    [SerializeField]
    Canvas canvas1;
    void Start()
    {

    }

    // Update is called once per frame
    public void OnClick()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Plot"), canvas1.transform.position, Quaternion.identity, 0);
    }

    // [PunRPC]
    //  void RPC_SincColors()
}
