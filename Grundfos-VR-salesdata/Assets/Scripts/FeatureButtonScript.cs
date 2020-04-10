using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureButtonScript : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject ScrollPrefab;

  public GameObject featureMenu;
  public int featureNumber;

  private LocalPlotController controller;
  public static bool featureMenuSpawned = false;

  public void Start()
  {
    controller = GameObject.FindObjectsOfType<LocalPlotController>()[0].GetComponent<LocalPlotController>();

  }
  public void OnClick()
  {

    if (!featureMenuSpawned)
    {
      featureMenu = Instantiate(ScrollPrefab, transform.position, Quaternion.identity) as GameObject;
      featureMenu.transform.GetChild(0).GetComponent<ButtonListControl>().FeatureNumber = featureNumber;
      featureMenu.transform.SetParent(transform.parent.parent);
      featureMenu.transform.localPosition = new Vector3(0, 0, 0);
      featureMenu.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
      featureMenu.transform.localEulerAngles = new Vector3(0, 0, 0);

      Debug.Log("im here");
      featureMenuSpawned = true;
    }

  }
  public void Update()
  {
    // if (controller.featuresChosen[featureNumber] != -1)
    // {
    //   //set text of this button equal to that feature
    //   transform.GetComponentInChildren<Text>().text = controller.headers[controller.featuresChosen[featureNumber]];
    // }
  }


}
