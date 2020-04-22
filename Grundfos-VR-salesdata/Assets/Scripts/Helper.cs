using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;

public static class Helper
{
  public static T GetComponentInChildWithTag<T>(this GameObject _parent, string tag) where T : Component
  {
    Transform t = _parent.transform;
    foreach (Transform tr in t)
    {
      if (tr.tag == tag)
      {
        return tr.GetComponent<T>();
      }
    }
    return null;
  }
  public static T GetComponentInChildrenWithTag<T>(this GameObject _parent, string tag) where T : Component
  {
    Transform parent = _parent.transform;
    foreach (Transform child in parent)
    {
      if (child.tag == tag)
      {
        return child.GetComponent<T>();
      }
      foreach (Transform grandChild in child)
      {
        if (grandChild.tag == tag)
        {
          return grandChild.GetComponent<T>();
        }
      }
    }
    return null;
  }

  public static XRRayInteractor AddXrayComponent(this GameObject hand)
  {
    XRRayInteractor ray = hand.AddComponent<XRRayInteractor>();
    // And Change XR Ray to be a line
    ray.lineType = XRRayInteractor.LineType.ProjectileCurve;
    ray.Velocity = 8;
    ray.enabled = true;
    LineRenderer lineRender = hand.AddComponent<LineRenderer>();
    XRInteractorLineVisual xRInteractorLineVisual = hand.AddComponent<XRInteractorLineVisual>();
    return ray;
  }

  public static void RemoveXrayComponent(this XRRayInteractor ray)
  {
    if (ray)
    {
      SpawnPlotController.Destroy(ray.gameObject.GetComponent<XRInteractorLineVisual>());
      SpawnPlotController.Destroy(ray.GetComponent<LineRenderer>());
      SpawnPlotController.Destroy(ray.attachTransform.gameObject);
      SpawnPlotController.Destroy(ray);
      // Debug.Log("Removed XRRAY");
    }
    // else
    // {
    //   XRDirectInteractor directInteractor = hand.GetComponent<XRDirectInteractor>();
    //   if (!directInteractor)
    //   {
    //     hand.AddComponent<XRDirectInteractor>();
    //     // Debug.Log("Added XRDirectInteractor");
    //   }
    // }
  }

  public static XRDirectInteractor AddDirectComponent(this GameObject hand)
  {
    return hand.AddComponent<XRDirectInteractor>();
  }
}
