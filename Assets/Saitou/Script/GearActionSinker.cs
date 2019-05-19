using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{

    public class GearActionSinker : GearActionBase
    {

        const float MAX_POS = 3.0f;

        float _posAmount;

        protected override void DoStart()
        {
            _posAmount = MAX_POS * 2;

            gear.Permission = GearPermission.Right;
        }

        protected override void DoUpdate()
        {
            ActiveEffect();
        }

        public override void ActiveEffect()
        {
            transform.position = 
                new Vector2(transform.position.x, (GetRotate / _posAmount));
        }
    }
}
