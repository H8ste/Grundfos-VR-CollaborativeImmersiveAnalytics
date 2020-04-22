using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonListControl : MonoBehaviour
{
  private GameObject buttonTemplate;
  DataReader dataReader;
  string[] headers;
  private List<GameObject> schrollMenuButtonList;

  private LocalPlotController plotControllerRef;


  public int FeatureSelected { get; set; } = -1;
  public int FeatureNumber { get; set; } = 0;


  int selectedID = -1;

  private Button confirmButton;
  public void BeginControl()
  {
    Debug.Log("New instance of control isntantiated");
    confirmButton = transform.GetComponentInChildren<Button>();
    plotControllerRef = GameObject.FindObjectsOfType<LocalPlotController>()[0].GetComponent<LocalPlotController>();
    // gameObject.AddComponent<DataReader>();
    // dataReader = gameObject.GetComponent<DataReader>();
    schrollMenuButtonList = new List<GameObject>();



    // Get an array with the headers from the data.
    headers = plotControllerRef.GetDataReader().GetHeaders();
    buttonTemplate = GameObject.FindGameObjectWithTag("Button");
    buttonTemplate.SetActive(false);

    for (int k = 0; k < headers.Length; k++)
    {
      GameObject button = Instantiate(buttonTemplate) as GameObject;
      button.SetActive(true);
      // Assigns the string header to the button
      button.GetComponent<ButtonListButton>().InitializeButton(headers[k], k);
      // Makes the clone to be a parent of the ButtonListContent
      //      aka positoning it under the last created button
      button.transform.SetParent(buttonTemplate.transform.parent, false);
      //filling the button list with the schroll menu buttons
      schrollMenuButtonList.Add(button);

    }
  }
  public void Awake()
  {

  }


  public void OnClick(Button button)
  {

    ChangeButtonColor(button);
  }

  public void OnClickConfirm()
  {
    plotControllerRef.confirmFeatureSelection(FeatureNumber, FeatureSelected);
    // if (FeatureSelected != -1)
    // {
    //   //// Call setFeature on localplotcontroller
    //   GameObject.FindObjectsOfType<LocalPlotController>()[0].GetComponent<LocalPlotController>().setFeature(FeatureNumber, FeatureSelected);
    //   Object.Destroy(transform.parent.gameObject);
    //   // Allows for another menu to be spawned
    // }
    // else
    // {
    //   Object.Destroy(transform.parent.gameObject);
    // }
    // FeatureButtonScript.featureMenuSpawned = false;
  }

  public void ChangeButtonColor(Button button)
  {
    //Chanegind each button in the schrollList array to white
    for (int i = 0; i < schrollMenuButtonList.Count; i++)
    {
      schrollMenuButtonList[i].GetComponent<Button>().image.color = Color.white;
    }
    // Setting new color to the clicked button
    button.image.color = Color.green;

  }

  private void Update()
  {
    // if feature isn't selected
    if (FeatureSelected == -1)
    {
      // Disable confirm button
      confirmButton.interactable = false;
    }
    else
    {
      // Enable confirm button
      confirmButton.interactable = true;
    }
  }
}