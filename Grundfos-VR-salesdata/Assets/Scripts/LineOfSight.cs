using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    // Start is called before the first frame update

    private RaycastHit vision;
    public float rayLenght;
    private bool isGrabbed;
    private Rigidbody grabbedObject;
    void Start()
    {
        rayLenght = 4.0f;
        isGrabbed = false;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red, 0.5f);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out vision, rayLenght)) ;
        {
            if (vision.collider.tag == "PlotMesh")
            {
                Debug.Log(vision.collider.name);
            }
            if (Input.GetKeyDown(KeyCode.E) && !isGrabbed)
            {
                grabbedObject = vision.rigidbody;
                grabbedObject.isKinematic = true;
                grabbedObject.transform.SetParent(gameObject.transform);
            }

            else if (isGrabbed && Input.GetKeyDown(KeyCode.E))
            {

                grabbedObject.transform.parent = null;
                grabbedObject.isKinematic = false;
                isGrabbed = false;


            }




        }
    }
}
