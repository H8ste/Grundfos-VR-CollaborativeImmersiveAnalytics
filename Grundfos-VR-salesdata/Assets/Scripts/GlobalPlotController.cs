using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlotController : MonoBehaviour
{
    private List<GameObject> plots;

    [System.Serializable]
    public class Debugger
    {
        public bool spawnPlotOnStart = false;
        public GameObject plotPrefab;
        [HideInInspector]
        public DataReader dataReader;
    }

    public Debugger debugInstance = new Debugger();
    // Start is called before the first frame update
    void Start()
    {
        plots = new List<GameObject>();

        if (debugInstance.spawnPlotOnStart)
        {
            gameObject.AddComponent<DataReader>();
            debugInstance.dataReader = gameObject.GetComponent<DataReader>();

            GameObject tempPlot = GameObject.Instantiate(debugInstance.plotPrefab);
            // tempPlot.GetComponent<CreateMesh>().Create((int)Random.Range(0, debugInstance.dataReader.GetHeaders().Length - 1), (int)Random.Range(0, debugInstance.dataReader.GetHeaders().Length - 1), debugInstance.dataReader);
            tempPlot.GetComponent<MeshHandler>().CreateNewPlot(11, 7, debugInstance.dataReader, TypeOfPlot.Barchart);

            AddPlot(tempPlot);
        }

    }

    public void AddPlot(GameObject plotToAdd)
    {
        // Add plot provided into the local array of plots
        plots.Add(plotToAdd);

        // Change plot's position transform to be to the right of the previous element's position

        // Add plot as child to this gameobject
        plots[plots.Count - 1].transform.SetParent(transform, false);
        plots[plots.Count - 1].transform.localScale = Vector3.one;
        plots[plots.Count - 1].transform.eulerAngles = Vector3.zero;
        if (plots.Count > 1)
        {
            plots[plots.Count - 1].transform.position = plots[plots.Count - 2].transform.position + new Vector3(7f, 0, 0);
        }
        else
        {
            plots[0].transform.localPosition += new Vector3(-5f, 1f, 0);
        }
    }

    // public void DeletePlot(GameObject plotToDelete)
    // {
    //   // Using its ID, find the plot provided in the local array of plots
    //   int index = FindPlotInArray(plotToDelete.GetComponent<MeshHandler>().plotID, plots);
    //   if (index != -1)
    //   {
    //     plots.RemoveAt(index);
    //   }

    //   RenderPlots();
    // }

    int FindPlotInArray(int providedPlotID, List<GameObject> array)
    {
        foreach (GameObject plot in array)
        {
            if (providedPlotID == plot.GetComponent<CreateMesh>().plotID)
            {
                return providedPlotID;
            }
        }
        return -1;
    }

    public void Update()
    {
        // This shouldn't be here
        RenderPlots();
    }

    void RenderPlots()
    {
        // Call render on each plot element in local array of plots
        foreach (var plot in plots)
        {
            plot.GetComponent<MeshHandler>().Render();
        }
    }
}
