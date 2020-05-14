using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;


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

    private MeshHandler CurrRefMesh;

    private Transform[] spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        plots = new List<GameObject>();

        if (debugInstance.spawnPlotOnStart)
        {
            gameObject.AddComponent<DataReader>();
            debugInstance.dataReader = gameObject.GetComponent<DataReader>();

            GameObject tempPlot = GameObject.Instantiate(debugInstance.plotPrefab);
            // tempPlot.GetComponent<CreateMesh>().Create((int)Random.Range(0, debugInstance.dataReader.GetHeaders().Length - 1), (int)Random.Range(0, debugInstance.dataReader.GetHeaders().Length - 1), debugInstance.dataReader);
            tempPlot.GetComponent<MeshHandler>().CreateNewPlot(9, 7, debugInstance.dataReader, TypeOfPlot.Barchart);

            AddPlot(tempPlot);
        }

    }

    [PunRPC]
    public void SendPlot(int _FeatureOneIndex, int _FeatureTwoIndex, float[] _XThresholds, float[] _YThresholds)
    {

        Debug.Log("RECEIVER plots added: " + plots.Count);
        Debug.Log(CurrRefMesh + " = curr reff mesh");
        Debug.Log(_FeatureOneIndex + "feature one index");
        Debug.Log(_FeatureTwoIndex + " =  feature two index");
        Debug.Log(_XThresholds + " thresholds X");
        Debug.Log(_YThresholds + " thresholds y");
        if (CurrRefMesh != null)
        {
            CurrRefMesh.GetComponent<MeshHandler>().MeshSendPlot(_FeatureOneIndex, _FeatureTwoIndex, _XThresholds, _YThresholds);

        }

    }

    public void AddPlot(GameObject plotToAdd)
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            GetComponent<PhotonView>().RequestOwnership();
        }
        // Add plot provided into the local array of plots
        // if (GetComponent<PhotonView>().IsMine)
        // {

        plots.Add(plotToAdd);
        Debug.Log("SENDER plots added: " + plots.Count);

        // Change plot's position transform to be to the right of the previous element's position
        //Instante Photon it
        // GameObject photonPlotHolder = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "whateveryouwant"), Vector3.zero, Quaternion.identity);
        // photonPlotHolder.transform.SetParent(transform, false);
        // photonPlotHolder.transform.SetParent(transform, false);
        // Add plot as child to this gameobject
        // Destroy(photonPlotHolder.GetComponent<MeshHandler>());
        // photonPlotHolder.AddComponent<MeshFilter>(plots[plots.Count - 1].GetComponent<MeshFilter>());
        // photonPlotHolder.AddComponent<MeshRenderer>(plots[plots.Count - 1].GetComponent<MeshRenderer>());
        // Destroy(photonPlotHolder.GetComponent<MeshFilter>());
        // Destroy(photonPlotHolder.GetComponent<MeshRenderer>());

        // photonPlotHolder.AddComponent<MeshHandler>(plots[plots.Count - 1].GetComponent<MeshHandler>());



        // photonPlotHolder.transform.localScale = Vector3.one * 0.23f;
        // photonPlotHolder.transform.position = new Vector3(-100f, 0f, 0f);
        // photonPlotHolder.transform.eulerAngles = Vector3.zero;
        // photonPlotHolder.GetComponent<MeshHandler>().plot.PlotID = plots.Count - 1;

        // photonPlotHolder.GetComponent<MeshHandler>() = plots[plots.Count - 1].GetComponent<MeshHandler>();
        plots[plots.Count - 1].transform.SetParent(transform, false);
        plots[plots.Count - 1].transform.localScale = Vector3.one * 0.23f;
        plots[plots.Count - 1].transform.position = new Vector3(-100f, 0f, 0f);
        plots[plots.Count - 1].transform.eulerAngles = Vector3.zero;
        plots[plots.Count - 1].GetComponentInChildren<MeshHandler>().plot.PlotID = plots.Count - 1;


        PlacePlot(plots[plots.Count - 1].gameObject);
        // }
        Debug.Log("Plots count: " + plots.Count);
        CurrRefMesh = plots[plots.Count - 1].GetComponentInChildren<MeshHandler>();
        // if (GetComponent<PhotonView>().IsMine)
        {
            PhotonView.Get(this).RPC("SendPlot", RpcTarget.AllBuffered, CurrRefMesh.plot.FeatureOneIndex, CurrRefMesh.plot.FeatureTwoIndex, CurrRefMesh.plot.PlotOptions.XThresholds, CurrRefMesh.plot.PlotOptions.YThresholds);

        }





        // if (plots.Count > 1)
        // {
        //     if (plots[plots.Count - 1] != null && plots[plots.Count - 2] != null)
        //     {
        //         plots[plots.Count - 1].transform.position = plots[plots.Count - 2].transform.position + new Vector3(7f, 0, 0);
        //     }

        // }
        // else
        // {
        //     plots[0].transform.localPosition = new Vector3(-5f, 1f, 0);
        // }
    }

    public void PlacePlot(GameObject plotToPlace)
    {
        bool isPlotPlaced = false;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            bool plotAlreadyInside = false;
            for (int j = 0; j < plots.Count; j++)
            {
                // if plot is within spawnpoints area
                if (isInsideSpawnPoint(plots[j].transform.position, spawnPoints[i].transform.position, plots[j].transform.lossyScale))
                {
                    Debug.Log("A plot was already inside. Plot: " + j);
                    plotAlreadyInside = true;
                    break;
                }
            }
            if (!plotAlreadyInside)
            {
                //spawn
                plotToPlace.transform.position = spawnPoints[i].position;
                isPlotPlaced = true;
                Debug.Log("Spawned plot in spawnposition: " + i + ". " + spawnPoints[i].transform.position);
                Debug.Log("Plot's new position: " + plotToPlace.transform.position);
                //break outer-for loop
                break;
            }
        }
        if (!isPlotPlaced)
        {
            //edge case when all spawnpoints were taken
            Debug.Log("All spawnPositions were taken");
            plotToPlace.transform.position = spawnPoints[0].position;
        }

    }

    private bool isInsideSpawnPoint(Vector3 plotPosition, Vector3 spawnPosition, Vector3 scale)
    {

        float xSpacing = Mathf.Abs(spawnPoints[0].transform.position.x - spawnPoints[3].transform.position.x);
        float ySpacing = Mathf.Abs(spawnPoints[0].transform.position.y - spawnPoints[3].transform.position.y);

        if (plotPosition.x + (172 * scale.x) / 2f > spawnPosition.x - xSpacing && plotPosition.x + (172 * scale.x) / 2f < spawnPosition.x + xSpacing)
        {
            if (plotPosition.y + (172 * scale.y) / 2f > spawnPosition.y - ySpacing && plotPosition.y + (172 * scale.y) / 2f < spawnPosition.y + ySpacing)
            {
                return true;
            }
        }
        return false;
    }

    public void DeletePlot(GameObject plotToDelete)
    {
        // Using its ID, find the plot provided in the local array of plots
        int index = FindPlotInArray(plotToDelete.GetComponentInChildren<MeshHandler>().plot.PlotID, plots);
        if (index != -1)
        {
            Destroy(plots[index]);
            // plots.RemoveAt(index);
        }

        RenderPlots();
    }

    int FindPlotInArray(int providedPlotID, List<GameObject> array)
    {
        foreach (GameObject plot in array)
        {
            if (providedPlotID == plot.GetComponentInChildren<MeshHandler>().plot.PlotID)
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
        for (int plotIndex = 0; plotIndex < plots.Count; plotIndex++)
        {
            if (plots[plotIndex] != null)
            {
                plots[plotIndex].GetComponentInChildren<MeshHandler>().Render();
            }
            else
            {
                plots.RemoveAt(plotIndex);
            }
        }
    }
}
