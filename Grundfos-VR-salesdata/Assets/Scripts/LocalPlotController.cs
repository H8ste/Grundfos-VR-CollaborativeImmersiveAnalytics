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
        GameObject.FindObjectOfType<SpawnPlotController>().rotateFeedForwardNeeded = false;

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
            MeshHandler meshHandlerRef = plot.GetComponent<MeshHandler>();
            meshHandlerRef.CreateNewPlot(featuresChosen[0], featuresChosen[1], dataReader, TypeOfPlot.Barchart);

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

            float[] xThresholds; float[] yThresholds;


            if (meshHandlerRef.plot.PlotOptions.XUniques != null)
                meshHandlerRef.plot.PlotOptions.XUniques = null;
            if (meshHandlerRef.plot.PlotOptions.YUniques != null)
                meshHandlerRef.plot.PlotOptions.YUniques = null;

            //x
            if (float.TryParse(meshHandlerRef.plot.DataComparedHeaders[0], out _))
            {
                // numerical
                xThresholds = meshHandlerRef.FindMinMaxValues(meshHandlerRef.plot.DataComparedHeaders.ToArray());
            }
            else
            {
                // alphabetical

                xThresholds = new float[2] { 0, meshHandlerRef.plot.DataComparedHeaders.Count - 1 };
                meshHandlerRef.plot.PlotOptions.XUniques = meshHandlerRef.plot.DataComparedHeaders.ToArray();
            }


            //y
            List<string> allYs = new List<string>();
            foreach (List<string> row in meshHandlerRef.plot.DataCompared)
            {
                foreach (string entry in row)
                {
                    allYs.Add(entry);
                }
            }

            if (float.TryParse(meshHandlerRef.plot.DataCompared[1][0], out _))
            {
                // numerical
                yThresholds = meshHandlerRef.FindMinMaxValues(allYs.ToArray());
            }
            else
            {
                // alphabetical
                // find all unique y possibilities
                // save those uniques into the plot for later use when thresholding
                // where we begin a for loop at int sliderValueMin (min slider) and end with sliderValueMax (max slider)
                // and if y value exist within that section, it thresholds by that 

                List<string> yUniques = new List<string>();
                HashSet<string> unique_y_values = new HashSet<string>(allYs);
                foreach (string yUnique in unique_y_values)
                {
                    yUniques.Add(yUnique);
                }
                meshHandlerRef.plot.PlotOptions.YUniques = yUniques.ToArray();
                yThresholds = new float[2] { 0, unique_y_values.Count - 1 };
            }


            // Resetting threshold to be that of min and max

            sliders[0].setSliderValues(xThresholds);

            // Debug.Log("y: " + yThresholds[0] + "," + yThresholds[1]);
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
            //find both feature buttons and hide both button image and its containing
            FeatureButtonScript[] featureButtons = transform.GetComponentsInChildren<FeatureButtonScript>();
            foreach (var featureButton in featureButtons)
            {
                featureButton.transform.GetComponent<Image>().enabled = false;
                featureButton.transform.GetComponentInChildren<Text>().enabled = false;
            }

            featureBeingChanged = feature;
            featureMenu = Instantiate(ScrollPrefab) as GameObject;
            featureMenu.GetComponentInChildren<ButtonListControl>().BeginControl();
            featureMenu.transform.SetParent(transform.GetChild(0), false);
            //   featureMenu.transform.localPosition = new Vector3(0, 0, 0);
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
        if (featureSelected != -1)
        {
            if (plot)
            {
                plot.SetActive(true);
            }
            // reenable both image and text for both feature buttons
            FeatureButtonScript[] featureButtons = transform.GetComponentsInChildren<FeatureButtonScript>();
            foreach (var featureButton in featureButtons)
            {
                featureButton.transform.GetComponent<Image>().enabled = true;
                featureButton.transform.GetComponentInChildren<Text>().enabled = true;
            }
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
