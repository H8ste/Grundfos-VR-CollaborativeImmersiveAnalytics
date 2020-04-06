using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleCreatePlot : MonoBehaviour
{
  // Start is called before the first frame update
  public GameObject myPrefab;
  public GlobalPlotController controller;
  public DataReader dataReader;

  void Start()
  {
    gameObject.AddComponent<DataReader>();
    dataReader = gameObject.GetComponent<DataReader>();
  }

  public void OnClick()
  {
    GameObject tempPlot = GameObject.Instantiate(myPrefab);
    tempPlot.GetComponent<CreateMesh>().Create((int)Random.Range(0, dataReader.GetHeaders().Length - 1), (int)Random.Range(0, dataReader.GetHeaders().Length - 1), dataReader);
    controller.AddPlot(tempPlot);
  }
}
