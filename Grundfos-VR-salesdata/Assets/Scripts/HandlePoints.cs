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
  private CreateMesh createMeshRef;

  private GameObject[] temporaryTextHolder = new GameObject[] { null, null };

  [SerializeField]

  private GameObject textPrefab;


  private bool[] previouslyAiming = new bool[] { false, false };

  // Start is called before the first frame update
  void Start()
  {
    if (!canvasGameObject)
      canvasGameObject = transform.gameObject.GetComponentInChildren<Canvas>().gameObject;

    canvasGameObject.GetComponent<Canvas>().worldCamera = Camera.main;

    if (!createMeshRef)
      createMeshRef = gameObject.GetComponent<CreateMesh>();

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
    int index = createMeshRef.GetIndexByPos(hitPosition);
    if (!temporaryTextHolder[(int)handside])
    {
      temporaryTextHolder[(int)handside] = Instantiate(textPrefab, hitPosition, Quaternion.identity);
      temporaryTextHolder[(int)handside].transform.SetParent(canvasGameObject.transform);
      temporaryTextHolder[(int)handside].transform.localScale = new Vector3(1, 1, 1);

    }
    // if (index != prevIndex)
    // {
    prevIndex = index;
    temporaryTextHolder[(int)handside].GetComponent<Text>().text = createMeshRef.dataAverages[index].ToString();
    Vector3 tempPos = transform.GetComponent<CreateMesh>().getTextPos(index);
    temporaryTextHolder[(int)handside].transform.position = hitWorldSpace;
    // }
    Debug.Log("This interactable was hit at position: " + hitPosition);
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
}
