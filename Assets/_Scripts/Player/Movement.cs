using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class Movement : MonoBehaviour
{
    [Header("WSAD")]
    public float speed = 5f;
    public float sprintSpeedMultiplier = 1.5f;
    public float smoothWsadTime = 0.1f;
    [Header("Drag")]
    public float dragSpeed = 10f;
    public float sprintSpeedDragMultiplier = 1.5f;
    public float smoothDragTime = 0.1f;
    [Header("Zooming")]
    public float zoomSpeed = 10f;
    //public float zoomMoveSpeed = 5f;
    public float zoomSensitivity = 5f;
    public float zoomSmoothTime = 0.1f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Vector2 movement;
    private Vector3 mouseDragPos;

    [Header("Smoothed velocities")]
    private Vector3 wsadSmoothVelocity;
    private Vector3 dragSmoothVelocity;

    [Header("Velocity references")]
    private Vector3 wsadVelocityRef;
    private Vector3 dragVelocityRef;

    [Header("Target velocities")]
    private Vector3 targetWsadVelocity;
    private Vector3 targetDragVelocity;

    private Vector3 currentVelocity;

    [Header("Zoom")]
    private float targetZoom;
    private Vector3 zoomMouseScreenPos;
    private Vector3 beforeZoom;
    private Vector3 afterZoom;
    Vector3 offset;

    private void Start()
    {
        targetZoom = Camera.main.orthographicSize;
        maxZoom = Mathf.Clamp(maxZoom, minZoom, (TileManager.Instance.bounds.size.x-2) / (2f * Camera.main.aspect));
    }

    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float scroll = Input.mouseScrollDelta.y;

            targetZoom -= scroll * zoomSensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            zoomMouseScreenPos = Input.mousePosition;
            beforeZoom = Camera.main.ScreenToWorldPoint(zoomMouseScreenPos);

        }

        if (Mathf.Abs(Camera.main.orthographicSize - targetZoom) > 0.1f)
        {
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, zoomSmoothTime * Time.deltaTime * Mathf.Abs(Camera.main.orthographicSize - targetZoom));

            beforeZoom += currentVelocity * Time.deltaTime;
            afterZoom = Camera.main.ScreenToWorldPoint(zoomMouseScreenPos);

            offset = beforeZoom - afterZoom;
        }
        else if (offset.magnitude != 0)
            offset = Vector3.zero;

        if (Input.GetMouseButtonDown(2))
        {
            //mouse to world pos
            mouseDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mouseDragPos - mousePos);

            float distance = direction.magnitude;

            direction.Normalize();

            float currentDragSpeed = dragSpeed * distance * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeedDragMultiplier : 1f);

            targetDragVelocity = direction * currentDragSpeed;
        }
        else
        {
            if(targetDragVelocity.magnitude > 0.01f)
                targetDragVelocity *= Mathf.Exp(-10f * Time.deltaTime);
            else
                targetDragVelocity = Vector3.zero;
        }

        movement.x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        movement.y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);

        movement.Normalize();

        float currentSpeed = speed * Mathf.Sqrt(Camera.main.orthographicSize / minZoom) * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeedMultiplier : 1f);

        targetWsadVelocity = movement * currentSpeed;



        wsadSmoothVelocity = Vector3.SmoothDamp(wsadSmoothVelocity, targetWsadVelocity, ref wsadVelocityRef, smoothWsadTime);

        dragSmoothVelocity = Vector3.SmoothDamp(dragSmoothVelocity, targetDragVelocity, ref dragVelocityRef, smoothDragTime);

        // smooth combined movement

        currentVelocity = wsadSmoothVelocity + dragSmoothVelocity;

        Vector3 targetCamPosition = transform.position + currentVelocity * Time.deltaTime;
        TileManager tm = TileManager.Instance;

        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;

        targetCamPosition += offset;
        targetCamPosition = new Vector3(Mathf.Clamp(targetCamPosition.x, tm.bounds.xMin+halfWidth+1, tm.bounds.xMax-halfWidth-1), Mathf.Clamp(targetCamPosition.y, tm.bounds.yMin+halfHeight+1, tm.bounds.yMax-halfHeight-1), 0);

        transform.position = targetCamPosition;
    }
}