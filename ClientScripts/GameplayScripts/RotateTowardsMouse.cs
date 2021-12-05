using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Vector3 prevMousePos;
    [SerializeField] GameObject firePoint;
    private Vector3 worldPos;
    void Update()
    {
      

        Vector3 mousePos = Input.mousePosition;
        if (Vector3.Distance(prevMousePos, mousePos)<31)
        {
            return;
        }
        mousePos.z = -Camera.main.transform.position.z;

        worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.LookAt(worldPos);
        prevMousePos = mousePos;
    }
    public Vector3 GetShotDirection()
    {
        Vector3 firePointPos = firePoint.transform.position;
        firePointPos += firePoint.transform.forward * 2;
        return firePointPos-transform.position;
    }
    public Quaternion GetRotation()
    {
        return this.transform.rotation;
    }
}
  



