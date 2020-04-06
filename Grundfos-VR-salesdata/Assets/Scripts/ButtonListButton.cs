using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{
    [SerializeField]
    private Text myText;

    [SerializeField]
    private ButtonListControl buttonControl;

    //private LocalPlotController plotController;
    private string myTextString;
    [SerializeField]
    int myButtonID;


    // Sets the header string name to the button
    public void InitializeButton(string textString, int buttonID)
    {
        myTextString = textString;
        myText.text = textString;
        myButtonID = buttonID;
    }

    public void AssignID(Button button)
    {
        buttonControl.FeatureSelected = myButtonID;// buttonControl.ChangeButtonColor(button);
        //ChangeButtonColor(button);
        Debug.Log(myButtonID);
        // return localID;
        //return myButtonID;
    }




}
