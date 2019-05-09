using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵の持つインターフェイス
public interface IEnemy
{
    // 移動
    void Move();

    // アクション(攻撃など)
    void Action();

    // ダメージを受ける
    void ApplyDamage();
}
