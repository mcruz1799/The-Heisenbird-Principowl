using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform;
    private Vector3 PlayerPosition
    {
        get
        {
            return PlayerTransform.position;
        }
    }
    private float leftFocus;
    public float LeftFocus
    {
        get
        {
            return leftFocus;
        }
        private set
        {
            leftFocus = value;
        }
    }
    //not sure what to make focus length yet
    [SerializeField] private float FocusLength = 10f;
    public float RightFocus
    {
        get
        {
            return LeftFocus + FocusLength;
        }
    }
    private float XDiff = 0.0f;
    private float YDiff = 0.0f;
    private Vector3 CameraPosition
    {
        get
        {
            return transform.position;
        }
        set{transform.position = value;}
    }
    [SerializeField] [Range(1f, 128f)] private int SmoothMovementX = 32;
    // Start is called before the first frame update
    void Start()
    {
        //not sure exactly where to initialize focus yet
        LeftFocus = PlayerPosition.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (PlayerPosition.x > RightFocus || PlayerPosition.x < LeftFocus)
        {
            //calculate the distance between new player position and old camera position
            XDiff = PlayerPosition.x - CameraPosition.x;            
            //move towards new x position with lag 
            Vector3 NewCameraPosition = new Vector3(CameraPosition.x + XDiff, CameraPosition.y, CameraPosition.z);
            StartCoroutine(MoveCameraX(NewCameraPosition,XDiff,SmoothMovementX));
        }
        LeftFocus = CameraPosition.x - FocusLength/2;
        
    }

    private IEnumerator MoveCameraX(Vector3 target, float XDistance, float smooth){
        while(CameraPosition != target)
        {
        yield return new WaitForEndOfFrame();
        Vector3.MoveTowards(CameraPosition, target, XDistance/smooth);
        }  
    }
}
