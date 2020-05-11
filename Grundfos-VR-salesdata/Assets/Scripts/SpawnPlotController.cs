using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.XR.Interaction.Toolkit;

public class SpawnPlotController : MonoBehaviour
{
    public bool DebugWithoutVR = false;

    float minRot = 200f; float maxRot = 320f;


    public GameObject PlotControllerPrefab;

    private GameObject[] HandGameObjects;
    private XRDirectInteractor[] HandDirectInteractors;
    private XRRayInteractor[] HandXrayInteractors;
    private UnityEngine.XR.InputDevice[] HandInputDevices;

    private GameObject spawnedPlotController;
    private int flippedHand = -1;

    public Vector3 testVector = Vector3.zero;

    [HideInInspector]
    public bool rotateFeedForwardNeeded = true;


    void Start()
    {
        if (DebugWithoutVR)
        {
            spawnedPlotController = Instantiate(PlotControllerPrefab);
            spawnedPlotController.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
            spawnedPlotController.transform.localPosition = new Vector3(0, 0.98f, -9.49f);
            spawnedPlotController.transform.localScale = new Vector3(0.03f, 0.03f, 1f);

        }
        else
        {
            XRController[] hands = gameObject.GetComponentsInChildren<XRController>();
            HandGameObjects = new GameObject[hands.Length];
            HandDirectInteractors = new XRDirectInteractor[hands.Length];
            HandXrayInteractors = new XRRayInteractor[hands.Length];
            HandInputDevices = new UnityEngine.XR.InputDevice[hands.Length];
            for (int i = 0; i < HandGameObjects.Length; i++)
            {
                HandGameObjects[i] = hands[i].gameObject;
                HandDirectInteractors[i] = HandGameObjects[i].GetComponent<XRDirectInteractor>();
                HandXrayInteractors[i] = null;
            }
        }

    }

    void Update()
    {
        if (DebugWithoutVR)
        {

        }
        else
        {
            FindControllers();

            if (MenuCouldBeUp() || isMenuUp())
            {
                // Debug.Log("Menu could be up");
                CheckForMenu();
                if (isMenuUp())
                {
                    // Check if hand that is not flipped, currently has XRDirectInteractor
                    if (HandDirectInteractors[1 - flippedHand])
                    {
                        // If it does, it should be removed
                        Destroy(HandDirectInteractors[1 - flippedHand].attachTransform.gameObject);
                        Destroy(HandDirectInteractors[1 - flippedHand]);
                        HandDirectInteractors[1 - flippedHand] = null;
                        // Debug.Log("Destroyed HandDirector for :" + (1 - flippedHand));
                    }
                    // If it doesn't, check if it has xray
                    else
                    {
                        if (!HandXrayInteractors[1 - flippedHand])
                        {
                            // If it doesn't, add xray to that hand
                            HandXrayInteractors[1 - flippedHand] = HandGameObjects[1 - flippedHand].AddXrayComponent();
                            // Debug.Log("Added Xray for :" + (1 - flippedHand));

                            // Remove the rotate feedforward sprite on both hands
                            foreach (GameObject hand in HandGameObjects)
                            {
                                Color temp = hand.GetComponentInChildren<Image>().color;
                                hand.GetComponentInChildren<Image>().color = new Color(temp.r, temp.g, temp.b, 0);
                            }
                        }
                    }
                }
                else
                {
                    // Debug.Log("Menu could be up but it isn't");
                    // For each hand,
                    for (int i = 0; i < HandGameObjects.Length; i++)
                    {
                        // Check if hand has Xray Interactor Component on it
                        // If it does, destory component
                        if (HandXrayInteractors[i])
                        {
                            // Debug.Log("Destroyed Xray for :" + i);
                            HandXrayInteractors[i].RemoveXrayComponent();
                            // Destroy(HandXrayInteractors[i]);
                            HandXrayInteractors[i] = null;
                        }
                        // If it doesn't
                        else
                        {
                            // Check if hand has Direct Interactor Component
                            if (!HandDirectInteractors[i])
                            {
                                // Debug.Log("Added Direct Interactor for :" + i);
                                // If it doesn't add a Director Interactor component to it
                                HandDirectInteractors[i] = HandGameObjects[i].AddDirectComponent();

                                // If button wasn't clicked sprites should be reshown
                                if (rotateFeedForwardNeeded)
                                {
                                    foreach (GameObject hand in HandGameObjects)
                                    {
                                        Color temp = hand.GetComponentInChildren<Image>().color;
                                        hand.GetComponentInChildren<Image>().color = new Color(temp.r, temp.g, temp.b, 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Debug.Log("Menu can't be up");
            }
        }
    }

    private bool MenuCouldBeUp()
    {
        if (HandDirectInteractors[0] || HandDirectInteractors[1])
        {
            if ((HandDirectInteractors[0] && HandDirectInteractors[0].isSelectActive) || (HandDirectInteractors[1] && HandDirectInteractors[1].isSelectActive))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    private void FindControllers()
    {
        for (int i = 0; i < HandInputDevices.Length; i++)
        {
            if (!HandInputDevices[i].isValid)
            {
                List<UnityEngine.XR.InputDevice> InputDevices = new List<UnityEngine.XR.InputDevice>();
                switch (i)
                {
                    case 0:
                        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, InputDevices);
                        break;
                    case 1:
                        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, InputDevices);
                        break;
                }
                if (InputDevices.Count == 1)
                {
                    HandInputDevices[i] = InputDevices[0];
                    Debug.Log(string.Format("Device name '{0}' with role '{1}'", HandInputDevices[i].name, HandInputDevices[i].characteristics.ToString()));
                }
                else if (InputDevices.Count > 1)
                {
                    Debug.Log("Found more than one hand!");
                }
                else if (InputDevices.Count == 0)
                {
                    // Debug.Log("Found no left hand");
                }
            }
        }
    }

    public void CheckForMenu()
    {
        float RHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand).eulerAngles.z;
        float LHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles.z;

        // Continiously check if hand is tilted and not grabbing anything
        // If that is the case, spawn the canvas for plotcontroller as child to hand (remember to have canvas in worldspace)
        bool[] boolArr = new bool[2] { false, false };

        if (LHandRot > 360 - maxRot && LHandRot < 360 - minRot)
        {
            boolArr[0] = true;
        }
        else
        {
            boolArr[0] = false;
        }
        if (RHandRot < maxRot && RHandRot > minRot)
        {
            boolArr[1] = true;
        }
        else
        {
            boolArr[1] = false;
        }

        if (boolArr[0] && !boolArr[1] && flippedHand == -1)
        {
            Spawn(HandSide.Left);
        }
        else if (!boolArr[0] && boolArr[1] && flippedHand == -1)
        {
            Spawn(HandSide.Right);
        }

        if (flippedHand != -1 && !boolArr[flippedHand])
        {
            DeSpawn();
        }
    }

    private void Spawn(HandSide hand)
    {
        flippedHand = (int)hand;

        spawnedPlotController = Instantiate(PlotControllerPrefab);
        spawnedPlotController.transform.GetChild(0).GetComponent<Canvas>().worldCamera = Camera.main;
        spawnedPlotController.transform.SetParent(HandGameObjects[flippedHand].transform);
        // spawnedPlotController.transform.localPosition = new Vector3(0, -0.12f, 0);
        switch (hand)
        {
            case HandSide.Left:
                Debug.Log("Leftside");
                spawnedPlotController.transform.localPosition = new Vector3(0.17f, -0.09f);

                break;
            case HandSide.Right:
                Debug.Log("Rightside");
                spawnedPlotController.transform.localPosition = new Vector3(-0.17f, -0.09f);
                break;
            default:
                Debug.Log("Default");
                break;
        }
    }

    private void DeSpawn()
    {
        flippedHand = -1;

        Destroy(spawnedPlotController);
    }

    public bool isMenuUp()
    {
        if (flippedHand == -1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
