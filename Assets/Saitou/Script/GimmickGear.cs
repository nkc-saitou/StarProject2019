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

        // 判定範囲から外れていたらtrue
        bool isExit = true;

        bool isFirstStop = true;

        float dis = 0;

        //------------------------------------------
        // プロパティ
        //------------------------------------------

        /// <summary>
        ///  回転速度
        /// </summary>
        public float RollSpeed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float RotationAmount { get; set; }

        /// <summary>
        /// Max回転量を与える
        /// </summary>
        public float MaxRotateAmount { get; set; }

        /// <summary>
        /// 戻したい回転位置をセット
        /// </summary>
        public float ReturnRotateAmount { get; set; }

        /// <summary>
        /// 回転量のたまるスピードを調整
        /// </summary>
        public float RotateAmountSpeed { get; set; }

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
            CheckDistance();

            GearRotate();
            GearReturn();

            PlayerRotateControl();
        }

        /// <summary>
        /// 回転
        /// </summary>
        void GearRotate()
        {
            if (isExit != false) return;

            // 現在の回転量がMaxで指定してあった回転量を上回る場合は処理をしない
            if ((IsRotate() == false || _player.State != PlayerState.Star))
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
                RotationAmount += RollSpeed * Time.deltaTime * (int)RotateDir * RotateAmountSpeed;
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
                RotationAmount -= Time.deltaTime * 0.5f;
                _animator.SetFloat("Speed", -0.1f);

                // 値がマイナスになったら値を０に
                if (RotationAmount < ReturnRotateAmount)
                    RotationAmount = ReturnRotateAmount;
            }
            // 左方向に回していた場合
            else if(RotationAmount < ReturnRotateAmount &&
                (Permission == GearPermission.Left || Permission == GearPermission.All))
            {
                RotationAmount += Time.deltaTime * 0.5f;
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

        void CheckDistance()
        {
            dis = Vector2.Distance(_player.transform.position, transform.position);
        }

        void PlayerRotateControl()
        {
            // 歯車と近づいていたら
            if (dis <= 0.9f && isFirstStop == false)
            {
                // 近づいているのでfalse
                isExit = false;
                // 一度しか判定しないように調整するためのflg
                isFirstStop = true;

                if (_player.RollSpeed != 0)
                    RotateDir = (_player.RollSpeed > 0 ? GearRotateDir.Left : GearRotateDir.Right);

                if (_player.RollSpeed != 0) State = GearState.Roll;
                else State = GearState.Free;
            }
            // 一定以上離れたら関与しない
            else if(dis > 0.9f && dis < 1.2f && isFirstStop == true)
            {
                // 離れたのでtrue
                isExit = true;
                isFirstStop = false;

                State = GearState.Free;
                _player.IsRotate = false;
            }
        }
    }
}
