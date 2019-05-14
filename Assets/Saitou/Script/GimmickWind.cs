using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// 風のギミック
    /// </summary>
    public class GimmickWind : MonoBehaviour,IGimmickEffect
    {
        //--------------------------------
        // private
        //--------------------------------

        [SerializeField] int windPower;
        GameObject _target;

        /// <summary>
        /// 風のギミック動作
        /// </summary>
        public void ActiveEffect()
        {
            // 対象にRigidbodyが無ければギミックの効果を発動させない
            Rigidbody2D _rg = _target.GetComponent<Rigidbody2D>();
            if (_rg == null) return;

            float moveForceMultiplier = 2.0f;
            // 移動制限
            _rg.AddForce(moveForceMultiplier * (((Vector2)transform.up * windPower) - _rg.velocity));
        }

        void OnTriggerStay2D(Collider2D _collision)
        {
            _target = _collision.gameObject;
            ActiveEffect();
        }
    }
}