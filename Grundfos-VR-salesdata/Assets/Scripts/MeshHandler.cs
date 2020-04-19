using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit;

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


public class PlotOptions
{
  private float plotLength, plotHeight;
  public float PlotLength { get { return plotLength; } set { plotLength = value; } }
  public float PlotHeight { get { return plotHeight; } set { plotHeight = value; } }


  GameObject[] plotLabels;

  public GameObject[] PlotLabels { get { return plotLabels; } set { plotLabels = value; } }
  // GameObject xLabel; GameObject yLabel;
  // public GameObject XLabel { get { return xLabel; } set { xLabel = value; } }
  // public GameObject YLabel { get { return yLabel; } set { yLabel = value; } }

  Color32[] featureColors;
  public Color32[] FeatureColors { get { return featureColors; } set { featureColors = value; } }


  TypeOfPlot plotType;
  public TypeOfPlot PlotType { get { return plotType; } set { plotType = value; } }

  public PlotOptions()
  {
    this.plotLength = 5f; this.plotHeight = 5f;
  }
}

public class Plot
{
  PlotOptions plotOptions;
  public PlotOptions PlotOptions { get { return plotOptions; } set { plotOptions = value; } }

  List<System.String>[] data;
  public List<System.String>[] Data { get { return data; } set { data = value; } }

  List<System.String>[] dataCompared;
  public List<System.String>[] DataCompared { get { return dataCompared; } set { dataCompared = value; } }

  string[] dataHeaders;
  public string[] DataHeaders { get { return dataHeaders; } set { dataHeaders = value; } }

  List<string> dataComparedHeaders = new List<string>();
  public List<string> DataComparedHeaders { get { return dataComparedHeaders; } set { dataComparedHeaders = value; } }

  float[] dataAverages;
  public float[] DataAverages { get { return dataAverages; } set { dataAverages = value; } }

  Mesh mesh; Vector3[] vertices; int[] triangles;
  public Mesh Mesh { get { return mesh; } set { mesh = value; } }
  public Vector3[] Vertices { get { return vertices; } set { vertices = value; } }
  public int[] Triangles { get { return triangles; } set { triangles = value; } }

  private int featureOneIndex, featureTwoIndex;
  public int FeatureOneIndex { get { return featureOneIndex; } set { featureOneIndex = value; } }
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

  Plot plot;


  // should only run content if anything is changed I think
  public void Render()
  {
    plot.Mesh.Clear();
    plot.Mesh.vertices = plot.Vertices;
    plot.Mesh.triangles = plot.Triangles;

    Color32[] colors = new Color32[plot.Vertices.Length];

    for (int i = 0; i < colors.Length; i++)
    {
      int colorIndex = (int)(((1 - (i / (plot.Vertices.Length * 1f))) * plot.DataCompared.Length) - 0.25);
      colors[i] = plot.PlotOptions.FeatureColors[colorIndex];
    }
    plot.Mesh.colors32 = colors;

    plot.Mesh.RecalculateNormals();
  }

  public void CreateNewPlot(int _featureOneIndex, int _featureTwoIndex, DataReader dataReaderRef, TypeOfPlot plotType)
  {
    plot = new Plot();
    plot.FeatureOneIndex = _featureOneIndex; plot.FeatureTwoIndex = _featureTwoIndex;
    dataReader = dataReaderRef;

    plot.Mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = plot.Mesh;

    ComputeMesh();

    InitialiseMeshColors();

    SetupAxisLabels();

    plot.Mesh.RecalculateNormals();
  }

  private void SetupAxisLabels()
  {
    if (plot.DataHeaders == null)
    {
      plot.DataHeaders = dataReader.GetHeaders();
    }
    switch (plot.PlotOptions.PlotType)
    {
      case TypeOfPlot.Barchart:
        plot.PlotOptions.PlotLabels = new GameObject[2];
        plot.PlotOptions.PlotLabels[0] = new GameObject("xLabel");
        plot.PlotOptions.PlotLabels[1] = new GameObject("xLabel");
        int axisCount = 0;
        foreach (var label in plot.PlotOptions.PlotLabels)
        {
          label.transform.SetParent(gameObject.GetComponentInChildren<Canvas>().transform);
          label.transform.localScale = new Vector3(1f, 1f, 1f);
          Text labelText = label.AddComponent<Text>();
          switch (axisCount)
          {
            // X
            case 0:
              labelText.text = plot.DataHeaders[plot.FeatureOneIndex];
              labelText.transform.localPosition = new Vector3(230f, 0f, -0.2f);
              labelText.alignment = TextAnchor.MiddleLeft;
              break;
            // Y
            case 1:
              labelText.text = plot.DataHeaders[plot.FeatureTwoIndex];
              labelText.transform.localPosition = new Vector3(0f, 180f, -0.2f);
              labelText.alignment = TextAnchor.MiddleCenter;
              break;
          }
          labelText.fontSize = 26;
          labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

          axisCount++;
        }
        break;
        // default:
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

    ReComputeColliders();
  }

  private void InitialiseMeshColors()
  {
    plot.PlotOptions.FeatureColors = new Color32[plot.DataCompared.Length];
    for (int i = 0; i < plot.PlotOptions.FeatureColors.Length; i++)
    {
      // gives random value, should use colors in user-selected color array
      plot.PlotOptions.FeatureColors[i] = new Color32((byte)(int)Random.Range(0, 255f), (byte)(int)Random.Range(0, 255f), (byte)(int)Random.Range(0, 255f), 255);
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
    for (int i = 0; i < comparison.Length; i++)
    {
      comparison[i] = seenBefore[i].content;
      plot.DataComparedHeaders.Add(seenBefore[i].header);
    }

    return comparison;
  }

  // Returns found vertices for data input
  private Vector3[] CreateBarchart(List<System.String>[] input)
  {
    spacing = plot.PlotOptions.PlotLength / input.Length;
    Vector3[] foundVertices = new Vector3[input.Length * 4];

    // If new features or dataset has changed, recalculate Averages
    // if (dataChanged)
    // {
    plot.DataAverages = FindMaxAvg(input);
    // dataChanged = false;
    // }

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
    // TODO: check  if input.length is different form plot.Datacompared.length
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
      Debug.Log("Position in array of bars: " + index + ". Avg: " + plot.DataAverages[index + 1].ToString());
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
    if (gameObject.GetComponent<MeshCollider>())
    {
      collider = gameObject.GetComponent<MeshCollider>();
    }
    else
    {
      collider = gameObject.AddComponent<MeshCollider>();
    }
    collider.convex = true;
    collider.convex = false;
    collider.sharedMesh = GetComponent<MeshFilter>().mesh;
    // meshColiderBool = true;
    GetComponent<XRScaleInteractable>().colliders.Clear();
    GetComponent<XRScaleInteractable>().colliders.Add(collider);
    GetComponent<XRScaleInteractable>().BigHack();
  }

}
