using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    public abstract class GearActionBase : MonoBehaviour,IGimmickEffect
    {
        //------------------------------------------
        // protected
        //------------------------------------------
        [SerializeField]
        protected GimmickGear gear;

        [SerializeField, Header("回転を戻す方向")]
        protected GearPermission permission;

        [Header("最大回転")]
        public float _maxRotate;

        [SerializeField,Header("回転量がたまるスピードを調整"),Range(0.1f,1.0f)]
        protected float _amountRotateSpeed = 0.0f;

        protected float _startRotate = 0.0f;

        protected float _returnRotate = 0.0f;

        protected float GetRotate { get; private set; }

        //------------------------------------------
        // 抽象メソッド
        //------------------------------------------

        public abstract void ActiveEffect();
        protected abstract void DoStart();
        protected abstract void DoUpdate();

        //------------------------------------------
        // 関数
        //------------------------------------------

        void Start()
        {
            DoStart();

            SetAllRotateValue();

            gear.MaxRotateAmount = _maxRotate;
            gear.RotationAmount = _startRotate;
            gear.ReturnRotateAmount = _returnRotate;
            gear.RotateAmountSpeed = _amountRotateSpeed;
        }

        void Update()
        {
            GetRotate = gear.RotationAmount;

            DoUpdate();
        }

        void SetAllRotateValue()
        {
            switch(permission)
            {
                case GearPermission.Right:
                    _startRotate = _maxRotate;
                    _returnRotate = _maxRotate;
                    break;

                case GearPermission.Left:
                    _startRotate = -_maxRotate;
                    _returnRotate = -_maxRotate;
                    break;

                case GearPermission.All:
                    _startRotate = 0;
                    _returnRotate = 0;
                    break;
            }
        }
    }
}
