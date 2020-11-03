using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var t = transform;
        var p = t.position;
        
        Gizmos.DrawRay(p, t.forward);
        Gizmos.DrawWireSphere(p, 0.25f);
    }
}
