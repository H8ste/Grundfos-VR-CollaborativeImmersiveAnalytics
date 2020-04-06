using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour
{
  private List<System.String>[] data;
  private string[] headers;

  void Start()
  {
    LoadData();
  }

  private void LoadData()
  {
    TextAsset loadedDataset = Resources.Load<TextAsset>("env/src/DataSet1");

    // Spliting dataset and filling the rows array
    string[] rows = loadedDataset.text.Split(new char[] { '\n' });
    data = new List<System.String>[rows[0].Split(new char[] { ',' }).Length];

    for (int i = 0; i < data.Length; i++)
    {
      data[i] = new List<System.String>();
    }

    // Save first element as header
    headers = rows[0].Split(new char[] { ',' });

    // Going through each data set input and skipping the first one as it is the name for the attribute
    for (int i = 1; i < rows.Length - 1; i++)
    {
      // Splitting comma separated dataset into columns
      string[] columns = rows[i].Split(new char[] { ',' });
      // Go through each column and add entry
      for (int k = 0; k < columns.Length; k++)
      {
        data[k].Add(columns[k]);
      }
    }
  }

  public List<System.String>[] GetData()
  {
    if (data == null)
      LoadData();
    return data;
  }

  public string[] GetHeaders()
  {
    if (headers == null)
      LoadData();
    return headers;
  }
}
