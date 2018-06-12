using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_3p : MonoBehaviour
{
    private const float Y_ANGLE_MIN = -5.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    [SerializeField] private float camY_Min = -2.0f;
    [SerializeField] private float camY_Max = 50.0f;

    [SerializeField] private float dist_Min = 30.0f;
    [SerializeField] private float dist_Max = 60.0f;
    [SerializeField] private float scrollSpeed = 10.0f;

    public Transform cam_target;
    public Transform target;
    public Transform camTransform;
    public Transform lockTarget;

    public GameObject player;

    private Vector3 camSnapTransform;
    private Camera cam;

    [SerializeField] private float distance = 21.0f;
    [SerializeField] private float currentX = 0.0f;
    [SerializeField] private float currentY = 0.0f;
    [SerializeField] private float camTargetY = 0.0f;
    [SerializeField] private float sensitivityX = 4.0f;
    [SerializeField] private float sensitivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X");
            currentY -= Input.GetAxis("Mouse Y");

            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
            camTargetY = Mathf.Clamp(currentY, camY_Min, camY_Max);
        }
    }

    private void LateUpdate()
    {

        bool tbool = player.GetComponent<P_Movement>().targetToggle;

        // Rotate camera with right-click/hold
        Vector3 camdir = new Vector3(0, 0, 15);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Quaternion camrotation = Quaternion.Euler(0, currentX, 0);

        if (tbool == true)
        {
            lockTarget = GameObject.Find("cam_Lookat").transform;

            var v3T = target.position - lockTarget.position;
            camTransform.position = target.position + v3T.normalized * distance;
        }
        else if (tbool == false)
        {
            distance = 21.0f;
            lockTarget = GameObject.Find("Lookat").transform;
            cam_target.position = target.position + camrotation * camdir;
            Vector3 dir = new Vector3(0, 0, -distance);
            camTransform.position = target.position + rotation * dir;
        }

        
        //if (tbool == false)
        //{
        //    distance = 21.0f;
        //    lockTarget = GameObject.Find("Lookat").transform;
        //    cam_target.position = target.position + camrotation * camdir;
        //}
        //else if (tbool == true)
        //{
        //    lockTarget = GameObject.Find("cam_Lookat").transform;
        //    float newDist = Vector3.Distance(target.transform.position, player.transform.position);
        //    distance += newDist;
        //}

        
        camTransform.LookAt(lockTarget.position);

        // Zoom extent with middle mouse wheel
        distance = distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

        distance = Mathf.Clamp(distance, dist_Min, dist_Max);
    }

}
