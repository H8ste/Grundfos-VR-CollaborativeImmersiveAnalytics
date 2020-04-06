using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureButtonScript : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject Canvas;
  private GameObject featureMenu;
  public int featureNumber;
  private bool scrollBarOpen;
  private LocalPlotController controller;

  public void Start()
  {
    controller = GameObject.FindObjectsOfType<LocalPlotController>()[0].GetComponent<LocalPlotController>();
  }
  public void OnClick()
  {
    featureMenu = Instantiate(Canvas, transform.position, Quaternion.identity) as GameObject;
    featureMenu.transform.GetChild(0).GetComponent<ButtonListControl>().FeatureNumber = featureNumber;
    featureMenu.transform.SetParent(controller.canvas.transform);
  }
  public void Update()
  {
    if (controller.featuresChosen[featureNumber] != -1)
    {
      //set text of this button equal to that feature
      transform.GetComponentInChildren<Text>().text = controller.headers[controller.featuresChosen[featureNumber]];
    }
  }


}
