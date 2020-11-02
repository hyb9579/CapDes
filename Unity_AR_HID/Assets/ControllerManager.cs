using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public GameObject CameraToFollow;

    private Vector3 camPos;

    //private void Awake()
    //{
    //    //CameraToFollow = GameObject.FindWithTag("MainCamere");
    //    //BTmanager = GameObject.Find("BTmanager");
    //}
    

    // Update is called once per frame
    void Update()
    {
        camPos = CameraToFollow.transform.position;
        transform.position = new Vector3(camPos.x + 0.15f, camPos.y - 0.5f, camPos.z + 0.25f);
    }
}