using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{

    public class SinkerEffectGizmo : MonoBehaviour
    {

        public GearActionSinker sinker;
        public Transform child;

        private void OnDrawGizmos()
        {
            //　Cubeのレイを疑似的に視覚化
            Gizmos.color = Color.red;

            Vector3 vec;

            if (sinker.isSide) vec = new Vector3((child.localScale.y * 2) + (child.localScale.y - 2), child.localScale.x);
            else vec = new Vector3(child.localScale.x, (child.localScale.y * 2) + (child.localScale.x - 2));

            Gizmos.DrawWireCube(transform.position, vec);
        }
    }
}
