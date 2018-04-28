﻿using UnityEngine;
using System.Collections;

public class Camera_Handler : MonoBehaviour
{

    private static readonly float PanSpeed = 20f;
    private static readonly float ZoomSpeedTouch = 0.2f;
    private static readonly float ZoomSpeedMouse = 0.5f;

    public static float[] BoundsX = new float[] { 5f, 35f };
    public static float[] BoundsZ = new float[] { -50f, -5f };
    public static float[] ZoomBounds = new float[] { 10f, 70f };

    public static readonly float[] BoundsXBottom = new float[] { 5f, 35f };
    public static readonly float[] BoundsZBottom = new float[] { -50f, -5f };
    public static readonly float[] BoundsXRight = new float[] { 5f, 35f };
    public static readonly float[] BoundsZRight = new float[] { -50f, -5f };
    public static readonly float[] BoundsXTop = new float[] { 5f, 35f };
    public static readonly float[] BoundsZTop = new float[] { -50f, -5f };
    public static readonly float[] BoundsXLeft = new float[] { 5f, 35f };
    public static readonly float[] BoundsZLeft = new float[] { -50f, -5f };

    public Camera cam;
    public GameObject plane;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only

    void Awake()
    {
        cam = cam.GetComponentInParent<Camera>();
    }

    void Update()
    {
        if (Globals.paintAllowed || !Globals.panningAllowed)
        {
            return;
        }
        // Disabled until I can sort the rotation issues out.
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch()
    {
        switch (Input.touchCount)
        {

            /*case 1: // Panning
                Debug.Log("In Panning");
                wasZoomingLastFrame = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;
                */
            case 2: // Zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        Debug.Log("In PanCamera()");
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);

        // Perform the movement
        transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }
}