using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Canvas;

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
            featureMenu = Instantiate(Canvas, transform.position, Quaternion.identity) as GameObject;
            featureMenu.transform.GetChild(0).GetComponent<ButtonListControl>().FeatureNumber = featureNumber;
            featureMenu.transform.SetParent(controller.canvas.transform);
            Debug.Log("im here");
            featureMenuSpawned = true;
        }

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
