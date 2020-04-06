using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlotController : MonoBehaviour
{
    public string[] headers;
    public List<int> featuresChosen = new List<int>();
    private enum plotType { };
    private int width, height;

    public GameObject createPlotPrefab;

    public GameObject createNewPlotButton;

    public GameObject canvas;

    public GameObject plotPrefab;

    private GameObject plot;

    DataReader dataReader;

    void Start()
    {

        gameObject.AddComponent<DataReader>();
        dataReader = gameObject.GetComponent<DataReader>();
        headers = dataReader.GetHeaders();

        featuresChosen.Add(-1);
        featuresChosen.Add(-1);


    }


    // Create plot function
    public void CreateNewPlot()
    {
        //remove button
        // GameObject.Destroy(buttonRef);
        createNewPlotButton.SetActive(false);
        //spawn canvas
        Debug.Log("Created plot");

        GameObject oof = GameObject.Instantiate(createPlotPrefab);
        oof.transform.SetParent(canvas.transform);
        oof.transform.localPosition = new Vector3(0, 150f, 0);

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
            plot.GetComponent<CreateMesh>().Create(featuresChosen[0], featuresChosen[1], dataReader);

            // Instantiate plot prefab
            //   plot = GameObject.Instantiate();
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



}
