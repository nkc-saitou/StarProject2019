using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffectGizmo : MonoBehaviour {

    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
