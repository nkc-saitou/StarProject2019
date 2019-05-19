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

    /// <summary>
    /// 回転許可
    /// </summary>
    public enum GearPermission
    {
        Right, // 右回転のみ許可
        Left,　// 左回転のみ許可
        All,   // どちらの回転も許可 
    }

    public class GimmickGear : MonoBehaviour
    {
        //------------------------------------------
        // private
        //------------------------------------------

        Animator _animator;
        Player _player;

        bool isExit = true;

        //------------------------------------------
        // プロパティ
        //------------------------------------------

        /// <summary>
        ///  回転速度
        /// </summary>
        public float RollSpeed { get; private set; }

        public float RotationAmount { get; private set; }

        /// <summary>
        /// Max回転量を与える
        /// </summary>
        public float MaxRotateAmount { get; private set; } = 20;

        /// <summary>
        /// 戻したい回転位置をセット
        /// </summary>
        public float ReturnRotateAmount { get; set; } = 0;

        /// <summary>
        /// ギアの状態
        /// </summary>
        public GearState State { get; private set; }

        /// <summary>
        /// 現在の回転方向
        /// </summary>
        public GearRotateDir RotateDir { get; private set; }

        /// <summary>
        /// 回転の許可
        /// </summary>
        public GearPermission Permission { get; set; }

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

        public void SetStartAmount(float amount)
        {
            RotationAmount = amount;
        }

        public void SetMaxAmount(float amount)
        {
            MaxRotateAmount = amount;
        }

        public void SetReturnAmount(float amount)
        {
            ReturnRotateAmount = amount;
        }

        /// <summary>
        /// 回転
        /// </summary>
        void GearRotate()
        {
            // 現在の回転量がMaxで指定してあった回転量を上回る場合は処理をしない
            if ((IsRotate() == false || _player.State != PlayerState.Star) && isExit == false)
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
                RotationAmount += RollSpeed * Time.deltaTime * (int)RotateDir;
            }

        }

        /// <summary>
        /// 歯車が巻き戻っている
        /// </summary>
        void GearReturn()
        {
            if (RotationAmount == ReturnRotateAmount || State == GearState.Roll) return;

            // 右方向に回していた場合
            if (RotationAmount > ReturnRotateAmount &&
                (Permission == GearPermission.Right || Permission == GearPermission.All))
            {
                RotationAmount -= Time.deltaTime;
                _animator.SetFloat("Speed", -0.1f);

                // 値がマイナスになったら値を０に
                if (RotationAmount < ReturnRotateAmount)
                    RotationAmount = ReturnRotateAmount;
            }
            // 左方向に回していた場合
            else if(RotationAmount < ReturnRotateAmount &&
                (Permission == GearPermission.Left || Permission == GearPermission.All))
            {
                RotationAmount += Time.deltaTime;
                _animator.SetFloat("Speed", 0.1f);

                // 値がプラスになったら値を０
                if (RotationAmount > ReturnRotateAmount)
                    RotationAmount = ReturnRotateAmount;
            }
        }

        /// <summary>
        /// 回すことが出来るかどうか
        /// </summary>
        /// <returns></returns>
        bool IsRotate()
        {
            if (RotationAmount > MaxRotateAmount && RotateDir == GearRotateDir.Right) return false;
            if (RotationAmount < -MaxRotateAmount && RotateDir == GearRotateDir.Left) return false;
            else return true;
        }

        private void OnCollisionStay2D(Collision2D _collision)
        {
            // 当たっているのがプレイヤー以外であれば処理しない
            if (_collision.gameObject != _player.gameObject) return;

            isExit = false;

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
            isExit = true;
        }
    }
}
