using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoll : MonoBehaviour
{
    public float leftEnd, rightEnd;
    public float maxSpeed, minDistanceToEdge;

    private Camera cam;
    private bool shouldPan = false;
    private Bounds b;
    private Vector2 mousePosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        if(leftEnd > rightEnd)
        {
            var t = leftEnd;
            leftEnd = rightEnd;
            rightEnd = t;
        }
    }

    void CheckIfIShouldPan()
    {
        FindMousePosition();
        b.center = transform.position;
        b.min = cam.ViewportToWorldPoint(Vector2.zero);
        b.max = cam.ViewportToWorldPoint(Vector2.one);
        var e = b.extents;
        e.x -= minDistanceToEdge;
        b.extents = e;
        shouldPan = !(b.Contains(mousePosition));
    }

    void FindMousePosition()
    {
        mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update()
    {
        var hor = Input.GetAxis("Horizontal");
        if (hor != 0)
        {
            var md = maxSpeed * Time.deltaTime;
            var cx = transform.position.x;
            var end = Mathf.Sign(hor) == 1 ? rightEnd : leftEnd;
            var shouldPan = Mathf.Sign(hor) == 1 ? !(cx + md > end) : !(cx - md < end);
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) md *= 2f;
            if (shouldPan) transform.position += new Vector3(md * hor, 0);
            return;
        }
        CheckIfIShouldPan();
        if (shouldPan)
        {
            var mx = mousePosition.x;
            var md = maxSpeed * Time.deltaTime;
            var cx = transform.position.x;
            var dir = mx > cx ? 1 : -1;
            var end = dir == 1 ? rightEnd : leftEnd;
            var shouldPan = dir == 1 ? !(cx + md > end) : !(cx - md < end);
            if (Input.GetMouseButton(0)) md *= 2f;
            if (shouldPan) transform.position += new Vector3(md * dir, 0);
        }
    }
}
