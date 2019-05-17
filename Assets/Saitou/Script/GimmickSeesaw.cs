using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// シーソーがどちらに傾いているか
    /// </summary>
    public enum SeesawState
    {
        left,
        right,
    }

    public class GimmickSeesaw : MonoBehaviour, IGimmickEffect
    {
        //-----------------------------------------
        // private
        //-----------------------------------------

        [SerializeField,Header("シーソーに乗っているオブジェクトを取得する,左右")]
        SeesawRide[] _rideArray;

        [SerializeField] SeesawTilt _seesawTilt;

        SeesawState _seesawState;

        //-----------------------------------------
        // 関数
        //-----------------------------------------

        // Use this for initialization
        void Start()
        {
            _seesawTilt.tiltHandler += (bool isLeft) =>
            {
                _seesawState = (isLeft) ? SeesawState.left : SeesawState.right;
                ActiveEffect();
            };
        }

        public void ActiveEffect()
        {
            // 左右どちらに乗っているオブジェクトを飛ばすか、配列添字番号を取得する
            // 上に飛ばすオブジェクトは、現在傾いている方とは反対の方に乗っている
            int _flyObjIndex = 
                (_seesawState == SeesawState.left) ? (int)SeesawState.right : (int)SeesawState.left;

            Debug.Log(_rideArray[_flyObjIndex].RideObject.Count);
            // 乗っているオブジェクトがなければ終了
            if (_rideArray[_flyObjIndex].RideObject.Count == 0) return;


            for (int i = 0; i < _rideArray[_flyObjIndex].RideObject.Count; i++)
            {
                Rigidbody2D rg = _rideArray[_flyObjIndex].RideObject[i].GetComponent<Rigidbody2D>();

                // 移動制限
                rg.AddForce(((Vector2)transform.up * 100.0f) - rg.velocity);
            }
        }
    }
}
