using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0.03f, 0, Space.Self);
    }
}
