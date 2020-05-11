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

    private int prevIndex = -1;

    private GameObject canvasGameObject;
    private MeshHandler meshHandlerRef;

    private GameObject[] temporaryTextHolder = new GameObject[] { null, null };

    [SerializeField]

    private GameObject textPrefab;

    private GameObject[] savedSpawnedTexts;


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
            temporaryTextHolder[(int)handside] = Instantiate(textPrefab, hitPosition, Quaternion.identity);
            temporaryTextHolder[(int)handside].transform.SetParent(canvasGameObject.transform);
            temporaryTextHolder[(int)handside].transform.localScale = new Vector3(1, 1, 1);

        }
        prevIndex = index;
        temporaryTextHolder[(int)handside].GetComponent<Text>().text = meshHandlerRef.GetDataAverages()[index + 1].ToString();
        Vector3 tempPos = meshHandlerRef.getTextPos(index);
        temporaryTextHolder[(int)handside].transform.position = hitWorldSpace;
    }

    public void XRNoPointerHit(HandSide handside)
    {
        if (previouslyAiming[(int)handside])
        {
            previouslyAiming[(int)handside] = false;
            Destroy(temporaryTextHolder[(int)handside]);
            temporaryTextHolder[(int)handside] = null;
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

        if (savedSpawnedTexts[index])
        {
            Destroy(savedSpawnedTexts[index]);
        }
        savedSpawnedTexts[index] = Instantiate(temporaryTextHolder[(int)handSide]);
        savedSpawnedTexts[index].transform.SetParent(canvasGameObject.transform, false);
    }
}
