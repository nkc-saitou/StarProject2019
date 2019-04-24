using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarProject2019.Saitou
{
    /// <summary>
    /// ギミックの動作を実装
    /// </summary>
    public interface IGimmickEffect
    {
        /// <summary>
        /// ギミックの効果発動
        /// </summary>
        /// <param name="_target">効果を与える相手</param>
        void ActiveEffect(GameObject _target);
    }
}