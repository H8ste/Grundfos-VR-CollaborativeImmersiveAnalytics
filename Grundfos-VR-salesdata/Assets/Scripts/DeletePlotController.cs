using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePlotController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 endPosition;
    private Vector3 currentPosition;
    public Vector3 CurrentPosition { get { return currentPosition; } set { currentPosition = value; } }



    private GameObject[] lines;
    private Text text;

    public float threshold = 0.7f;

    private bool isDeleting = false;




    // Start is called before the first frame update
    void Start()
    {
        // Getting lines
        lines = new GameObject[transform.childCount - 1];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = transform.GetChild(i).gameObject;
        }
        // Getting text
        text = transform.GetComponentInChildren<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDeleting)
        {
            //get progress
            float progress = getProgress();

            //pass that progress into width of both lines + change opacity of both text and both lines

            if (progress <= 1 && progress >= 0)
            {
                foreach (GameObject line in lines)
                {
                    line.GetComponent<RectTransform>().sizeDelta = new Vector2(line.GetComponent<RectTransform>().sizeDelta.x, progress * 100);
                    Image image = line.GetComponent<Image>();
                    image.color = new Color(image.color.r, image.color.g, image.color.b, progress);
                }
                text.color = new Color(text.color.r, text.color.g, text.color.b, progress);
                if (progress >= .97f)
                {
                    Transform.FindObjectOfType<GlobalPlotController>().DeletePlot(transform.parent.parent.parent.gameObject.GetComponent<MeshHandler>().plot.PlotID);
                    gameObject.SetActive(false);
                }
            }
        }

    }

    public void beginDeletion()
    {
        isDeleting = true;
    }
    public void endDeletion()
    {
        isDeleting = false;
        foreach (GameObject line in lines)
        {
            line.GetComponent<RectTransform>().sizeDelta = new Vector2(line.GetComponent<RectTransform>().sizeDelta.x, 0f);
            Image image = line.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
    }

    public void resetDeletion(Vector3 _initialPosition)
    {
        initialPosition = _initialPosition;
        endPosition = initialPosition + new Vector3(0f, 0f, -threshold);
    }


    private float getProgress()
    {
        return (currentPosition - initialPosition).z / (endPosition - initialPosition).z;
    }
}
