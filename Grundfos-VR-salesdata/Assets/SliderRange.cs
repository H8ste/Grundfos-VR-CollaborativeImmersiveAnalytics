using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderRange : MonoBehaviour
{
    // Start is called before the first frame update
    Slider[] sliders;
    RectTransform[] sliderHandles;

    bool updateSliderOof = true;

    public RectTransform fill;
    void FindSliders()
    {
        Debug.Log("finding sliders");
        sliders = GetComponentsInChildren<Slider>();
        sliderHandles = new RectTransform[sliders.Length];
        sliderHandles[0] = sliders[0].transform.GetChild(0).GetComponentInChildren<RectTransform>();
        sliderHandles[1] = sliders[1].transform.GetChild(0).GetComponentInChildren<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sliders != null)
        {
            Debug.Log("Sliders found inside update: " + sliders.Length);



            // Debug.Log(updateSliderOof);
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


                Debug.Log("Sliders found: " + sliders.Length);
                // Change text of min max to their values
                foreach (Slider slider in sliders)
                {
                    Debug.Log(slider.value);
                    slider.GetComponentInChildren<Text>().text = slider.value.ToString();
                }
            }
            else
            {

                // Debug.Log("not running update");
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
        FindSliders();

        Debug.Log(minmax[0] + ", " + minmax[1]);

        Debug.Log("Sliders found: " + sliders.Length);

        sliders[0].minValue = minmax[0]; sliders[0].maxValue = minmax[1]; sliders[0].value = sliders[0].minValue;
        sliders[1].minValue = minmax[0]; sliders[1].maxValue = minmax[1]; sliders[1].value = sliders[1].maxValue;


        this.updateSliderOof = true;
    }
}
