using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
  public GameObject ScrollPrefab;

  private GameObject plot;
  private GameObject featureMenu;

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
    Debug.Log("changed feature: " + featureNumber + ", to: " + featureID);
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
      // Create(featuresChosen[0], featuresChosen[1], dataReader);
      plot.transform.SetParent(transform);
      plot.transform.localPosition = new Vector3(-2.69f, -2.58f, 0.005f);
      plot.transform.localScale = new Vector3(1f, 1f, 1f);
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

  public void confirmFeatureSelection(int feature, int featureSelected)
  {
    if (plot)
    {
      plot.SetActive(true);
    }
    if (featureSelected != -1)
    {
      setFeature(featureBeingChanged, featureSelected);
      Object.Destroy(featureMenu);
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




}
