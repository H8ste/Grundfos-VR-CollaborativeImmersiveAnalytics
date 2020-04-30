using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SliderAxis
{
    x, y
}

public class SliderRange : MonoBehaviour
{
    // Start is called before the first frame update
    Slider[] sliders;
    RectTransform[] sliderHandles;

    bool updateSliderOof = true;


    public SliderAxis sliderAxis;

    public RectTransform fill;

    private LocalPlotController localPlotRef;
    void FindSliders()
    {
        if (sliders == null)
        {
            localPlotRef = GameObject.FindObjectOfType<LocalPlotController>();
            Debug.Log("It was neccescary to find sliders");
            sliders = GetComponentsInChildren<Slider>();
            sliderHandles = new RectTransform[sliders.Length];
            sliderHandles[0] = sliders[0].transform.GetChild(0).GetComponentInChildren<RectTransform>();
            sliderHandles[1] = sliders[1].transform.GetChild(0).GetComponentInChildren<RectTransform>();
        }
    }

    void Awake()
    {
        if (sliders == null)
        {
            localPlotRef = GameObject.FindObjectOfType<LocalPlotController>();
            sliders = GetComponentsInChildren<Slider>();
            sliderHandles = new RectTransform[sliders.Length];
            sliderHandles[0] = sliders[0].transform.GetChild(0).GetComponentInChildren<RectTransform>();
            sliderHandles[1] = sliders[1].transform.GetChild(0).GetComponentInChildren<RectTransform>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (sliders != null)
        {
            if (updateSliderOof)
            {
                // Debug.Log("Update on slider run");
                //if max is below min, set max to min
                if (sliders[1].value < sliders[0].value + sliders[0].maxValue * 0.1f)
                {
                    sliders[1].value = sliders[0].value + sliders[0].maxValue * 0.1f;
                }
                if (sliders[0].value > sliders[1].value - sliders[0].maxValue * 0.1f)
                {
                    sliders[0].value = sliders[1].value - sliders[0].maxValue * 0.1f;
                }

                // Reposition fill
                if (transform.localEulerAngles.z == 270f)
                {
                    // y  
                    Vector3 differenceVector = (sliderHandles[1].GetChild(0).transform.position - sliderHandles[0].GetChild(0).transform.position);
                    fill.transform.position = new Vector3(fill.position.x, sliderHandles[0].GetChild(0).transform.position.y + differenceVector.y / 2f, fill.position.z);
                }
                else
                {
                    // x
                    Vector3 differenceVector = (sliderHandles[1].GetChild(0).transform.position - sliderHandles[0].GetChild(0).transform.position);
                    fill.transform.position = new Vector3(sliderHandles[0].GetChild(0).transform.position.x + differenceVector.x / 2f, fill.position.y, fill.position.z);
                }

                // Reevaluate fill width
                fill.sizeDelta = new Vector2(Remap(sliders[1].value, sliders[1].minValue, sliders[1].maxValue, 0f, 200f) - Remap(sliders[0].value, sliders[0].minValue, sliders[0].maxValue, 0f, 200f), fill.sizeDelta.y);

                // Change text of min max to their values
                foreach (Slider slider in sliders)
                {
                    MeshHandler meshHandlerRef = localPlotRef.GetPlot().GetComponent<MeshHandler>();
                    if (meshHandlerRef)
                    {
                        switch (sliderAxis)
                        {
                            case SliderAxis.x:
                                // check if x values are numerical or alphabetical
                                if (meshHandlerRef.plot.PlotOptions.XUniques == null)
                                {
                                    slider.GetComponentInChildren<Text>().text = slider.value.ToString();
                                }
                                else
                                {
                                    // Debug.Log("value: " + slider.value + ", int version: " + (int)slider.value + ", array length: " + meshHandlerRef.plot.PlotOptions.XUniques.Length);
                                    slider.GetComponentInChildren<Text>().text = meshHandlerRef.plot.PlotOptions.XUniques[(int)slider.value];
                                }

                                break;
                            case SliderAxis.y:
                                if (meshHandlerRef.plot.PlotOptions.YUniques == null)
                                {
                                    slider.GetComponentInChildren<Text>().text = slider.value.ToString();
                                }
                                else
                                {
                                    // Debug.Log("value: " + slider.value + ", int version: " + (int)slider.value + ", array length: " + meshHandlerRef.plot.PlotOptions.XUniques.Length);
                                    slider.GetComponentInChildren<Text>().text = meshHandlerRef.plot.PlotOptions.YUniques[(int)slider.value];
                                }
                                break;
                        }
                    }
                    else
                    {
                        // Debug.Log("couldn't find localPlotRef");
                    }

                }
            }
        }
        else
        {
            FindSliders();
        }
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void initialiseSlider(float[] minmax)
    {
        Debug.Log("Initialised slider");
        FindSliders();

        sliders[0].minValue = minmax[0]; sliders[0].maxValue = minmax[1]; sliders[0].value = sliders[0].minValue;
        sliders[1].minValue = minmax[0]; sliders[1].maxValue = minmax[1]; sliders[1].value = sliders[1].maxValue;


        this.updateSliderOof = true;
    }

    public void setSliderValues(float[] minmax)
    {
        Debug.Log("sliderValues set for" + sliderAxis);
        FindSliders();
        sliders[0].minValue = minmax[0]; sliders[0].maxValue = minmax[1]; sliders[0].value = sliders[0].minValue;
        sliders[1].minValue = minmax[0]; sliders[1].maxValue = minmax[1]; sliders[1].value = sliders[1].maxValue;
    }

    public void sliderValueChanged()
    {
        // Debug.Log(sliders[0].value);
        GameObject.FindObjectOfType<LocalPlotController>().SliderValueChanged(sliderAxis, sliders[0].value, sliders[1].value);
    }
}
