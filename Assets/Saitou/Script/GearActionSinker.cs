using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    public class GearActionSinker : GearActionBase
    {
        public float PosAmount { get; private set; }

        [Header("横方向")]
        public bool isSide;

        Vector2 moveVec;

        Vector2 startVec;

        protected override void DoStart()
        {
            startVec = transform.position;

            PosAmount = _maxRotate;

            gear.Permission = GearPermission.All;
        }

        protected override void DoUpdate()
        {
            ActiveEffect();
        }

        public override void ActiveEffect()
        {
            if (isSide) moveVec = new Vector2(GetRotate, 0);
            else　moveVec = new Vector2(0, GetRotate);

            transform.position = startVec + moveVec;
        }
    }
}
