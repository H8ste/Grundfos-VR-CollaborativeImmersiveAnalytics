﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    public Camera camera;
    RaycastHit hit;
    private CreateMesh createMesh = null;
    public Vector3 touchPosVector;

    [SerializeField]
    public Text featureTypeText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        CalculateCoordOfHit();

    }
    public void CalculateCoordOfHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 15, Color.yellow);
        // Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 20, Color.green, 2, false);
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);




        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.collider.gameObject.name);

            Vector3 touchPosVector = new Vector3(hit.collider.gameObject.transform.position.x, hit.collider.gameObject.transform.position.y, hit.collider.transform.position.z);
            touchPosVector = hit.point - touchPosVector;
            // Debug.Log("world coords : " + hit.point);
            // Debug.Log("coords on the barchart" + touchPosVector.ToString());

            if (createMesh)
            {
                // float spacing1 = createMesh.GetComponent<CreateMesh>().spacing;
                featureTypeText.gameObject.SetActive(true);
                featureTypeText.text = createMesh.GetComponent<CreateMesh>().setTextBox(touchPosVector);
                featureTypeText.gameObject.transform.position = hit.point;
            }
            else
            {

                createMesh = GameObject.FindObjectOfType<CreateMesh>();
                // createMesh.dataAverages;
            }
        }
        //featureTypeText.gameObject.SetActive(false);
    }




}