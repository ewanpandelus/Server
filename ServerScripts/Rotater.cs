using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public void SetLookAt(Vector3 _worldPos)
    {
        transform.LookAt(_worldPos);
    }
}
