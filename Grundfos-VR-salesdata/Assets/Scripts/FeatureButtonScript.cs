using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureButtonScript : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject ScrollPrefab;

  [HideInInspector]
  public GameObject featureMenu;
  public int featureNumber;

  // private LocalPlotController controller;
  public static bool featureMenuSpawned = false;

  private LocalPlotController plotControllerRef;

  public void Start()
  {
    plotControllerRef = GameObject.FindObjectsOfType<LocalPlotController>()[0].GetComponent<LocalPlotController>();
  }
  public void OnClick()
  {
    Debug.Log("clicked Featurebutton: " + featureNumber);
    plotControllerRef.spawnFeatureSelection(featureNumber);
  }
  public void Update()
  {
    if (plotControllerRef.featuresChosen[featureNumber] != -1)
    {
      //set text of this button equal to that feature
      transform.GetComponentInChildren<Text>().text = plotControllerRef.GetDataReader().GetHeaders()[plotControllerRef.featuresChosen[featureNumber]];
    }
  }


}
