using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Photon.Pun;

// namespace Photon
// {
public class TestScriptPhoton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool checkIfParentIsMine()
    {
        return transform.GetComponent<PhotonView>().IsMine;
    }
}
// }