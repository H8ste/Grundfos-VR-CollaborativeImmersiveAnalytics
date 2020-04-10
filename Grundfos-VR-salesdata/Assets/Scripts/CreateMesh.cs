using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.UI;

public class Exist
{
    public string header;
    public List<string> content;




    public Exist(string _header, string firstContent)
    {
        this.header = _header;

        this.content = new List<string>();
        this.content.Add(firstContent);
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CreateMesh : MonoBehaviour
{
  public int plotID { get; private set; }
  Mesh mesh;
  Vector3[] vertices;
  int[] triangles;
  List<System.String>[] data;
  GameObject label1;
  GameObject label2;

    public float[] dataAverages { get; set; }
    public bool dataChanged = true;

    private List<System.String>[] dataCompared;

    public float plotLength = 5f, plotHeight = 5f;
    private float LplotLength = 5f, LplotHeight = 5f;

    public Color32 plotColor;
    private Color32 LplotColor;

    public Color32[] featureColors;

    private int featureOne, featureTwo;

    private DataReader dataReader;
    bool meshColiderBool;

    public float spacing;

    private List<string> featureTypeList = new List<string>();
    private Vector3 previousMousePos;


    public void Create(int _featureOne, int _featureTwo, DataReader _dataReader)
    {
        featureOne = _featureOne; featureTwo = _featureTwo; dataReader = _dataReader;
    string strFtr1 = dataReader.GetHeaders()[featureOne];
    string strFtr2 = dataReader.GetHeaders()[featureTwo];

    mesh = new Mesh(); GetComponent<MeshFilter>().mesh = mesh;
    UpdateMesh();

    label1 = new GameObject("myLabel1");
    label1.transform.SetParent(this.transform);
 
    Text myText = label1.AddComponent<Text>();
    myText.text = "LLLLLLLLLLLLEEEEEEEEEEEEEEEEEEEEETTTTTTTTTTTTTTTTTTT MMMMMMMMMMMMMMMMMEEEEEEEEEEEEEEEEEEEEE CCCCCCCCCCCCRRRRRRRRRRRRYYYYYYYYYYYYYYYYYYYY";
    myText.fontSize = 26;


        data = dataReader.GetData();
        dataCompared = Compare(data[featureOne], data[featureTwo]);

        mesh = new Mesh(); GetComponent<MeshFilter>().mesh = mesh;
        UpdateMesh();
        mesh.RecalculateNormals();
        meshColiderBool = false;

    }

    void Update()
    {
        Render();

        if (!meshColiderBool)
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.convex = false;
            GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
            Debug.Log("Mesh Collider Created");
            Debug.Log(collider.bounds);
            meshColiderBool = true;
        }


    }

    public void UpdateMesh()
    {
        if (dataCompared == null)
        {
            data = dataReader.GetData();
            dataCompared = Compare(data[featureOne], data[featureTwo]);

        }
        vertices = CreateBarchart(dataCompared);
        triangles = FindTriangles(vertices);
        featureColors = new Color32[dataCompared.Length];
        for (int i = 0; i < featureColors.Length; i++)
        {
            featureColors[i] = new Color32((byte)(int)Random.Range(0, 255f), (byte)(int)Random.Range(0, 255f), (byte)(int)Random.Range(0, 255f), 255);
        }



    }

    // Recomputes the vertices, triangles, and colors for mesh.
    public void Render()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Color32[] colors = new Color32[vertices.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            int colorIndex = (int)(((1 - (i / (vertices.Length * 1f))) * dataCompared.Length) - 0.25);
            colors[i] = featureColors[colorIndex];
        }
        mesh.colors32 = colors;


    }

    // Returns found vertices for data input
    private Vector3[] CreateBarchart(List<System.String>[] input)
    {
        spacing = plotLength / input.Length;
        Vector3[] foundVertices = new Vector3[input.Length * 4];

        // If new features or dataset has changed, recalculate Averages
        if (dataChanged)
        {
            dataAverages = FindMaxAvg(input);
            dataChanged = false;
        }

        for (int i = 0; i < input.GetLength(0); i++)
        {
            // Compute the 4 vertices
            foundVertices[i * 4 + 0] = new Vector3(i * spacing, 0, 0); //1
            foundVertices[i * 4 + 1] = new Vector3(i * spacing, Remap(dataAverages[i + 1], 0, dataAverages[0], 0, plotHeight), 0); //2
            foundVertices[i * 4 + 2] = new Vector3(i * spacing + spacing, Remap(dataAverages[i + 1], 0, dataAverages[0], 0, plotHeight), 0); //3
            foundVertices[i * 4 + 3] = new Vector3(i * spacing + spacing, 0, 0); //4
        }
        return foundVertices;
    }

    // From input vertices, finds appropiate triangles
    private int[] FindTriangles(Vector3[] input)
    {
        List<int> returnList = new List<int>();
        int k = 0;
        for (int i = 0; i < dataCompared.Length; i++)
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

    List<System.String>[] Compare(List<System.String> first, List<System.String> second)
    {
        List<Exist> seenBefore = new List<Exist>();
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
                seenBefore.Add(new Exist(first[i], second[i]));
        }
        List<System.String>[] comparison = new List<System.String>[seenBefore.Count];
        featureTypeList = new List<string>();
        for (int i = 0; i < comparison.Length; i++)
        {
            comparison[i] = seenBefore[i].content;
            featureTypeList.Add(seenBefore[i].header);
            //Debug.Log(featureTypeList[i]);
        }

        return comparison;
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

    private bool IsDifferentColor(Color32 firstColor, Color32 secondColor)
    {
        bool isDifferent = false;

        if (firstColor.r != secondColor.r)
            isDifferent = true;

        if (firstColor.g != secondColor.g)
            isDifferent = true;

        if (firstColor.b != secondColor.b)
            isDifferent = true;

        if (firstColor.a != secondColor.a)
            isDifferent = true;

        return isDifferent;
    }

    public int GetIndexByPos(Vector3 mousePosInMesh)
    {



        //spacing dataAverages[] featureTypeList[] featureTypeText
        //plotLenght
        if (featureTypeList != null)
        {

            float xPosMouse = mousePosInMesh.x;
            float index = (xPosMouse / plotLength) * featureTypeList.Count;
            int index1 = (int)index;


            previousMousePos = mousePosInMesh;

            // Debug.Log(dataAverages[index1 + 1].ToString());

            return index1;


        }




        return -1;
    }

    public Vector3 getTextPos(int index)
    {


        //Debug.Log("index = " + index);


        Vector3 vectTopLeftCorner = new Vector3(index * spacing, Remap(dataAverages[index + 1], 0, dataAverages[0], 0, plotHeight), 0);
        Vector3 vectTopRightCorner = new Vector3(index * spacing + spacing, Remap(dataAverages[index + 1], 0, dataAverages[0], 0, plotHeight), 0);

        // Debug.Log("Vect Top left = " + vectTopLeftCorner);
        // Debug.Log("VectTopRight = " + vectTopRightCorner);
        //Debug.Log(((vectTopRightCorner - vectTopLeftCorner) / 2).ToString());
        return vectTopRightCorner + (vectTopLeftCorner - vectTopRightCorner);
    }


}
