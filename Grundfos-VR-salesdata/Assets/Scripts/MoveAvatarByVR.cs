using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveAvatarByVR : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Transform XRRig = FindObjectOfType<SpawnPlotController>().GetComponentInChildren<Camera>().transform;
            transform.position = XRRig.position;
            transform.rotation = XRRig.rotation;
        }
    }
}
