using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Gimmick;

/// <summary>
/// 障害物クラス
/// </summary>
public class Niedle : GimmickChip
{
    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">当たったコリジョン</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // プレイヤーでないならリターン
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player == null) return;

        // プレイヤーにダメージを与える
        player.ApplyDamage(gameObject, DamageType.Gimmick);
    }
}
