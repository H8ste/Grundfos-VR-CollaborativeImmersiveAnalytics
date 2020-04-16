using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Unity.Oculus.XR;
// using Unit
using Unity.XR.Oculus;
namespace UnityEngine.XR.Interaction.Toolkit
{

  // using UnityEngine
  public enum HandSide
  {
    Left,
    Right
  }

  public class XRPointerInteractor : MonoBehaviour
  {

    public HandSide handSide = new HandSide();

    private List<UnityEngine.XR.InputDevice> inputDevice;

    private UnityEngine.XR.InputDevice hand;

    private bool controllerFound = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      FindController();
      // If hand is pointing

      if (controllerFound)
      {
        float pointerValue;
        if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.indexTouch, out pointerValue) && pointerValue < 1)
        {
          // we are point
          Debug.Log("Is pointing: " + handSide);
          Ray ray = new Ray(transform.position, transform.forward);

          // Bit shift the index of the layer (8) to get a bit mask
          int layerMask = 1 << 11;

          // This would cast rays only against colliders in layer 8.
          // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
          // layerMask = ~layerMask;

          RaycastHit hit;
          // Physics.Raycast(transform.position,
          //   transform.TransformDirection(Vector3.forward),
          //   out hit, Mathf.Infinity, layerMask);
          if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50f, layerMask))
          {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("Did Hit");
            hit.transform.GetComponent<XRScaleInteractable>().XRPointerHit(hit.point - hit.collider.gameObject.transform.position);
          }
          else
          {
            hit.transform.GetComponent<XRScaleInteractable>().XRNoPointerHit();
          }
          // Debug.DrawRay(ray.origin, ray.direction * 15, Color.yellow);
        }


        // Unity.XR.Oculus.OculusUsages.indexTouch
        if (handSide == HandSide.Left)
        {
          // Debug.Log(OVRInput.Get(OVRInput.RawTouch.));
          //   Debug.Log(UnityEngine.XR.CommonUsages.indexTouch);
        }
        else
        {
          //   Debug.Log(OVRInput.GetUp(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RHand));
          //Debug.Log(OVRInput.Get(OVRInput.RawTouch.RIndexTrigger));

        }
        // Debug.Log(UnityEngine.XR.CommonUsages.triggerButton.Touch);



        // hand.TryGetFeatureValue(Unity.XR.Oculus.OculusUsages.indexTouch, out pointerValue);
        // if (true)
        // {
        //   Debug.Log("Is pointing: " + pointerValue);
        // }
        // else
        // {
        //   //   Debug.Log("Couldn't check value");
        // }
      }
      else
      {
        Debug.Log("Couldn't find controller");
      }

      // Spawn ray

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





