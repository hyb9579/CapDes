using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpawnOnPlane : MonoBehaviour
    //, IPointerClickHandler
{
    #region Editor public fields

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    GameObject crosshairPrefab;

    [SerializeField]
    Color crosshairInsidePlaneColor = Color.green;

    [SerializeField]
    Color crosshairOutsidePlaneColor = Color.yellow;

    [SerializeField]
    GameObject prefabToSpawn;

    [SerializeField]
    ARRaycastManager arraycastManager;

    [SerializeField]
    float scaleMinimum = 0.2f;

    [SerializeField]
    float scaleMaximum = 1f;



    [SerializeField]
    GameObject Bluetooth;

    [SerializeField]
    GameObject Controller;
    
    private Boolean isTouched;

    manager manager_cs;

    //[SerializeField]
    //GameObject[] prefabList;

    #endregion

    #region Public properties

    #endregion

    #region Private fields

    Vector3 screenCenter;

    GameObject crosshair;
    Renderer crosshairRenderer;

    Ray ray;

    readonly List<ARRaycastHit> arraycastHits = new List<ARRaycastHit>();

    bool raycast;
    bool arraycast;

    RaycastHit hitob;

    int shaderColorId;

    int layerMask;

    #endregion

    #region Unity methods

    void Start() {

        layerMask = 1 << LayerMask.NameToLayer("Object");

        manager_cs = Bluetooth.GetComponent<manager>();
        screenCenter = new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 0f);
        
        crosshair = Instantiate(crosshairPrefab);
        crosshairRenderer = crosshair.GetComponentInChildren<Renderer>();
        crosshair.SetActive(false);

        shaderColorId = Shader.PropertyToID("_Color");
    }

    void Update() {

        //if (Input.touchCount == 0)
        //{
        //    return;
        //}

        //Touch touch = Input.GetTouch(0);

        ////if(touch.phase != TouchPhase.Began)
        ////{
        ////    return;
        ////}

        //ray = touch.position;

        isTouched = manager_cs.GetTouch();

        ray.origin = Controller.transform.position;
        ray.direction = Controller.transform.forward;

        // ray.direction = Controller.transform.rotation.eulerAngles + new Vector3(0, -90, 0);
        // foward를 사용해 봐야 할듯?

        raycast = Physics.Raycast(ray, out hitob, 10, layerMask);

        arraycast = arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinBounds) ||
                arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinInfinity);


        //if (raycast && !arraycast) //객체 위치조정 X, 자세조정 O
        //{

        //}
        //if(raycast && arraycast) //객체 위치조정 O, 자세조정 O
        //{

        //}
        //else if(!raycast && arraycast) // 새 객체 생성
        //{

        //}

            
        if (arraycast)
        {

            var hit = arraycastHits[0];
            var pose = hit.pose;

            crosshair.SetActive(true);
            crosshair.transform.position = pose.position;
            crosshair.transform.up = pose.up;

            if (crosshairRenderer != null)
            {
                var color = hit.hitType == TrackableType.PlaneWithinBounds
                    ? crosshairInsidePlaneColor
                    : crosshairOutsidePlaneColor;
                crosshairRenderer.material.SetColor(shaderColorId, color);
            }

            //객체 조작시
            if (raycast)
            {
                if (isTouched == true)
                {
                    hitob.transform.position = pose.position;
                    hitob.transform.up = Controller.transform.forward;

                }
            }
            else
            {
                //새 객체 생성
                if (isTouched == true)
                {
                    pose = arraycastHits[0].pose;
                    var spawnedObject = Instantiate(prefabToSpawn, pose.position, pose.rotation);

                    // Adjust the spawned object to look towards the camera (while staying
                    // perpendicular to the plane of a general orientation).
                    // Project the camera position to the tracked plane:
                    Vector3 distance = mainCamera.transform.position - spawnedObject.transform.position;
                    Vector3 normal = spawnedObject.transform.up.normalized;
                    Vector3 projectedPoint = mainCamera.transform.position
                        - (normal * Vector3.Dot(normal, distance));

                    // Rotate the spawned object towards the projected position:
                    Vector3 newForward = projectedPoint - spawnedObject.transform.position;
                    float angle = Vector3.Angle(spawnedObject.transform.forward, newForward);
                    spawnedObject.transform.Rotate(spawnedObject.transform.up, angle, Space.World);

                    //var randomScale = UnityEngine.Random.Range(scaleMinimum, scaleMaximum);
                    //spawnedObject.transform.localScale = Vector3.one * randomScale;
                }

            }

            //isTouched가 True일때
            //hit.transform으로 물체의 위치, 자세 조작하기
            //hit.point 와hit.transform.position로 지면으로의 벡터를 이용해
            //hit.transform.up
            //isTouched True 되면 객체의 position을 가르키도록 하고 터치를 떼면돌아오거나
            //터치하면 객체의 position을 가르키는 가상의 ray를 만들고 원래 controller와 같이 움직이도록
            //아두이노 Y축 회전값이 객체의 회전을 구현하도록
        }
        else
        {
            crosshair.SetActive(false);
        }




        //if (Physics.Raycast(ray, out hit, 10))
        //{
        //    if(isTouched = True)
        //    {
        //        //hit.transform.position = ;
        //    }

        //    //isTouched가 True일때
        //    //hit.transform으로 물체의 위치, 자세 조작하기
        //    //hit.point 와hit.transform.position로 지면으로의 벡터를 이용해
        //    //hit.transform.up
        //    //isTouched True 되면 객체의 position을 가르키도록 하고 터치를 떼면돌아오거나
        //    //터치하면 객체의 position을 가르키는 가상의 ray를 만들고 원래 controller와 같이 움직이도록
        //    //아두이노 Y축 회전값이 객체의 회전을 구현하도록


        //}

        //if (arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinBounds) ||
        //        arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinInfinity))
        //{
        //    var hit = arraycastHits[0];
        //    var pose = hit.pose;

        //    crosshair.SetActive(true);
        //    crosshair.transform.position = pose.position;
        //    crosshair.transform.up = pose.up;

        //    if (crosshairRenderer != null)
        //    {
        //        var color = hit.hitType == TrackableType.PlaneWithinBounds
        //            ? crosshairInsidePlaneColor
        //            : crosshairOutsidePlaneColor;
        //        crosshairRenderer.material.SetColor(shaderColorId, color);
        //    }


        //    if (isTouched == true)
        //    {
        //        pose = arraycastHits[0].pose;
        //        var spawnedObject = Instantiate(prefabToSpawn, pose.position, pose.rotation);

        //        // Adjust the spawned object to look towards the camera (while staying
        //        // perpendicular to the plane of a general orientation).
        //        // Project the camera position to the tracked plane:
        //        Vector3 distance = mainCamera.transform.position - spawnedObject.transform.position;
        //        Vector3 normal = spawnedObject.transform.up.normalized;
        //        Vector3 projectedPoint = mainCamera.transform.position
        //            - (normal * Vector3.Dot(normal, distance));

        //        // Rotate the spawned object towards the projected position:
        //        Vector3 newForward = projectedPoint - spawnedObject.transform.position;
        //        float angle = Vector3.Angle(spawnedObject.transform.forward, newForward);
        //        spawnedObject.transform.Rotate(spawnedObject.transform.up, angle, Space.World);

        //        var randomScale = UnityEngine.Random.Range(scaleMinimum, scaleMaximum);
        //        spawnedObject.transform.localScale = Vector3.one * randomScale;
        //    }

        //}
        //else
        //{
        //    crosshair.SetActive(false);
        //}


        // Raycasting with only PlaneWithinInfinity always sets hitType to PlaneWithinInfinity, so
        // we need to potentially do two raycasts in order to color the crosshair properly.
        //if (arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinBounds) ||
        //    arraycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinInfinity)) {
        //    var hit = arraycastHits[0];
        //    var pose = hit.pose;

        //    crosshair.SetActive(true);
        //    crosshair.transform.position = pose.position;
        //    crosshair.transform.up = pose.up;

        //    if (crosshairRenderer != null) {
        //        var color = hit.hitType == TrackableType.PlaneWithinBounds
        //            ? crosshairInsidePlaneColor
        //            : crosshairOutsidePlaneColor;
        //        crosshairRenderer.material.SetColor(shaderColorId, color);
        //    }


        //    if (isTouched == true)
        //    {
        //        pose = arraycastHits[0].pose;
        //        var spawnedObject = Instantiate(prefabToSpawn, pose.position, pose.rotation);

        //        // Adjust the spawned object to look towards the camera (while staying
        //        // perpendicular to the plane of a general orientation).
        //        // Project the camera position to the tracked plane:
        //        Vector3 distance = mainCamera.transform.position - spawnedObject.transform.position;
        //        Vector3 normal = spawnedObject.transform.up.normalized;
        //        Vector3 projectedPoint = mainCamera.transform.position
        //            - (normal * Vector3.Dot(normal, distance));

        //        // Rotate the spawned object towards the projected position:
        //        Vector3 newForward = projectedPoint - spawnedObject.transform.position;
        //        float angle = Vector3.Angle(spawnedObject.transform.forward, newForward);
        //        spawnedObject.transform.Rotate(spawnedObject.transform.up, angle, Space.World);

        //        var randomScale = UnityEngine.Random.Range(scaleMinimum, scaleMaximum);
        //        spawnedObject.transform.localScale = Vector3.one * randomScale;
        //    }

        //} else {
        //    crosshair.SetActive(false);
        //}

    }

    #endregion

    #region Interaction
    
    //화면터치를 이용할때
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (raycastManager.Raycast(ray, arraycastHits, TrackableType.PlaneWithinBounds
    //        | TrackableType.PlaneWithinInfinity))
    //    {

    //        var pose = arraycastHits[0].pose;
    //        var spawnedObject = Instantiate(prefabToSpawn, pose.position, pose.rotation);

    //        // Adjust the spawned object to look towards the camera (while staying
    //        // perpendicular to the plane of a general orientation).
    //        // Project the camera position to the tracked plane:
    //        Vector3 distance = mainCamera.transform.position - spawnedObject.transform.position;
    //        Vector3 normal = spawnedObject.transform.up.normalized;
    //        Vector3 projectedPoint = mainCamera.transform.position
    //            - (normal * Vector3.Dot(normal, distance));

    //        // Rotate the spawned object towards the projected position:
    //        Vector3 newForward = projectedPoint - spawnedObject.transform.position;
    //        float angle = Vector3.Angle(spawnedObject.transform.forward, newForward);
    //        spawnedObject.transform.Rotate(spawnedObject.transform.up, angle, Space.World);

    //        var randomScale = Random.Range(scaleMinimum, scaleMaximum);
    //        spawnedObject.transform.localScale = Vector3.one * randomScale;
    //    }
    //}

    #endregion

    //void ChangePrefabs()
    //{
    //    //prefabToSpawn = prefabList[i];
    //}

}