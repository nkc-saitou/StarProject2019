using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // ターゲット
    protected GameObject target;

    // 自身のRigitbody
    protected Rigidbody2D myRig;

    // アクションの範囲(攻撃範囲)
    [SerializeField]
    protected float actionRange;
    // アクションを起こせるかどうか
    protected bool canAction;
    // プレイヤーとの距離
    protected float playerDis;

    /// <summary>
    /// 目標との距離を返す
    /// </summary>
    /// <param name="_targetPos">目標地点の座標</param>
    /// <returns></returns>
    protected float CheckDistance(Vector2 _targetPos)
    {
        // 目標座標との距離の検出
        float _distance = Vector2.Distance(_targetPos, transform.position);

        // 距離を返す
        return _distance;
    }

    // アクションを起こすかチェック
    protected virtual void CheckAction() { }

    /// <summary>
    /// インターバルを使ってなにかする用
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected virtual IEnumerator Interval(float _interval) { yield return null; }
}
