using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Movement : MonoBehaviour {

    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float jukeSpeed = 2.0f; // Used for determining reaction speed when drastically changing direction

    [SerializeField] private float move_direction_Z; // Forward & Backward
    [SerializeField] private float move_direction_X; // Left & Right

    public Transform mCamera;
    public Transform lookat;
    public Transform target;
    public Transform head;

    [SerializeField] public bool targetToggle = false;
    [SerializeField] public bool scanTargets = false;

    [SerializeField] private float yRotation;
    [SerializeField] private float zRotation;
    [SerializeField] private float xRotation;

    [SerializeField] private float laX;
    [SerializeField] private float laY;
    [SerializeField] private float laZ;

    [SerializeField] private int hnRange = 0;
    [SerializeField] private GameObject[] targetArr;
    [SerializeField] private GameObject targetEnemy;
    [SerializeField] private int currentEnemy;

    public Transform other;

    [SerializeField] private float rotateSpeed = 125.0f;

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        lookat.position = new Vector3(transform.position.x + 0.75f, transform.position.y + 1.75f, transform.position.z + 1.00f);

        InputManager();
        //Movement();
        if (targetToggle)
            {
                zTargeting();
            }
        PlayerMovement();
    }

    private void InputManager()
    {
        // Determine zTargetting Toggle
        if (Input.GetKeyDown(KeyCode.Z))
        {
            targetToggle = !targetToggle;
            if (targetToggle == true)
            {
                scanTargets = true;
            }
        }

        //Determine next target
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(targetToggle == true)
            {
                int i = hnRange;
                if (currentEnemy + 1 > hnRange)
                {
                    i = 0;
                }
                else
                {
                    i = currentEnemy + 1;
                }
                targetEnemy = targetArr[i];
                acquireTarget(targetEnemy);
            }
        }
    }

    void PlayerMovement()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        move_direction_X = Input.GetAxis("Horizontal");
        move_direction_Z = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(move_direction_X, 0.0f, move_direction_Z);
        movement = mCamera.transform.TransformDirection(movement);
        movement.y = 0.0f;
        transform.position += movement * moveSpeed * Time.deltaTime;
        
        // Tilt player in determined direction
        if (move_direction_X > 0)
        {

        }
    }

    void acquireTarget(GameObject targetEnemy)
    {
        targetEnemy.GetComponent<E_Behavior>().E_Focus = true;
        Transform eFocus = targetEnemy.transform.GetChild(0);
        float tDist = Vector3.Distance(eFocus.position, target.position);
        Vector3 tMove = new Vector3(eFocus.position.x, eFocus.position.y, eFocus.position.z);
        target.position = tMove;
    }

    private void zTargeting()
    {
        // Update list of in range enemies if count has changed
        GameObject[] enemyArr = GameObject.FindGameObjectsWithTag("Hostiles");
        hnRange = enemyArr.Length;
        int n = 0;
        int enemyCount = enemyArr.Length;
        float shortestDist = 51;
        GameObject[] targetArr = new GameObject[enemyArr.Length];

        if (scanTargets)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                // Check distance of enemy
                other = enemyArr[i].transform;
                float dist = Vector3.Distance(other.position, transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    targetEnemy = enemyArr[i];
                }

                // Check raycast of enemy
                int mask = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11);
                Ray tCheck = new Ray(mCamera.position, Vector3.forward);

                // Check visibility of enemy
                bool enemyVis = enemyArr[i].GetComponent<E_Behavior>().E_Visible;

                if (Physics.Raycast(tCheck, 50, mask) && dist <= 50 && enemyVis == true)
                {
                    targetArr[n] = (enemyArr[i]);
                    hnRange++;
                    n++;
                }
            }
        }

        // Return bool to false to keep from automatically updating targets based on proximity
        if (scanTargets == true)
        {
            scanTargets = false;
        }

        acquireTarget(targetEnemy);

        // Rotate player to face camera direction
        var q = Quaternion.LookRotation(target.position - head.position);
        float angle = 5.0f;
        float speedDeterm = Vector3.Angle(head.position, target.position - head.position);
        if (speedDeterm > 90)
        {
            rotateSpeed = 350.0f;
        }
        else if (speedDeterm > 45 && speedDeterm <= 90)
        {
            rotateSpeed = 215.0f;
        }
        else if (speedDeterm >= angle && speedDeterm <= 45)
        {
            rotateSpeed = 125.0f;
        }
        else if (speedDeterm < angle)
        {
            rotateSpeed = 100.0f;
        }

        head.rotation = Quaternion.RotateTowards(head.rotation, q, rotateSpeed * Time.deltaTime);
    }

    /*void Movement()
    {
        // Determine previous move speed & direction and animate accordingly depending on current input
        // Determine current movement and animate accoringly
        // Determine possible moves/reactions depending on current movement (ie, if jumping can't change direction, etc)
    }*///
}
