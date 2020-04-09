using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.XR.Interaction.Toolkit
{
  public class ControlRayLogic : MonoBehaviour
  {
    private SpawnPlotController MenuRef;

    private List<UnityEngine.XR.InputDevice> leftHandDevices, rightHandDevices;

    private UnityEngine.XR.InputDevice leftHand, rightHand;

    private bool[] controllerFound = new bool[2] { false, false };

    private GameObject LeftHandRef, RightHandRef;

    private bool[] triggerHeld = new bool[2] { false, false };

    // Start is called before the first frame update



    void Start()
    {
      if (!MenuRef)
      {
        MenuRef = transform.parent.GetComponentInChildren<SpawnPlotController>();
      }

      if (!LeftHandRef)
      {
        LeftHandRef = transform.parent.GetChild(1).gameObject;

      }
      if (!RightHandRef)
      {
        RightHandRef = transform.parent.GetChild(2).gameObject; ;
      }

      findControllers();
    }

    // Update is called once per frame
    void Update()
    {
      findControllers();

      MenuRef.CheckForMenu();

      if (MenuRef.isMenuUp())
      {
        if (MenuRef.FlippedHand)
        {
          // Disable Any potential XR Ray on current hand if they are not already converted to line renderer
          XRRayInteractor rayInteractorRef = MenuRef.FlippedHand.GetComponent<XRRayInteractor>();
          rayInteractorRef.enabled = false;

          // 	Add XR Ray interactable on opposite hand (straight line) if Menu is UP
          if (MenuRef.NonFlippedHand)
          {
            rayInteractorRef = MenuRef.NonFlippedHand.GetComponent<XRRayInteractor>() as XRRayInteractor;
            // And Change XR Ray to be a line
            rayInteractorRef.enabled = true;
            rayInteractorRef.lineType = XRRayInteractor.LineType.StraightLine;

            // And change it such that this ray only interacts with UI Elements
            // rayInteractorRef.InteractionLayerMask =
          }
          else
          {
            // Debug.Log("Couldn't find ref for nonflippedhand");
          }
        }
      }
      else
      {
        // If button is held Add XR Ray interactable to contorller (curvy  thick line)
        // Check left hand
        bool leftTriggerValue;
        if (leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTriggerValue) && leftTriggerValue)
        {
          if (triggerHeld[0] == false)
          {
            triggerHeld[0] = true;
            Debug.Log("Left Trigger button is pressed.");
            XRRayInteractor rayInteractorRef = LeftHandRef.GetComponent<XRRayInteractor>() as XRRayInteractor;
            rayInteractorRef.lineType = XRRayInteractor.LineType.ProjectileCurve;
            rayInteractorRef.Velocity = 8;
            rayInteractorRef.enabled = true;

            // And change it such that this ray only interacts with Area Elements
            // rayInteractorRef.InteractionLayerMask =
          }
        }
        else
        {
          triggerHeld[0] = false;
          LeftHandRef.GetComponent<XRRayInteractor>().enabled = false;
        }
        bool rigthTriggerValue;
        if (rightHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rigthTriggerValue) && rigthTriggerValue)
        {
          if (triggerHeld[1] == false)
          {
            triggerHeld[1] = true;
            Debug.Log("Right Trigger button is pressed.");
            XRRayInteractor rayInteractorRef = RightHandRef.GetComponent<XRRayInteractor>() as XRRayInteractor;
            rayInteractorRef.lineType = XRRayInteractor.LineType.ProjectileCurve;
            rayInteractorRef.Velocity = 8;
            rayInteractorRef.enabled = true;
          }
        }
        else
        {
          triggerHeld[1] = false;
          RightHandRef.GetComponent<XRRayInteractor>().enabled = false;
        }
        // Check right hand
      }
    }

    private void findControllers()
    {
      if (!controllerFound[0])
      {
        leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count == 1)
        {
          controllerFound[0] = true;
          leftHand = leftHandDevices[0];
          Debug.Log(string.Format("Device name '{0}' with role '{1}'", leftHand.name, leftHand.characteristics.ToString()));
        }
        else if (leftHandDevices.Count > 1)
        {
          Debug.Log("Found more than one left hand!");
        }
        else if (leftHandDevices.Count == 0)
        {
          // Debug.Log("Found no left hand");
        }
      }
      //left

      //rigth
      if (!controllerFound[1])
      {
        rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        if (rightHandDevices.Count == 1)
        {
          controllerFound[1] = true;
          rightHand = rightHandDevices[0];
          Debug.Log(string.Format("Device name '{0}' with role '{1}'", rightHand.name, rightHand.characteristics.ToString()));
        }
        else if (rightHandDevices.Count > 1)
        {
          Debug.Log("Found more than one left hand!");
        }
        else if (rightHandDevices.Count == 0)
        {
          // Debug.Log("Found no right hand");
        }
      }
    }

    public void TeleportEnter()
    {

      // Debug.Log("Teleporting should SHOW now");
    }

    public void TeleportLeave()
    {
      // Debug.Log("Teleporting should HIDE now");
    }
  }
}
