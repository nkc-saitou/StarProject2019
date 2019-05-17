using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// ギアの状態
    /// </summary>
    public enum GearState
    {
        Free,     // 何も行われていない状態
        Lock,     // 回せない状態
        Roll,
        Return    // 巻きが戻っている状態
    }

    /// <summary>
    /// 回転方向
    /// </summary>
    public enum GearRotateDir
    {
        Left = -1,
        Right = 1,
    }

    public class GimmickGear : MonoBehaviour
    {
        //------------------------------------------
        // private
        //------------------------------------------

        Animator _animator;
        Player _player;

        // 回転量
        float _rotationAmount = 0;

        //------------------------------------------
        // プロパティ
        //------------------------------------------

        /// <summary>
        ///  回転速度
        /// </summary>
        public float RollSpeed
        {
            get; private set;
        }

        /// <summary>
        /// Max回転量を与える
        /// </summary>
        public float MaxRotateAmount { get; set; } = 20;

        /// <summary>
        /// ギアの状態
        /// </summary>
        public GearState State
        {
            get; private set;
        }

        public GearRotateDir RotateDir
        {
            get; private set;
        }


        //------------------------------------------
        // 関数
        //------------------------------------------

        void Start()
        {
            _animator = transform.GetChild(0).GetComponent<Animator>();
            _player = FindObjectOfType<Player>();
        }

        void Update()
        {
            GearRotate();
            GearReturn();
        }

        /// <summary>
        /// 回転
        /// </summary>
        void GearRotate()
        {
            // 現在の回転量がMaxで指定してあった回転量を上回る場合は処理をしない
            if (IsRotate() == false || _player.State != PlayerState.Star)
            {
                _player.IsRotate = true;
                // アニメーション
                _animator.SetFloat("Speed", 0);
                return;
            }

            _player.IsRotate = false;

            // 回転速度
            RollSpeed = (_player.RollSpeed < 0 ? _player.RollSpeed * -1 : _player.RollSpeed);

            // アニメーション
            _animator.SetFloat("Speed", State == GearState.Roll ? RollSpeed / 2 * (int)RotateDir : 0);

            // 回転量
            if (State == GearState.Roll)
            {
                _rotationAmount += RollSpeed * Time.deltaTime * (int)RotateDir;
            }

        }

        /// <summary>
        /// 歯車が巻き戻っている
        /// </summary>
        void GearReturn()
        {
            if (_rotationAmount == 0 || State == GearState.Roll) return;

            // 右方向に回していた場合
            if (_rotationAmount > 0)
            {
                _rotationAmount -= Time.deltaTime;
                _animator.SetFloat("Speed", -0.1f);

                // 値がマイナスになったら値を０に
                if (_rotationAmount < 0) _rotationAmount = 0;
            }
            // 左方向に回していた場合
            else if(_rotationAmount < 0)
            {
                _rotationAmount += Time.deltaTime;
                _animator.SetFloat("Speed", 0.1f);

                // 値がプラスになったら値を０
                if (_rotationAmount > 0) _rotationAmount = 0;
            }
        }

        /// <summary>
        /// 回すことが出来るかどうか
        /// </summary>
        /// <returns></returns>
        bool IsRotate()
        {
            if (_rotationAmount >= MaxRotateAmount && RotateDir == GearRotateDir.Right) return false;
            if (_rotationAmount <= -MaxRotateAmount && RotateDir == GearRotateDir.Left) return false;
            else return true;
        }

        private void OnCollisionStay2D(Collision2D _collision)
        {
            // 当たっているのがプレイヤー以外であれば処理しない
            if (_collision.gameObject != _player.gameObject) return;

            // 移動方向(プレイヤーの回転と反対方向に回転)
            if (_player.RollSpeed != 0)
                RotateDir = (_player.RollSpeed > 0 ? GearRotateDir.Left : GearRotateDir.Right);

            if (_player.RollSpeed != 0) State = GearState.Roll;
            else State = GearState.Free;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            State = GearState.Free;

            _player.IsRotate = false;
        }
    }
}
