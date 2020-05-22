using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByJoyStick : MonoBehaviour
{
    public float speed = 0.73f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.Log("Running");
        FindController();
        if (controllerFound)
        {
            Vector2 triggerValue;

            if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out triggerValue))
            {

                transform.position = Vector3.Lerp(transform.position, new Vector3((transform.position + transform.GetComponentInChildren<Camera>().transform.forward * triggerValue.y * speed).x,
                 transform.position.y,
                 (transform.position + transform.GetComponentInChildren<Camera>().transform.forward * triggerValue.y * speed).z), Time.deltaTime * 3f);
            }
            // Debug.Log(triggerValue);
        }

    }
    private bool controllerFound = false;
    private List<UnityEngine.XR.InputDevice> inputDevice;
    UnityEngine.XR.InputDevice hand;
    void FindController()
    {
        if (!controllerFound)
        {
            inputDevice = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(transform.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRController>().controllerNode, inputDevice);
            Debug.Log(inputDevice.Count);
            if (inputDevice.Count > 0)
            {
                hand = inputDevice[0];
                controllerFound = true;
            }


        }
    }
}
