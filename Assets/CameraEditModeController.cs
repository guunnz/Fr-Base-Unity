using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEditModeController : MonoBehaviour
{
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float acceleration = 0;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered
    [SerializeField] protected Cinemachine.CinemachineVirtualCamera EditModeCamera;
    [SerializeField] public float CameraMoveSpeed = 0.1f;
    // Update is called once per frame

    private void OnEnable()
    {
        EditModeCamera.enabled = true;
    }

    private void OnDisable()
    {
        EditModeCamera.enabled = false;
    }

    void Update()
    {
        if (acceleration > 0)
        {
            if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
            {
                if ((lp.x > fp.x))  //If the movement was to the right
                {
                    EditModeCamera.transform.position = Vector3.Lerp(EditModeCamera.transform.position, new Vector3(EditModeCamera.transform.position.x - (((Mathf.Abs(lp.x - fp.x) / 30) * Time.deltaTime) * acceleration), EditModeCamera.transform.position.y, EditModeCamera.transform.position.z), CameraMoveSpeed);
                }
                else
                {
                    EditModeCamera.transform.position = Vector3.Lerp(EditModeCamera.transform.position, new Vector3(EditModeCamera.transform.position.x + (((Mathf.Abs(lp.x - fp.x) / 30) * Time.deltaTime) * acceleration), EditModeCamera.transform.position.y, EditModeCamera.transform.position.z), CameraMoveSpeed);
                }
            }
            else
            {
                if (lp.y > fp.y)  //If the movement was up
                {
                    EditModeCamera.transform.position = Vector3.Lerp(EditModeCamera.transform.position, new Vector3(EditModeCamera.transform.position.x, EditModeCamera.transform.position.y - (((Mathf.Abs(lp.y - fp.y) / 30) * Time.deltaTime) * acceleration), EditModeCamera.transform.position.z), CameraMoveSpeed);
                }
                else
                {
                    EditModeCamera.transform.position = Vector3.Lerp(EditModeCamera.transform.position, new Vector3(EditModeCamera.transform.position.x, EditModeCamera.transform.position.y + (((Mathf.Abs(lp.y - fp.y) / 30) * Time.deltaTime) * acceleration), EditModeCamera.transform.position.z), CameraMoveSpeed);
                }
            }
        }

        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                acceleration = 0;
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
                Debug.Log("moving");
                Debug.Log(acceleration);
                if (acceleration > 1)
                {
                    acceleration = 1;
                }
                else
                {
                    acceleration += Time.deltaTime * 10;
                }

            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list
            }
        }
        else
        {
            if (acceleration <= 0)
            {
                acceleration = 0;
            }
            else
            {
                acceleration -= Time.deltaTime;
            }
        }
    }
}
