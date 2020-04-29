using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization; //used to ensure correct parsing of comma numbers

public class LocalPlotController : MonoBehaviour
{
    public string[] headers;
    public List<int> featuresChosen = new List<int>();
    private enum plotType { };
    private int width, height;

    public bool inVR;

    public GameObject createButtonPrefab;
    public GameObject plotCreatorPrefab;
    public GameObject plotPrefab;

    public GameObject ThresholdingSlidersPrefab;

    public GameObject ScrollPrefab;

    private GameObject plot;
    private GameObject featureMenu;

    private GameObject ThresholdingSliders;

    private GameObject PlotComfirmButton;



    DataReader dataReader;

    void Start()
    {
        gameObject.AddComponent<DataReader>();
        dataReader = gameObject.GetComponent<DataReader>();
        headers = dataReader.GetHeaders();

        featuresChosen.Add(-1);
        featuresChosen.Add(-1);
    }

    private void HideMenu()
    {
        // transform.GetComponentInChildren<
        gameObject.GetComponentInChildrenWithTag<RectTransform>("PlotCreatorIntro").gameObject.SetActive(false);
        // transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    private void ShowMenu()
    {
        gameObject.GetComponentInChildrenWithTag<RectTransform>("PlotCreatorIntro").gameObject.SetActive(false);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    GameObject plotCreator;
    // Spawns in screen that is used to create a new plot
    public void NewPlotCreator()
    {


        // Hide Menu Screen
        HideMenu();

        // // Spawn
        plotCreator = GameObject.Instantiate(plotCreatorPrefab);
        plotCreator.transform.SetParent(transform.GetChild(0));
        plotCreator.transform.localPosition = new Vector3(0, 0, 0);
        plotCreator.transform.localScale = new Vector3(1f, 1f, 1f);
        plotCreator.transform.localEulerAngles = new Vector3(0, 0, 0);

        if (!PlotComfirmButton)
            PlotComfirmButton = GameObject.FindObjectOfType<PushPlotToGlobalPlot>().gameObject;
        PlotComfirmButton.gameObject.SetActive(false);

    }


    public void Update()
    {
        if (inVR)
        {    //rotate towards headset

            // Determine which direction to rotate towards
            Vector3 targetDirection = transform.position - transform.parent.parent.GetChild(0).position;

            // The step size is equal to speed times frame time.
            float singleStep = 3f * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
            // transform.localPosition = new Vector3(0, 0, 0);}
        }
    }


    public void setFeature(int featureNumber, int featureID)
    {
        featuresChosen[featureNumber] = featureID;
        // check if both features have been selected,
        bool ready = true;
        foreach (var feature in featuresChosen)
        {
            if (feature == -1)
            {
                ready = false;
                break;
            }
        }
        if (ready)
        {
            if (plot != null)
            {
                GameObject.Destroy(plot);
            }
            plot = GameObject.Instantiate(plotPrefab);
            plot.GetComponent<MeshHandler>().CreateNewPlot(featuresChosen[0], featuresChosen[1], dataReader, TypeOfPlot.Barchart);

            plot.transform.SetParent(transform);
            plot.transform.localPosition = new Vector3(-2.69f, -2.58f, 0.005f);
            plot.transform.localScale = new Vector3(1f, 1f, 1f);
            plot.transform.localEulerAngles = new Vector3(0, 0, 0);

            if (ThresholdingSliders == null)
            {
                ThresholdingSliders = GameObject.Instantiate(ThresholdingSlidersPrefab);
                ThresholdingSliders.transform.SetParent(plotCreator.transform, false);
            }
            SliderRange[] sliders = ThresholdingSliders.GetComponentsInChildren<SliderRange>();

            PlotComfirmButton.gameObject.SetActive(true);

            List<string> allYs = new List<string>();
            foreach (List<string> row in plot.GetComponent<MeshHandler>().plot.DataCompared)
            {
                foreach (string entry in row)
                {
                    if (float.TryParse(entry, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                    {

                    }

                    allYs.Add(entry);
                }
            }

            // Resetting threshold to be that of min and max
            float[] xThresholds = plot.GetComponent<MeshHandler>().FindMinMaxValues(plot.GetComponent<MeshHandler>().plot.DataComparedHeaders.ToArray());
            Debug.Log("x: " + xThresholds[0] + "," + xThresholds[1]);
            sliders[0].setSliderValues(xThresholds);

            float[] yThresholds = plot.GetComponent<MeshHandler>().FindMinMaxValues(allYs.ToArray());
            Debug.Log("y: " + yThresholds[0] + "," + yThresholds[1]);
            sliders[1].setSliderValues(yThresholds);
        }

    }
    private bool isReady()
    {
        if (featuresChosen.Count == 4)
        {
            return true;
        }

        return false;
    }

    private void renderFeatureSelector(int featureNumber)
    {



    }


    private void renderFeatureSelector(int featureNumber, int featureID)
    {



    }

    private void sendPlot(GameObject plot)
    {


    }
    private void reRenderLocalVersion()
    {



    }

    private int featureBeingChanged = -1;
    public void spawnFeatureSelection(int feature)
    {
        if (plot)
        {
            plot.SetActive(false);
        }
        // If feature menu isn't already spawned
        if (!featureMenu)
        {
            featureBeingChanged = feature;
            featureMenu = Instantiate(ScrollPrefab, transform.position, Quaternion.identity) as GameObject;
            featureMenu.GetComponentInChildren<ButtonListControl>().BeginControl();
            featureMenu.transform.SetParent(transform.GetChild(0));
            featureMenu.transform.localPosition = new Vector3(0, 0, 0);
            featureMenu.transform.localScale = new Vector3(1f, 1f, 1f);
            featureMenu.transform.localEulerAngles = new Vector3(0, 0, 0);


            Button[] temp = plotCreator.GetComponentsInChildren<Button>();

            //make both feature buttons inactive
            foreach (var button in temp)
            {
                button.interactable = false;
            }
        }
    }

    public void confirmFeatureSelection(int featureSelected)
    {
        if (plot)
        {
            plot.SetActive(true);
        }
        if (featureSelected != -1)
        {
            setFeature(featureBeingChanged, featureSelected);
            Object.Destroy(featureMenu);
            featureBeingChanged = -1;
        }

        Button[] temp = plotCreator.GetComponentsInChildren<Button>();

        //make both feature buttons inactive
        foreach (var button in temp)
        {
            button.interactable = true;
        }

    }

    public DataReader GetDataReader()
    {
        return dataReader;
    }

    public void SliderValueChanged(SliderAxis axis, float minValue, float maxValue)
    {
        // Using provided min and max, change plotoptions threshold for appropiate axis
        switch (axis)
        {
            case SliderAxis.x:
                // Debug.Log(plot.GetComponent<MeshHandler>().plot.PlotOptions.PlotLength);
                plot.GetComponent<MeshHandler>().plot.PlotOptions.XThresholds = new float[2] { minValue, maxValue };
                // Debug.Log(plot.GetComponent<MeshHandler>().plot.PlotOptions.XThresholds[0] + " " + plot.GetComponent<MeshHandler>().plot.PlotOptions.XThresholds[1]);
                break;
            case SliderAxis.y:
                // Debug.Log("y is being changed");
                plot.GetComponent<MeshHandler>().plot.PlotOptions.YThresholds = new float[2] { minValue, maxValue };
                // Debug.Log(plot.GetComponent<MeshHandler>().plot.PlotOptions.YThresholds[0] + " " + plot.GetComponent<MeshHandler>().plot.PlotOptions.YThresholds[1]);
                break;
        }
        // Enforce a recomputation of comparedData
        plot.GetComponent<MeshHandler>().ThresholdPlot();
    }

    public GameObject GetPlot()
    {
        return plot;
    }




}
