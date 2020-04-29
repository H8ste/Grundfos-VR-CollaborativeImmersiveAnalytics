using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPlotToGlobalPlot : MonoBehaviour
{
    // Start is called before the first frame update

    private LocalPlotController localPlot;
    private GlobalPlotController globalPlot;

    void Start()
    {
        localPlot = GameObject.FindObjectOfType<LocalPlotController>();
        globalPlot = GameObject.FindObjectOfType<GlobalPlotController>();
    }

    void Awake()
    {
        localPlot = GameObject.FindObjectOfType<LocalPlotController>();
        globalPlot = GameObject.FindObjectOfType<GlobalPlotController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendPlotToGlobalPlotController()
    {
        globalPlot.AddPlot(localPlot.GetPlot());
    }
}
