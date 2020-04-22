using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public enum TypeOfPlot { Barchart };

// Rename to comparedRow
public class ComparedRow
{
  public string header;
  public List<string> content;

  public ComparedRow(string _header, string firstContent)
  {
    this.header = _header;

    this.content = new List<string>();
    this.content.Add(firstContent);
  }
}

[Serializable]
public class PlotOptions
{
  private float plotLength, plotHeight;
  [HideInInspector]
  public float PlotLength { get { return plotLength; } set { plotLength = value; } }
  [HideInInspector]
  public float PlotHeight { get { return plotHeight; } set { plotHeight = value; } }


  Text[] plotNotations;
  [HideInInspector]
  public Text[] PlotNotations { get { return plotNotations; } set { plotNotations = value; } }


  List<GameObject> plotXLabels = new List<GameObject>();
  [HideInInspector]
  public List<GameObject> PlotXLabels { get { return plotXLabels; } set { plotXLabels = value; } }

  List<GameObject> plotYLabels = new List<GameObject>();
  [HideInInspector]
  public List<GameObject> PlotYLabels { get { return plotYLabels; } set { plotYLabels = value; } }

  [SerializeField]
  Color32[] featureColors;
  [HideInInspector]
  public Color32[] FeatureColors { get { return featureColors; } set { featureColors = value; } }


  TypeOfPlot plotType;
  [HideInInspector]
  public TypeOfPlot PlotType { get { return plotType; } set { plotType = value; } }

  string[] specifiedOrder = null;
  [HideInInspector]
  public string[] SpecifiedOrder { get { return specifiedOrder; } set { specifiedOrder = value; } }

  public PlotOptions()
  {
    this.plotLength = 5f; this.plotHeight = 5f;
  }
}

[Serializable]
public class Plot
{
  [SerializeField]
  PlotOptions plotOptions;
  [HideInInspector]
  public PlotOptions PlotOptions { get { return plotOptions; } set { plotOptions = value; } }

  List<System.String>[] data;
  [HideInInspector]
  public List<System.String>[] Data { get { return data; } set { data = value; } }

  List<System.String>[] dataCompared;
  [HideInInspector]
  public List<System.String>[] DataCompared { get { return dataCompared; } set { dataCompared = value; } }

  string[] dataHeaders;
  [HideInInspector]
  public string[] DataHeaders { get { return dataHeaders; } set { dataHeaders = value; } }

  List<string> dataComparedHeaders = new List<string>();
  [HideInInspector]
  public List<string> DataComparedHeaders { get { return dataComparedHeaders; } set { dataComparedHeaders = value; } }

  float[] dataAverages;
  [HideInInspector]
  public float[] DataAverages { get { return dataAverages; } set { dataAverages = value; } }

  Mesh mesh; Vector3[] vertices; int[] triangles;
  [HideInInspector]
  public Mesh Mesh { get { return mesh; } set { mesh = value; } }
  [HideInInspector]
  public Vector3[] Vertices { get { return vertices; } set { vertices = value; } }
  [HideInInspector]
  public int[] Triangles { get { return triangles; } set { triangles = value; } }

  private int featureOneIndex, featureTwoIndex;
  [HideInInspector]
  public int FeatureOneIndex { get { return featureOneIndex; } set { featureOneIndex = value; } }
  [HideInInspector]
  public int FeatureTwoIndex { get { return featureTwoIndex; } set { featureTwoIndex = value; } }

  public Plot()
  {
    plotOptions = new PlotOptions();
  }
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshHandler : MonoBehaviour
{

  DataReader dataReader;
  float spacing;

  [SerializeField]
  public Plot plot;

  private int[] sortedOrder;


  [SerializeField] bool plotOptionsChanged = false;
  bool plotMeshChanged = false;

  public GameObject plotLabelPrefab;


  // should only run content if anything is changed I think
  public void Render()
  {

    if (plotMeshChanged)
    {
      plotMeshChanged = false;

      plot.Mesh.Clear();
      plot.Mesh.vertices = plot.Vertices;
      plot.Mesh.triangles = plot.Triangles;

      plot.Mesh.RecalculateNormals();

      GetComponent<MeshFilter>().mesh = plot.Mesh;

      ReComputeColliders();
    }

    if (plotOptionsChanged)
    {
      plotOptionsChanged = false;

      Color32[] colors = new Color32[plot.Vertices.Length];

      for (int i = 0; i < colors.Length; i++)
      {
        int colorIndex = (int)(((1 - (i / (plot.Vertices.Length * 1f))) * plot.DataCompared.Length) - 0.25);
        colors[i] = plot.PlotOptions.FeatureColors[colorIndex];
      }
      plot.Mesh.colors32 = colors;

      plot.Mesh.RecalculateNormals();

      GetComponent<MeshFilter>().mesh = plot.Mesh;
    }

  }

  public void CreateNewPlot(int _featureOneIndex, int _featureTwoIndex, DataReader dataReaderRef, TypeOfPlot plotType)
  {
    plot = new Plot();

    plot.FeatureOneIndex = _featureOneIndex; plot.FeatureTwoIndex = _featureTwoIndex;
    dataReader = dataReaderRef;

    plot.Mesh = new Mesh();

    ComputeMesh();

    InitialiseMeshColors();

    SetupAxisNotation();

    SetupAxisLabels();

    plot.Mesh.RecalculateNormals();

    plotOptionsChanged = true;
    plotMeshChanged = true;
    Render();
  }

  private void SetupAxisNotation()
  {
    if (plot.DataHeaders == null)
    {
      plot.DataHeaders = dataReader.GetHeaders();
    }
    switch (plot.PlotOptions.PlotType)
    {
      case TypeOfPlot.Barchart:
        plot.PlotOptions.PlotNotations = transform.GetChild(0).GetComponentsInChildren<Text>();
        // plot.PlotOptions.PlotNotations[0] = new GameObject("xLabel");
        // plot.PlotOptions.PlotNotations[1] = new GameObject("yLabel");
        int axisCount = 0;
        foreach (var notation in plot.PlotOptions.PlotNotations)
        {
          // label.transform.SetParent(gameObject.GetComponentInChildren<Canvas>().transform);
          notation.transform.localScale = new Vector3(1f, 1f, 1f);
          notation.transform.eulerAngles = new Vector3(0f, 0f, 0f);
          // Text labelText = label.AddComponent<Text>();
          notation.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 40);
          // label.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0.5f);

          switch (axisCount)
          {
            // X
            case 0:
              notation.text = plot.DataHeaders[plot.FeatureOneIndex];
              // label.transform.localPosition = new Vector3(380f, 0f, -0.2f);
              notation.alignment = TextAnchor.MiddleLeft;
              break;
            // Y
            case 1:
              notation.text = plot.DataHeaders[plot.FeatureTwoIndex];
              // label.transform.localPosition = new Vector3(0f, 200f, -0.2f);
              notation.alignment = TextAnchor.MiddleCenter;
              break;
          }
          notation.fontSize = 26;
          notation.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

          axisCount++;
        }
        break;
        // default:
    }
  }

  private void SetupAxisLabels()
  {
    if (plot.PlotOptions.PlotXLabels.Count > 0)
      plot.PlotOptions.PlotXLabels.Clear();
    if (plot.PlotOptions.PlotYLabels.Count > 0)
      plot.PlotOptions.PlotYLabels.Clear();

    // X
    for (int i = 0; i < plot.DataCompared.Count(); i++)
    {
      float actualSpacing = 176f / plot.DataCompared.Count();
      GameObject temp = Instantiate(plotLabelPrefab,
      new Vector3(i * actualSpacing + (actualSpacing / 2), -7f, -0.06f),
      Quaternion.Euler(0f, 0f, -35f));
      temp.transform.SetParent(transform.GetChild(0).GetChild(0), false);
      plot.PlotOptions.PlotXLabels.Add(temp);
      plot.PlotOptions.PlotXLabels[i].GetComponent<Text>().text = plot.DataComparedHeaders[i];
      plot.PlotOptions.PlotXLabels[i].transform.localScale = (1f / (float)plot.DataCompared.Count()) * new Vector3(1f, 1f, 1f);
    }

    // y
    for (int i = 0; i < 11; i++)
    {
      float actualSpacing = 176f / 10;
      GameObject temp = Instantiate(plotLabelPrefab,
      new Vector3(0f, i * actualSpacing, -0.06f),
      Quaternion.Euler(0f, 0f, -35f));
      temp.transform.SetParent(transform.GetChild(0).GetChild(0), false);
      plot.PlotOptions.PlotYLabels.Add(temp);
      plot.PlotOptions.PlotYLabels[i].GetComponent<Text>().text = Remap(i * actualSpacing, 0, 176f, 0, plot.DataAverages[0]).ToString();
      plot.PlotOptions.PlotYLabels[i].transform.localScale = (1f / 10f) * new Vector3(1f, 1f, 1f);
    }
  }

  private void ComputeMesh()
  {
    if (plot.DataCompared == null)
    {
      plot.Data = dataReader.GetData();
      plot.DataCompared = Compare(plot.Data[plot.FeatureOneIndex], plot.Data[plot.FeatureTwoIndex]);
    }

    plot.Vertices = CreateChartOfType(plot.PlotOptions.PlotType);
    plot.Triangles = FindTriangles(plot.Vertices);
  }

  private void InitialiseMeshColors()
  {
    plot.PlotOptions.FeatureColors = new Color32[plot.DataCompared.Length];
    for (int i = 0; i < plot.PlotOptions.FeatureColors.Length; i++)
    {
      // gives random value, should use colors in user-selected color array
      plot.PlotOptions.FeatureColors[i] = new Color32((byte)(int)UnityEngine.Random.Range(0, 255f), (byte)(int)UnityEngine.Random.Range(0, 255f), (byte)(int)UnityEngine.Random.Range(0, 255f), 255);
    }
  }

  private Vector3[] CreateChartOfType(TypeOfPlot plotType)
  {
    switch (plotType)
    {
      case TypeOfPlot.Barchart:
        return CreateBarchart(plot.DataCompared);

        // more cases here
    }

    return null;
  }

  List<System.String>[] Compare(List<System.String> first, List<System.String> second)
  {
    List<ComparedRow> seenBefore = new List<ComparedRow>();
    // For each item in first feature
    for (int i = 0; i < first.Count; i++)
    {
      // Check if entry is seen before
      bool isUnique = true;
      do
      {
        for (int k = 0; k < seenBefore.Count; k++)
        {
          if (first[i] == seenBefore[k].header)
          {
            // Is not unique
            isUnique = false;
            // Insert entry of second feature into list of appropiate object of seenBefore
            seenBefore[k].content.Add(second[i]);
            break;
          }
        }
        break;
      } while (isUnique);
      // Is unique
      if (isUnique)
        seenBefore.Add(new ComparedRow(first[i], second[i]));
    }

    List<System.String>[] comparison = new List<System.String>[seenBefore.Count];
    plot.DataComparedHeaders = new List<string>();

    if (plot.PlotOptions.SpecifiedOrder == null)
    {
      sortedOrder = findSortedOrder(seenBefore);

      int i = 0;
      foreach (var index in sortedOrder)
      {
        comparison[i] = seenBefore[index].content;
        plot.DataComparedHeaders.Add(seenBefore[index].header);
        i++;
      }

      // foreach (var header in plot.DataComparedHeaders)
      // {
      //   Debug.Log("header: " + header);
      // }

    }
    else
    {
      // TODO: Add code to use mapping provided from user instead of return value from findSortedOrder
    }

    return comparison;
  }

  private int[] findSortedOrder(List<ComparedRow> input)
  {
    string[] arrayToSort = new string[input.Count];

    // Fill an int array : [0, 1, 3, ..., length]
    int[] mapping = Enumerable.Range(0, arrayToSort.Length).Select(i => (int)i).ToArray();

    for (int i = 0; i < input.Count; i++)
    {
      arrayToSort[i] = input[i].header;
    }
    string temp;
    int tempMapping;
    // Loops through array from back and from the front, sorting it by eather greatest number or greatest string value (alphabetically)
    for (int i = 1; i < arrayToSort.Length; i++)
    {
      for (int j = 0; j < arrayToSort.Length - 1; j++)
      {
        if (float.TryParse(arrayToSort[j], out _))
        {
          // Numbers
          if (float.Parse(arrayToSort[j]) > float.Parse(arrayToSort[j + 1]))
          {
            temp = arrayToSort[j];
            arrayToSort[j] = arrayToSort[j + 1];
            arrayToSort[j + 1] = temp;

            tempMapping = mapping[j];
            mapping[j] = mapping[j + 1];
            mapping[j + 1] = tempMapping;
          }
        }
        else
        {
          // Letters
          if (arrayToSort[j][0] > arrayToSort[j + 1][0])
          {
            temp = arrayToSort[j];
            arrayToSort[j] = arrayToSort[j + 1];
            arrayToSort[j + 1] = temp;

            tempMapping = mapping[j];
            mapping[j] = mapping[j + 1];
            mapping[j + 1] = tempMapping;
          }
        }
      }
    }

    return mapping;
  }

  // Returns found vertices for data input
  private Vector3[] CreateBarchart(List<System.String>[] input)
  {
    spacing = plot.PlotOptions.PlotLength / input.Length;
    Vector3[] foundVertices = new Vector3[input.Length * 4];

    plot.DataAverages = FindMaxAvg(input);

    for (int i = 0; i < input.GetLength(0); i++)
    {
      // Compute the 4 vertices
      foundVertices[i * 4 + 0] = new Vector3(i * spacing, 0, 0); //1
      foundVertices[i * 4 + 1] = new Vector3(i * spacing, Remap(plot.DataAverages[i + 1], 0, plot.DataAverages[0], 0, plot.PlotOptions.PlotHeight), 0); //2
      foundVertices[i * 4 + 2] = new Vector3(i * spacing + spacing, Remap(plot.DataAverages[i + 1], 0, plot.DataAverages[0], 0, plot.PlotOptions.PlotHeight), 0); //3
      foundVertices[i * 4 + 3] = new Vector3(i * spacing + spacing, 0, 0); //4
    }
    return foundVertices;
  }

  // From input vertices, finds appropiate triangles
  private int[] FindTriangles(Vector3[] input)
  {
    List<int> returnList = new List<int>();
    int k = 0;

    for (int i = 0; i < plot.DataCompared.Length; i++)
    {
      returnList.Add(k);//first
      returnList.Add(k + 1);//second
      returnList.Add(k + 3);//third

      returnList.Add(k + 1);//first
      returnList.Add(k + 2);//second
      returnList.Add(k + 3);//third
      k = k + 4;
    }

    return returnList.ToArray();
  }

  private float Remap(float value, float from1, float to1, float from2, float to2)
  {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
  }

  // [maxAvg, featureAvg1,featureAvg2,featureAvg3...]
  private float[] FindMaxAvg(List<System.String>[] input)
  {
    float[] returnArr = new float[input.Length + 1];
    for (int i = 0; i < input.Length; i++)
    {
      returnArr[i + 1] = FindAvg(input[i]);
      if (returnArr[i + 1] > returnArr[0])
        returnArr[0] = returnArr[i + 1];
    }
    return returnArr;
  }

  private float FindAvg(List<System.String> input)
  {
    float avgVal = 0;
    int lengthDeduct = 0;

    // Checks if input is ordinal -> Yes: Return occurence    No: Return Average
    if (float.TryParse(input[0], out _))
    {
      foreach (var number in input)
      {
        float parsedNumber = float.Parse(number);
        if (parsedNumber == 0)
        {
          lengthDeduct++;
        }
        else
        {
          avgVal += parsedNumber;
        }
      }
      if (((float)input.Count - lengthDeduct) != 0)
      {
        avgVal = avgVal / ((float)input.Count - lengthDeduct);
      }
      else
      {
        avgVal = 0;
      }

      if (avgVal != avgVal)
      {
        Debug.Log("NAN FLOAT: " + avgVal + " / " + ((float)input.Count - lengthDeduct));
      }
    }
    else
    {
      // Return occurence
      avgVal = input.Count;

      if (avgVal != avgVal)
      {
        Debug.Log("NAN STRING: " + input.Count);
      }
    }
    return avgVal;
  }

  public int GetIndexByPos(Vector3 hitPosInMesh)
  {
    if (plot.DataHeaders != null)
    {
      float xPosMouse = hitPosInMesh.x;
      int index = (int)(((xPosMouse / (plot.PlotOptions.PlotLength * transform.localScale.x)) * plot.DataComparedHeaders.Count));
      // Debug.Log("Position in array of bars: " + index + ". Avg: " + plot.DataAverages[index + 1].ToString());
      return index;
    }
    return -1;
  }

  public Vector3 getTextPos(int index)
  {
    Vector3 vectTopLeftCorner = new Vector3(
      index * spacing,
      Remap(plot.DataAverages[index + 1], 0, plot.DataAverages[0], 0, plot.PlotOptions.PlotHeight),
      0);
    Vector3 vectTopRightCorner = new Vector3(
      index * spacing + spacing,
      Remap(plot.DataAverages[index + 1], 0, plot.DataAverages[0], 0, plot.PlotOptions.PlotHeight),
      0);
    // This does nothing but return vectTopLeftCorner right now
    return vectTopRightCorner + (vectTopLeftCorner - vectTopRightCorner);
  }

  public float[] GetDataAverages()
  {
    return plot.DataAverages;
  }

  public void Update()
  {

  }

  public void ReComputeColliders()
  {
    //Redo collider
    MeshCollider collider;
    if (GetComponent<MeshCollider>())
    {
      Destroy(GetComponent<MeshCollider>());
    }
    collider = gameObject.AddComponent<MeshCollider>();

    collider.sharedMesh = GetComponent<MeshFilter>().mesh;
    collider.convex = false;
    // collider.sharedMaterial =h;
    // collider.convex = false;

    // meshColiderBool = true;
    GetComponent<XRScaleInteractable>().colliders.Clear();
    GetComponent<XRScaleInteractable>().colliders.Add(collider);
    GetComponent<XRScaleInteractable>().BigHack();
  }

}
