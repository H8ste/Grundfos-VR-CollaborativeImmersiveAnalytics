using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawnPlotController : MonoBehaviour
{
  public float minRot, maxRot;

  public GameObject PlotControllerPrefab;

  private bool alreadySpawned = false;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    // Quaternion diff = rotationPreset * Quaternion.Inverse(OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand));
    // Debug.Log(Quaternion.Angle(OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand), rotationPreset));
    // Debug.Log("Left hand: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand));
    float RHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles.z;
    float LHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles.z;
    if (RHandRot < maxRot && RHandRot > minRot)
    {
      //check if a plot controller has already been spawned
      if (!alreadySpawned)
      {
        
      }
      Debug.Log("RHandRot ROTATED");
    }
    else
    {
      //   Debug.Log("Right hand: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles);
    }


    if (LHandRot > 360 - maxRot && LHandRot < 360 - minRot)
    {
      //check if a plot controller has already been spawned
      if (!alreadySpawned)
      {

      }
      Debug.Log("LHandRot ROTATED");
    }
    else
    {
      //   Debug.Log("Left hand: " + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles);
    }
    // Continiously check if hand is tilted and not grabbing anything
    // If that is the case, spawn the canvas for plotcontroller as child to hand (remember to have canvas in worldspace)

  }
}
