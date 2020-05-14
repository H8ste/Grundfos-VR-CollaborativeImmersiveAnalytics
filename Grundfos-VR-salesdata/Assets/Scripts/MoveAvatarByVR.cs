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
            transform.GetChild(2).position = XRRig.position;
            transform.GetChild(2).rotation = XRRig.rotation;

            transform.GetChild(3).position = XRRig.parent.GetChild(1).position;
            transform.GetChild(3).rotation = XRRig.parent.GetChild(1).rotation;

            transform.GetChild(4).position = XRRig.parent.GetChild(2).position;
            transform.GetChild(4).rotation = XRRig.parent.GetChild(2).rotation;

        }
    }
}
