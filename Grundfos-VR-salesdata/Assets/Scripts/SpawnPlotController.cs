using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SpawnPlotController : MonoBehaviour
{
  float minRot = 200f; float maxRot = 320f;

  public GameObject PlotControllerPrefab;

  private GameObject LeftHand;
  private GameObject RightHand;

  private GameObject spawnedPlotController;
  private Camera cameraForCanvas;

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

    LeftHand = transform.parent.GetChild(1).gameObject;
    RightHand = transform.parent.GetChild(2).gameObject;
    cameraForCanvas = transform.parent.GetChild(0).GetComponent<Camera>();

  }

  // Update is called once per frame
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

    if (!NonFlippedHand)
    {
      // Debug.Log("Reference is not known");
    }

    if (!RightHand)
    {
      Debug.Log("Right hand not found");
    }
    if (!LeftHand)
    {
      Debug.Log("Left hand not found");
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
          FlippedHand = LeftHand;
          NonFlippedHand = RightHand;
          alreadySpawned[0] = true;
          break;

        case "right":
          FlippedHand = RightHand;
          NonFlippedHand = LeftHand;
          alreadySpawned[1] = true;
          break;
      }
      spawnedPlotController = Instantiate(PlotControllerPrefab);
      spawnedPlotController.transform.GetChild(0).GetComponent<Canvas>().worldCamera = cameraForCanvas;
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
          FlippedHand = LeftHand;
          break;

        case "right":
          FlippedHand = RightHand;
          break;
      }
      Destroy(FlippedHand.transform.GetChild(FlippedHand.transform.childCount - 1).gameObject);
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
