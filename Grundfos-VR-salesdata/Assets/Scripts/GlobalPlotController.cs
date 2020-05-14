using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.UI;

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

    // public GameObject plotPrefab;
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
    public void SendPlot(int _FeatureOneIndex, int _FeatureTwoIndex, float[] _XThresholds, float[] _YThresholds, Vector3 scale)
    {

        //instantiate new plotprefab
        plots.Add(Instantiate(debugInstance.plotPrefab, Vector3.zero, Quaternion.identity));
        // plots.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlotPrefab"), Vector3.zero, Quaternion.identity));
        plots[plots.Count - 1].transform.SetParent(transform, false);
        // call meshsendplot on it
        plots[plots.Count - 1].GetComponentInChildren<MeshHandler>().MeshSendPlot(_FeatureOneIndex, _FeatureTwoIndex, _XThresholds, _YThresholds, scale);
        plots[plots.Count - 1].GetComponentInChildren<MeshHandler>().plot.PlotID = plots.Count - 1;
        Debug.Log("RECEIVER plots added: " + plots.Count);
        Debug.Log(CurrRefMesh + " = curr reff mesh");
        Debug.Log(_FeatureOneIndex + "feature one index");
        Debug.Log(_FeatureTwoIndex + " =  feature two index");
        Debug.Log(_XThresholds + " thresholds X");
        Debug.Log(_YThresholds + " thresholds y");
        // if (CurrRefMesh != null)
        // {
        //     CurrRefMesh.GetComponent<MeshHandler>().MeshSendPlot(_FeatureOneIndex, _FeatureTwoIndex, _XThresholds, _YThresholds);

        // }

    }

    public void AddPlot(GameObject plotToAdd)
    {

        // if (!GetComponent<PhotonView>().IsMine)
        // {
        //     GetComponent<PhotonView>().RequestOwnership();
        // }
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
        Debug.Log(plots[plots.Count - 1]);
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
            PhotonView.Get(this).RPC("SendPlot", RpcTarget.OthersBuffered, CurrRefMesh.plot.FeatureOneIndex, CurrRefMesh.plot.FeatureTwoIndex, CurrRefMesh.plot.PlotOptions.XThresholds, CurrRefMesh.plot.PlotOptions.YThresholds, CurrRefMesh.transform.localScale);

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


    public void updatePosition(int id, Vector3 newPos)
    {
        PhotonView.Get(this).RPC("updatePositionRPC", RpcTarget.OthersBuffered, id, newPos);
    }

    public void updateScale(int id, Vector3 newScale)
    {
        PhotonView.Get(this).RPC("updateScaleRPC", RpcTarget.OthersBuffered, id, newScale);
    }

    [PunRPC]
    void updatePositionRPC(int id, Vector3 newPos)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
            foundPlot.transform.position = newPos;
    }

    [PunRPC]
    void updateScaleRPC(int id, Vector3 newScale)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
            foundPlot.transform.localScale = newScale;
    }

    public void DeletePlot(int id)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
            Destroy(foundPlot);
        PhotonView.Get(this).RPC("DeletePlotRPC", RpcTarget.OthersBuffered, id);
    }

    [PunRPC]
    public void DeletePlotRPC(int id)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
            Destroy(foundPlot);

        RenderPlots();
    }

    GameObject FindPlot(int id)
    {
        foreach (var plot in plots)
        {
            if (plot.GetComponent<MeshHandler>().plot.PlotID == id)
            {
                return plot;
            }
        }
        return null;
    }

    public GameObject BarValueTextPrefab = null;

    public GameObject SpawnBarValue(int id, int handIndex, string barValue, Vector3 barvaluePosition, Transform parentTransform)
    {
        GameObject spawnedBarValue = Instantiate(BarValueTextPrefab, barvaluePosition, Quaternion.identity);
        Debug.Log("SENDER Spawned barValueTextPrefab");
        spawnedBarValue.transform.SetParent(parentTransform);
        spawnedBarValue.transform.localScale = Vector3.one;

        PhotonView.Get(this).RPC("SpawnBarValueRPC", RpcTarget.OthersBuffered, id, handIndex, barValue, barvaluePosition);

        return spawnedBarValue;
    }

    [PunRPC]
    public void SpawnBarValueRPC(int id, int handIndex, string barValue, Vector3 barvaluePosition)
    {
        //remember to set text holder with spawned text 
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
        {
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex] = Instantiate(BarValueTextPrefab, barvaluePosition, Quaternion.identity);
            Debug.Log("RECIEVER Spawned barValueTextPrefab");
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex].transform.SetParent(foundPlot.GetComponent<HandlePoints>().transform.GetChild(0).transform, false);
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex].transform.localScale = Vector3.one;
        }
    }

    public void SetBarValueText(int id, int handIndex, string newBarValue, GameObject textToChange, Vector3 worldPos)
    {
        textToChange.GetComponent<Text>().text = newBarValue;
        textToChange.transform.position = worldPos;

        //rpc function
        PhotonView.Get(this).RPC("SetBarValueTextRPC", RpcTarget.OthersBuffered, id, handIndex, newBarValue, worldPos);
    }

    [PunRPC]
    public void SetBarValueTextRPC(int id, int handIndex, string newBarValue, Vector3 worldPos)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
        {
            if (foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder != null)
            {
                // Debug.Log("It could fine the temporary text holder. Length: " + foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder.Length);
                if (foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[0] == null)
                {

                    // Debug.Log("Could not find the index 0 ");
                }
                if (foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[1] == null)
                {
                    // Debug.Log("Could not find the index 1 ");
                }
            }
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex].GetComponent<Text>().text = newBarValue;
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex].GetComponent<Text>().transform.position = worldPos;
        }
    }


    public void RemoveBarValue(int id, int handIndex)
    {
        PhotonView.Get(this).RPC("RemoveBarValueRPC", RpcTarget.OthersBuffered, id, handIndex);
    }

    [PunRPC]
    public void RemoveBarValueRPC(int id, int handIndex)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
        {
            Destroy(foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex]);
            foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex] = null;
            //    Destroy(temporaryTextHolder[(int)handside]);
            // temporaryTextHolder[(int)handside] = null;
        }

    }

    public void SaveBar(int id, int handIndex, int barIndex)
    {
        PhotonView.Get(this).RPC("SaveBarRPC", RpcTarget.OthersBuffered, id, handIndex, barIndex);
    }


    [PunRPC]
    public void SaveBarRPC(int id, int handIndex, int barIndex)
    {
        GameObject foundPlot = FindPlot(id);
        if (foundPlot)
        {
            Debug.Log("RECIEVER Saving: " + id + ", " + handIndex + ", " + barIndex);
            if (foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts == null)
            {
                foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts = new GameObject[foundPlot.GetComponent<MeshHandler>().plot.DataCompared.Length];
            }
            if (foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts[barIndex] == null)
            {
                foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts = new GameObject[foundPlot.GetComponent<MeshHandler>().plot.DataCompared.Length];
            }
            if (foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts.Length != foundPlot.GetComponent<MeshHandler>().plot.DataCompared.Length)
            {
                // should really do some smart stuff here but out of time
                foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts = new GameObject[foundPlot.GetComponent<MeshHandler>().plot.DataCompared.Length];
            }

            if (foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts[barIndex])
            {
                Destroy(foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts[barIndex]);
            }

            foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts[barIndex] = Instantiate(foundPlot.GetComponent<HandlePoints>().TemporaryTextHolder[handIndex]);
            foundPlot.GetComponent<HandlePoints>().SavedSpawnedTexts[barIndex].transform.SetParent(foundPlot.GetComponent<HandlePoints>().transform.GetChild(0).transform, false);
        }
    }


}
