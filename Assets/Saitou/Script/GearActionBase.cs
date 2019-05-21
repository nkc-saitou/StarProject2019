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

        [SerializeField]
        protected float _maxRotate = 20.0f;

        [SerializeField]
        protected float _startRotate = 0.0f;

        [SerializeField]
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
            gear.SetMaxAmount(_maxRotate);
            gear.SetStartAmount(_startRotate);
            gear.SetReturnAmount(_returnRotate);

            DoStart();
        }

        void Update()
        {
            GetRotate = gear.RotationAmount;

            DoUpdate();
        }
    }
}
