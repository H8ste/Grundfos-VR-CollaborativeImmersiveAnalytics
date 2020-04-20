using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  private GameObject plot;

  DataReader dataReader;

  void Start()
  {
    // Debug.Log("Started localplotcontroller");
    gameObject.AddComponent<DataReader>();
    dataReader = gameObject.GetComponent<DataReader>();
    headers = dataReader.GetHeaders();

    featuresChosen.Add(-1);
    featuresChosen.Add(-1);
  }

  private void HideMenu()
  {
    // transform.GetComponentInChildren<
    gameObject.GetComponentInChildWithTag<RectTransform>("PlotCreatorINTRO").gameObject.SetActive(false);
    // transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
  }

  private void ShowMenu()
  {
    gameObject.GetComponentInChildWithTag<Transform>("PlotCreatorINTRO").gameObject.SetActive(false);
    transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
  }

  // Spawns in screen that is used to create a new plot
  public void NewPlotCreator()
  {
    // Hide Menu Screen
    HideMenu();

    // // Spawn
    // GameObject plotCreator = GameObject.Instantiate(plotCreatorPrefab);
    // plotCreator.transform.SetParent(transform.GetChild(0));
    // plotCreator.transform.localPosition = new Vector3(0, 0, 0);
    // plotCreator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    // plotCreator.transform.localEulerAngles = new Vector3(0, 0, 0);
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

      // Draw a ray pointing at our target in
      // Debug.DrawRay(transform.position, newDirection, Color.red);

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
      plot.GetComponent<CreateMesh>().Create(featuresChosen[0], featuresChosen[1], dataReader);
      plot.transform.SetParent(transform);
      plot.transform.localPosition = new Vector3(0, 0, 0);
      plot.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
      plot.transform.localEulerAngles = new Vector3(0, 0, 0);
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
