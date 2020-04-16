using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.XR.Interaction.Toolkit
{
  using System;
  using UnityEngine.UI;
  public class InitialDistance
  {
    public bool isInitialised;
    public Vector3 distanceVector;
    public Vector3 initialScale;
    public InitialDistance(bool _isInitialised, Vector3 _distanceVector, Vector3 _initialLocalScale)
    {
      this.isInitialised = _isInitialised;
      this.distanceVector = _distanceVector;
      this.initialScale = _initialLocalScale;
    }
  }

  public class InitialGrab
  {
    public bool isInitialised;
    public Vector3 interactorStart;
    public Vector3 interactableStart;
    public InitialGrab(bool _isInitialised, Vector3 _interactorStart, Vector3 _interactableStart)
    {
      this.isInitialised = _isInitialised;
      this.interactorStart = _interactorStart;
      this.interactableStart = _interactableStart;
    }
  }




  /// <summary>
  /// Interactable component that allows basic "scaling" functionality.
  /// </summary>
  [DisallowMultipleComponent]
  [RequireComponent(typeof(Rigidbody))]
  [AddComponentMenu("XR/XR Scale Interactable")]
  public class XRScaleInteractable : XRBaseInteractable
  {
    // private bool[] isSelected = new bool[2] { false, false };
    private bool RequireSelectExclusive;

    public override bool requireSelectExclusive
    {
      get { return RequireSelectExclusive; }
      set { RequireSelectExclusive = value; }
    }

    XRBaseInteractor m_FirstSelectingInteractor;
    /// <summary>Get the first selecting interactor for this interactable.
    public XRBaseInteractor firstSelectingInteractor { get { return m_FirstSelectingInteractor; } }

    XRBaseInteractor m_SecondSelectingInteractor;
    /// <summary>Get the first selecting interactor for this interactable.
    public XRBaseInteractor secondSelectingInteractor { get { return m_SecondSelectingInteractor; } }

    // public requireSelectExclusive

    Rigidbody m_RigidBody;

    private InitialDistance initialDistance = new InitialDistance(false, new Vector3(), new Vector3());
    private InitialGrab initialGrab = new InitialGrab(false, new Vector3(), new Vector3());


    private int prevIndex = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void Awake()
    {
      base.Awake();
      requireSelectExclusive = false;
      if (m_RigidBody == null)
        m_RigidBody = GetComponent<Rigidbody>();
      if (m_RigidBody == null)
        Debug.LogWarning("Scale Interactable does not have a required RigidBody.", this);
    }

    /// <summary>This method is called by the interaction manager
    /// when the interactor first initiates selection of an interactable.</summary>
    /// <param name="interactor">Interactor that is initiating the selection.</param>
    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
      if (!interactor)
      {
        Debug.Log("Interactor was equal to null");
        return;
      }


      if (!firstSelectingInteractor)
      {
        m_FirstSelectingInteractor = interactor;
        Debug.Log("First controller has selected object");
      }
      else
      {
        m_SecondSelectingInteractor = interactor;
        Debug.Log("Second controller has selected object");
      }
    }

    /// <summary>This method is called by the interaction manager
    /// when the interactor ends selection of an interactable.</summary>
    /// <param name="interactor">Interactor that is ending the selection.</param>
    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
      initialDistance = new InitialDistance(false, new Vector3(), new Vector3());
      initialGrab = new InitialGrab(false, new Vector3(), new Vector3());
      // base.OnSelectExit(interactor);

      if (m_SecondSelectingInteractor == interactor)
      {
        m_SecondSelectingInteractor = null;
        Debug.Log("Second controller has de-selected object");
      }
      if (m_FirstSelectingInteractor == interactor)
      {
        m_FirstSelectingInteractor = m_SecondSelectingInteractor;
        m_SecondSelectingInteractor = null;
        Debug.Log("First controller has de-selected object");
      }
    }



    // Update is called once per frame
    void Update()
    {
      // If only a single selector has been Entered
      if (m_FirstSelectingInteractor && !m_SecondSelectingInteractor)
      {

        if (!initialGrab.isInitialised)
        {
          Debug.Log("initialDistance has been reset");
          initialGrab.isInitialised = true;
          initialGrab.interactorStart = m_FirstSelectingInteractor.transform.position;
          initialGrab.interactableStart = m_FirstSelectingInteractor.selectTarget.transform.position;
        }

        //move interactable to interactor
        m_FirstSelectingInteractor.selectTarget.transform.position =
        new Vector3(
          initialGrab.interactableStart.x + (m_FirstSelectingInteractor.transform.position.x - initialGrab.interactorStart.x),
          initialGrab.interactableStart.y + (m_FirstSelectingInteractor.transform.position.y - initialGrab.interactorStart.y),
          m_FirstSelectingInteractor.selectTarget.transform.position.z
        );
      }

      // If both selectors has been Entered
      if (m_FirstSelectingInteractor && m_SecondSelectingInteractor)
      {
        if (!initialDistance.isInitialised)
        {
          Debug.Log("initialDistance has been reset");
          initialDistance.isInitialised = true;
          initialDistance.distanceVector = m_SecondSelectingInteractor.transform.position - m_FirstSelectingInteractor.transform.position;
          initialDistance.initialScale = transform.localScale;
        }

        Vector3 newDistance = m_SecondSelectingInteractor.transform.position - m_FirstSelectingInteractor.transform.position;

        float scaleMultiplier = Vector3.Magnitude(newDistance) - Vector3.Magnitude(initialDistance.distanceVector);

        transform.localScale = new Vector3(initialDistance.initialScale.x * (1 + scaleMultiplier), initialDistance.initialScale.y * (1 + scaleMultiplier), 1f);
      }
    }

    public void reRegisterColliders()
    {
      // RegisteredInteractionManager.UnregisterInteractable(this);
      // RegisteredInteractionManager = null;
      BigHack();
    }

    public void XRPointerHit(Vector3 hitPosition)
    {

      // float spacing1 = createMesh.GetComponent<CreateMesh>().spacing;
      int index = transform.GetComponent<CreateMesh>().GetIndexByPos(hitPosition);
      if (index != prevIndex)
      {
        prevIndex = index;
        transform.GetComponentInChildren<Text>().transform.gameObject.SetActive(true);
        transform.GetComponentInChildren<Text>().text = transform.GetComponent<CreateMesh>().
          dataAverages[index].ToString();
        // featureTypeText.gameObject.SetActive(true);
        // featureTypeText.text = createMesh.dataAverages[createMesh.GetComponent<CreateMesh>().GetIndexByPos(touchPosVector)].ToString();
        Vector3 tempPos = transform.GetComponent<CreateMesh>().getTextPos(index);
        transform.GetComponentInChildren<Text>().transform.position = new Vector3(tempPos.x, tempPos.y + 10, tempPos.z - 5);
        // Debug.Log("assigned pos  = " + createMesh.getTextPos(index));


      }
      Debug.Log("This interactable was hit at position: " + hitPosition);
    }

    public void XRNoPointerHit()
    {
      transform.GetComponentInChildren<Text>().transform.gameObject.SetActive(false);
    }


    public void SelectEnter()
    {

    }

    void FirstSelectEnter()
    {


    }

    void SecondSelectEnter()
    {


    }

    public void SelectExit()
    {

    }

    void FirstSelectExit()
    {

    }

    void SecondSelectExit()
    {

    }
  }
}
