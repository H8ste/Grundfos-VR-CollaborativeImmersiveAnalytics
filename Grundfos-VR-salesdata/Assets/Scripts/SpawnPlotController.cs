using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.XR.Interaction.Toolkit;

public class SpawnPlotController : MonoBehaviour
{
  float minRot = 200f; float maxRot = 320f;

  public GameObject PlotControllerPrefab;

  private GameObject LeftHandGameObject;
  private GameObject RightHandGameObject;
  private UnityEngine.XR.InputDevice LeftHandInputDevice = new UnityEngine.XR.InputDevice();
  private UnityEngine.XR.InputDevice RightHandInputDevice = new UnityEngine.XR.InputDevice();
  private XRDirectInteractor LeftHandDirectInteractor;
  private XRDirectInteractor RightHandDirectInteractor;

  private GameObject spawnedPlotController;
  // private Camera cameraForCanvas;

  private bool[] alreadySpawned = new bool[2] { false, false };
  private Vector3 leftOffset = new Vector3(-90f, -90f, 0);
  private Vector3 rightOffset = new Vector3(90f, 90f, 0);

  [HideInInspector]
  public GameObject FlippedHand { get { return flippedHand; } private set { flippedHand = value; } }
  private GameObject flippedHand = null;

  [HideInInspector]
  public GameObject NonFlippedHand { get { return nonFlippedHand; } private set { nonFlippedHand = value; } }
  private GameObject nonFlippedHand = null;

  // Start is called before the first frame update
  void Start()
  {
    XRController[] hands = gameObject.GetComponentsInChildren<XRController>();
    LeftHandGameObject = hands[0].gameObject;
    RightHandGameObject = hands[1].gameObject;

    LeftHandDirectInteractor = LeftHandGameObject.GetComponent<XRDirectInteractor>();
    RightHandDirectInteractor = RightHandGameObject.GetComponent<XRDirectInteractor>();
    // cameraForCanvas = transform.parent.GetChild(0).GetComponent<Camera>();

  }

  void Update()
  {
    FindControllers();

    // DirectInteractor Isn't even there
    //  should check for menu
    // DirectInteractor is there
    //  but neither interactor is selected and active
    //    Should check for menu



    if (MenuCouldBeUp() || isMenuUp())
    {
      // Debug.Log("Menu could be up");

      CheckForMenu();
      if (isMenuUp())
      {
        if (NonFlippedHand && FlippedHand)
        {
          // removes XRDirectInteractor on hand
          XRDirectInteractor directInteractor = NonFlippedHand.GetComponent<XRDirectInteractor>();
          if (directInteractor)
          {
            Destroy(directInteractor.attachTransform.gameObject);
            Destroy(directInteractor);
            // Debug.Log("Removed existing directInteractor");
          }
        }
        else
        {
          if (!LeftHandGameObject.GetComponent<XRDirectInteractor>() && !LeftHandGameObject.GetComponent<XRRayInteractor>())
          {
            // Debug.Log("Added xray");
            XRRayInteractor ray = LeftHandGameObject.AddComponent<XRRayInteractor>();
            // And Change XR Ray to be a line
            ray.lineType = XRRayInteractor.LineType.ProjectileCurve;
            ray.Velocity = 8;
            ray.enabled = true;
            LineRenderer lineRender = LeftHandGameObject.AddComponent<LineRenderer>();
            XRInteractorLineVisual xRInteractorLineVisual = LeftHandGameObject.AddComponent<XRInteractorLineVisual>();
          }
          if (!RightHandGameObject.GetComponent<XRDirectInteractor>() && !RightHandGameObject.GetComponent<XRRayInteractor>())
          {
            // Debug.Log("Added xray");
            XRRayInteractor ray = RightHandGameObject.AddComponent<XRRayInteractor>();
            // And Change XR Ray to be a line
            ray.lineType = XRRayInteractor.LineType.ProjectileCurve;
            ray.Velocity = 8;
            ray.enabled = true;
            LineRenderer lineRender = RightHandGameObject.AddComponent<LineRenderer>();
            XRInteractorLineVisual xRInteractorLineVisual = RightHandGameObject.AddComponent<XRInteractorLineVisual>();
          }
        }
      }
      else
      {
        // left
        XRRayInteractor ray = LeftHandGameObject.GetComponent<XRRayInteractor>();
        if (ray)
        {
          Destroy(LeftHandGameObject.GetComponent<XRInteractorLineVisual>());
          Destroy(LeftHandGameObject.GetComponent<LineRenderer>());
          Destroy(ray.attachTransform.gameObject);
          Destroy(ray);
          // Debug.Log("Removed XRRAY");
        }
        else
        {
          XRDirectInteractor directInteractor = LeftHandGameObject.GetComponent<XRDirectInteractor>();
          if (!directInteractor)
          {
            LeftHandGameObject.AddComponent<XRDirectInteractor>();
            // Debug.Log("Added XRDirectInteractor");
          }
        }

        // right
        ray = RightHandGameObject.GetComponent<XRRayInteractor>();
        if (ray)
        {
          Destroy(RightHandGameObject.GetComponent<XRInteractorLineVisual>());
          Destroy(RightHandGameObject.GetComponent<LineRenderer>());
          Destroy(ray.attachTransform.gameObject);
          Destroy(ray);
          // Debug.Log("Removed XRRAY");
        }
        else
        {
          XRDirectInteractor directInteractor = RightHandGameObject.GetComponent<XRDirectInteractor>();
          if (!directInteractor)
          {
            RightHandGameObject.AddComponent<XRDirectInteractor>();
            // Debug.Log("Added XRDirectInteractor");
          }
        }
      }
    }
    else
    {
      // Debug.Log("Menu can't be up");
    }

  }

  private bool MenuCouldBeUp()
  {
    if (LeftHandDirectInteractor || RightHandDirectInteractor)
    {
      if (LeftHandDirectInteractor.isSelectActive || RightHandDirectInteractor.isSelectActive)
      {
        // Debug.Log("Menu Could Be up - 1");
        return false;
      }
      else
      {
        // Debug.Log("Menu Could Be up - 2");
        return true;
      }
    }
    else
    {
      return true;
    }
    // else
    // {
    //   // hand director isn't there, is xrray interactor there?
    //   if (FlippedHand && NonFlippedHand)
    //   {
    //     if (FlippedHand.GetComponent<XRRayInteractor>())
    //     {

    //     }
    //   }
    //   Debug.Log("Menu Could Be up -3 ");
    //   return true;
    // }
  }

  private void FindControllers()
  {
    if (!LeftHandInputDevice.isValid)
    {
      List<UnityEngine.XR.InputDevice> leftInputDevices = new List<UnityEngine.XR.InputDevice>();
      UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftInputDevices);

      if (leftInputDevices.Count == 1)
      {
        LeftHandInputDevice = leftInputDevices[0];
        Debug.Log(string.Format("Device name '{0}' with role '{1}'", LeftHandInputDevice.name, LeftHandInputDevice.characteristics.ToString()));
      }
      else if (leftInputDevices.Count > 1)
      {
        Debug.Log("Found more than one left hand!");
      }
      else if (leftInputDevices.Count == 0)
      {
        // Debug.Log("Found no left hand");
      }
    }
    //left

    //rigth
    if (!RightHandInputDevice.isValid)
    {
      List<UnityEngine.XR.InputDevice> rightInputDevices = new List<UnityEngine.XR.InputDevice>();
      UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightInputDevices);

      if (rightInputDevices.Count == 1)
      {
        RightHandInputDevice = rightInputDevices[0];
        Debug.Log(string.Format("Device name '{0}' with role '{1}'", RightHandInputDevice.name, RightHandInputDevice.characteristics.ToString()));
      }
      else if (rightInputDevices.Count > 1)
      {
        Debug.Log("Found more than one right hand!");
      }
      else if (rightInputDevices.Count == 0)
      {
        // Debug.Log("Found no right hand");
      }
    }
  }



  public void CheckForMenu()
  {
    float RHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles.z;
    float LHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles.z;

    // Continiously check if hand is tilted and not grabbing anything
    // If that is the case, spawn the canvas for plotcontroller as child to hand (remember to have canvas in worldspace)
    if (LHandRot > 360 - maxRot && LHandRot < 360 - minRot)
    {
      Spawn("left");
    }
    else
    {
      DeSpawn("left");
    }

    if (RHandRot < maxRot && RHandRot > minRot)
    {
      Spawn("right");
    }
    else
    {
      DeSpawn("right");
    }
  }

  private void Spawn(string hand)
  {
    FlippedHand = null;
    NonFlippedHand = null;
    if (alreadySpawned[0] || alreadySpawned[1])
    { }
    else
    {
      switch (hand)
      {
        case "left":
          FlippedHand = LeftHandGameObject;
          NonFlippedHand = RightHandGameObject;
          alreadySpawned[0] = true;
          break;

        case "right":
          FlippedHand = RightHandGameObject;
          NonFlippedHand = LeftHandGameObject;
          alreadySpawned[1] = true;
          break;
      }
      spawnedPlotController = Instantiate(PlotControllerPrefab);
      spawnedPlotController.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
      spawnedPlotController.transform.SetParent(FlippedHand.transform);
      spawnedPlotController.transform.localPosition = new Vector3(0, -0.12f, 0);


    }
  }

  private void DeSpawn(string hand)
  {
    int handIndex = -1;
    switch (hand)
    {
      case "left":
        handIndex = 0;
        break;

      case "right":
        handIndex = 1;
        break;
    }

    if (alreadySpawned[handIndex])
    {
      switch (hand)
      {
        case "left":
          FlippedHand = LeftHandGameObject;
          break;

        case "right":
          FlippedHand = RightHandGameObject;
          break;
      }

      if (spawnedPlotController)
        Destroy(spawnedPlotController);

      alreadySpawned[handIndex] = false;
      FlippedHand = null;
      NonFlippedHand = null;
    }
  }

  public bool isMenuUp()
  {
    return spawnedPlotController;
  }
}
