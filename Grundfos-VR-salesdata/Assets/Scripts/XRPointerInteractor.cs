using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Unity.Oculus.XR;
// using Unit
using Unity.XR.Oculus;
// using UnityEngine
public enum HandSide
{
    Left = (int)0,
    Right = (int)1
}

namespace UnityEngine.XR.Interaction.Toolkit
{


    public class XRPointerInteractor : MonoBehaviour
    {

        public HandSide handSide = new HandSide();

        private List<UnityEngine.XR.InputDevice> inputDevice;

        private UnityEngine.XR.InputDevice hand;

        private bool controllerFound = false;

        private RaycastHit prevHit;

        private bool alreadyDeleted = false;

        private SpawnPlotController PlotController;

        private bool prevTriggerValue = false;
        private bool hasPressedTrigger = false;


        // Start is called before the first frame update
        void Start()
        {
            PlotController = gameObject.GetComponentInParent<SpawnPlotController>();
        }

        // Update is called once per frame
        void Update()
        {
            FindController();

            if (controllerFound && !PlotController.isMenuUp())
            {
                Ray ray = new Ray(transform.position, transform.forward);

                // Bit shift the index of the layer (11) to get a bit mask
                int layerMask = 1 << 11;
                // This would cast rays only against colliders in layer 11.

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50f, layerMask))
                {
                    alreadyDeleted = false;
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                    hit.transform.GetComponent<HandlePoints>().XRPointerHit(hit.point - hit.collider.gameObject.transform.position, handSide, hit.point + new Vector3(0, 0, -0.02f));
                    prevHit = hit;

                    float triggerValue;
                    if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out triggerValue) && triggerValue > 0.98f)
                    {
                        hasPressedTrigger = true;
                    }
                    if (triggerValue < 0.02 && hasPressedTrigger)
                    {
                        hasPressedTrigger = false;
                        Debug.Log("Trigger has been released");
                        hit.transform.GetComponent<HandlePoints>().XRPointerHitSave(hit.point - hit.collider.gameObject.transform.position, handSide);
                    }
                }
                else
                {
                    if (!alreadyDeleted && prevHit.transform)
                    {
                        alreadyDeleted = true;
                        prevHit.transform.GetComponent<HandlePoints>().XRNoPointerHit(handSide);
                    }
                    hasPressedTrigger = false;
                }
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f, Color.yellow);
            }
        }

        void FindController()
        {
            if (!controllerFound)
            {
                inputDevice = new List<UnityEngine.XR.InputDevice>();
                UnityEngine.XR.InputDevices.GetDevicesAtXRNode(transform.GetComponent<XRController>().controllerNode, inputDevice);
                if (inputDevice.Count == 1)
                {
                    hand = inputDevice[0];
                    controllerFound = true;
                }
            }
        }
    }
}





