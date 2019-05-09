using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// シーソーが傾いたかどうかを判定
    /// </summary>
    public class SeesawTilt : MonoBehaviour
    {
        //------------------------------------------
        // private
        //------------------------------------------

        [Header("下のあたり判定、左右"),SerializeField]
        GameObject[] _under;

        //------------------------------------------
        // event
        //------------------------------------------

        //傾いた時の処理を登録
        public delegate void TiltHandler(bool isLeft);
        public event TiltHandler tiltHandler;


        void Start()
        {

        }

        void Update()
        {

        }

        void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.gameObject == null) return;

            for (int i = 0; i < _under.Length; i++)
            {
                if (_under[i] == _collision.gameObject)
                {
                    // 倒れている方向が左(0)だったのでtrue
                    if (i == 0) tiltHandler(true);
                    else tiltHandler(false);
                }
            }
        }
    }
}
