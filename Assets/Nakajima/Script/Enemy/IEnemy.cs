using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemyインタフェース
/// </summary>
public interface IEnemy
{
    /// <summary>
    /// 移動処理
    /// </summary>
    void Move();

    /// <summary>
    /// Enemyごとのアクション(攻撃処理など)
    /// </summary>
    void Action();

    /// <summary>
    /// ダメージ処理
    /// </summary>
    void ApplyDamage();
}
