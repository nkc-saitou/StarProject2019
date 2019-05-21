using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

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

        [SerializeField] Transform endPos;

        GameObject _target;

        Player _player;

        [SerializeField] ParticleSystem particle;

        ParticleSystem.ShapeModule particleShape;

        //--------------------------------
        // 関数
        //--------------------------------

        void Start()
        {
            _player = FindObjectOfType<Player>();

            particleShape = particle.shape;

            Vector3 size = new Vector3(transform.localScale.y * transform.localScale.y, transform.localScale.y);

            particleShape.scale = size;
        }

        void Update()
        {
            Vector2 vec = new Vector2(transform.position.x + 0.7f, transform.position.y);

            float dis = Vector2.Distance(transform.position, endPos.position);

            //メインカメラ上のマウスカーソルのある位置からRayを飛ばす
            Ray2D ray =  new Ray2D(vec,transform.up);

            int layerMask = ~(1 << 8 | 1 << 9);

            RaycastHit2D hit = Physics2D.BoxCast(ray.origin,transform.localScale,0.0f, ray.direction,dis,layerMask);

            //なにかと衝突した時だけそのオブジェクトの名前をログに出す
            if (hit.collider)
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.DrawLine(transform.position, hit.collider.transform.position, Color.green);
                _target = hit.collider.gameObject;

                ActiveEffect();
            }
        }

        public void ActiveEffect()
        {
            if (_player.gameObject != _target) return;
            if (_player.State != PlayerState.Circle) return;

            Rigidbody2D _rg = _target.GetComponent<Rigidbody2D>();

            if (_rg == null) return;

            float moveForceMultiplier = 2.0f;
            _rg.AddForce(moveForceMultiplier * (((Vector2)transform.up * windPower) - _rg.velocity));

        }

        ///// <summary>
        ///// 風のギミック動作
        ///// </summary>
        //public void ActiveEffect()
        //{
        //    if (_player.State != PlayerState.Circle) return;
        //    // 対象にRigidbodyが無ければギミックの効果を発動させない
        //    Rigidbody2D _rg = _target.GetComponent<Rigidbody2D>();
        //    if (_rg == null) return;

        //    float moveForceMultiplier = 2.0f;
        //    // 移動制限
        //    _rg.AddForce(moveForceMultiplier * (((Vector2)transform.up * windPower) - _rg.velocity));
        //}

        //void OnTriggerStay2D(Collider2D _collision)
        //{
        //    _target = _collision.gameObject;

        //    ActiveEffect();
        //}
    }
}