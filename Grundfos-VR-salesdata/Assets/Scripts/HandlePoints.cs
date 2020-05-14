using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// public enum HandSide
// {
//   Left = 0,
//   Right = 1
// }
public class HandlePoints : MonoBehaviour
{
    private GameObject canvasGameObject;
    private MeshHandler meshHandlerRef;

    private GameObject[] temporaryTextHolder = new GameObject[] { null, null };
    public GameObject[] TemporaryTextHolder { get { return temporaryTextHolder; } set { temporaryTextHolder = value; } }

    [SerializeField]

    private GameObject textPrefab;

    private GameObject[] savedSpawnedTexts;
    public GameObject[] SavedSpawnedTexts { get { return savedSpawnedTexts; } set { savedSpawnedTexts = value; } }


    private bool[] previouslyAiming = new bool[] { false, false };

    // Start is called before the first frame update
    void Start()
    {
        if (!canvasGameObject)
            canvasGameObject = transform.gameObject.GetComponentInChildren<Canvas>().gameObject;

        canvasGameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        if (!meshHandlerRef)
            meshHandlerRef = gameObject.GetComponent<MeshHandler>();

        if (!textPrefab)
        {
            Debug.Log("You forgot to add reference to the text prefab");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void XRPointerHit(Vector3 hitPosition, HandSide handside, Vector3 hitWorldSpace)
    {
        previouslyAiming[(int)handside] = true;
        int index = meshHandlerRef.GetIndexByPos(hitPosition);



        if (!temporaryTextHolder[(int)handside])
        {
            temporaryTextHolder[(int)handside] = FindObjectOfType<GlobalPlotController>().SpawnBarValue(
            meshHandlerRef.plot.PlotID, (int)handside, meshHandlerRef.GetDataAverages()[index + 1].ToString(),
             hitWorldSpace, canvasGameObject.transform);
            // temporaryTextHolder[(int)handside] = Instantiate(textPrefab, hitPosition, Quaternion.identity);
            // temporaryTextHolder[(int)handside].transform.SetParent(canvasGameObject.transform);
            // temporaryTextHolder[(int)handside].transform.localScale = new Vector3(1, 1, 1);
        }

        FindObjectOfType<GlobalPlotController>().SetBarValueText(
            meshHandlerRef.plot.PlotID, (int)handside,
            meshHandlerRef.GetDataAverages()[index + 1].ToString(), temporaryTextHolder[(int)handside], hitWorldSpace);
        // temporaryTextHolder[(int)handside].GetComponent<Text>().text = meshHandlerRef.GetDataAverages()[index + 1].ToString();
        // Vector3 tempPos = meshHandlerRef.getTextPos(index);
        // temporaryTextHolder[(int)handside].transform.position = hitWorldSpace;
    }

    public void XRNoPointerHit(HandSide handside)
    {
        if (previouslyAiming[(int)handside])
        {
            previouslyAiming[(int)handside] = false;
            Destroy(temporaryTextHolder[(int)handside]);
            temporaryTextHolder[(int)handside] = null;
            FindObjectOfType<GlobalPlotController>().RemoveBarValue(meshHandlerRef.plot.PlotID, (int)handside);
        }
    }

    public void XRPointerHitSave(Vector3 hitPosition, HandSide handSide)
    {
        // If handlepoint doesn't have a regstry for each bar (to save text in)
        if (savedSpawnedTexts == null)
        {
            savedSpawnedTexts = new GameObject[meshHandlerRef.plot.DataCompared.Length];
        }

        if (savedSpawnedTexts.Length != meshHandlerRef.plot.DataCompared.Length)
        {
            // should really do some smart stuff here but out of time
            savedSpawnedTexts = new GameObject[meshHandlerRef.plot.DataCompared.Length];
        }

        int index = meshHandlerRef.GetIndexByPos(hitPosition);


        // FindObjectOfType<GlobalPlotController>().SpawnBarValue(meshHandlerRef)

        if (savedSpawnedTexts[index])
        {
            Destroy(savedSpawnedTexts[index]);
        }
        savedSpawnedTexts[index] = Instantiate(temporaryTextHolder[(int)handSide]);
        savedSpawnedTexts[index].transform.SetParent(canvasGameObject.transform, false);

        FindObjectOfType<GlobalPlotController>().SaveBar(meshHandlerRef.plot.PlotID, (int)handSide, index);
    }
}
