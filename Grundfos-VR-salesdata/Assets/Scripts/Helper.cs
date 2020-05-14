using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

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

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

}
