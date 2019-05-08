using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 潜伏型の敵クラス
/// </summary>
public class HidingEnemy : EnemyBase, IEnemy
{

	// Use this for initialization
	void Start () {
        target = GameObject.Find("Player");
        myRig = GetComponent<Rigidbody2D>();
        canAction = true;
	}
	
	// Update is called once per frame
	void Update () {
        CheckAction();
	}

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {
        
    }

    /// <summary>
    /// アクションを起こすかチェック
    /// </summary>
    protected override void CheckAction()
    {
        // ターゲットがいないならリターン
        if (target == null || canAction == false) return;

        // プレイヤーとの距離を検出
        playerDis = CheckDistance(target.transform.position);

        // アクション範囲にいるなら
        if(playerDis <= actionRange)
        {
            Action();
        }

    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        // プレイヤー方向
        Vector2 playerDir = target.transform.position - transform.position;

        // 突進
        myRig.AddForce(playerDir * 100.0f, ForceMode2D.Force);

        canAction = false;

        StartCoroutine(Interval(2.0f));
    }

    /// <summary>
    /// アクションのインターバル
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected override IEnumerator Interval(float _interval)
    {
        yield return new WaitForSeconds(_interval);

        canAction = true;
    }
}
