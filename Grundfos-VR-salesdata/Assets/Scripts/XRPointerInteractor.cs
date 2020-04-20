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


    // Start is called before the first frame update
    void Start()
    {
      PlotController = gameObject.GetComponentInParent<SpawnPlotController>();
    }

    // Update is called once per frame
    void Update()
    {
      FindController();
      // If hand is pointing

      if (controllerFound && !PlotController.isMenuUp())
      {
        float pointerValue;
        if (hand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.indexTouch, out pointerValue) && pointerValue < 1)
        {
          // we are point
          // Debug.Log("Is pointing: " + handSide);
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
            alreadyDeleted = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("Did Hit");
            hit.transform.GetComponent<HandlePoints>().XRPointerHit(hit.point - hit.collider.gameObject.transform.position, handSide, hit.point + new Vector3(0, 0, -0.02f));
            prevHit = hit;
          }
          else
          {
            if (!alreadyDeleted && prevHit.transform)
            {
              alreadyDeleted = true;
              prevHit.transform.GetComponent<HandlePoints>().XRNoPointerHit(handSide);
            }


          }
          Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f, Color.yellow);
        }
        else
        {
          if (!alreadyDeleted)
          {
            // alreadyDeleted = true;
            // if (prevHit.point != null)
            // {
            //   prevHit.transform.GetComponent<HandlePoints>().XRNoPointerHit(handSide);
            // }

          }
        }

      }
      else
      {
        // Debug.Log("Couldn't find controller");
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





