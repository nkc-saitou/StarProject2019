﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : EnemyBase, IEnemy
{
    // 現在の位置
    private Vector2 currentPos;
    // 自分が目指す位置
    private Vector2 targetPos;
    // 自身の原点
    private Vector2 originPos;
    // 探索位置
    [SerializeField]
    private Vector2 moveValue;
    // 目標地点
    private Vector2 goalPos;

    // 自身のLineRenderer
    private LineRenderer lineRen;

    // LineRendererの描画地点
    [SerializeField]
    private GameObject startObj;
    private Vector3 endPos;

    // 移動に使う変数
    [SerializeField]
    private float speed;
    float time = 0.0f;

    // Use this for initialization
    void Start()
    {
        lineRen = GetComponent<LineRenderer>();
        target = GameObject.Find("Player");
        currentPos = transform.position;
        originPos = transform.position;
        goalPos = originPos + moveValue;
        targetPos = goalPos;
        canAction = true;
    }
	
	// Update is called once per frame
	void Update () {
        CheckAction();

        Move();
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {
        time += Time.deltaTime;

        // 目標地点に移動
        transform.position = Vector2.Lerp(transform.position, targetPos, time * speed);
        // 現在位置の更新
        currentPos = transform.position;

        // 目標地点に近づいたら行き先変更
        if(Mathf.Abs(currentPos.x - targetPos.x) + Mathf.Abs(currentPos.y - targetPos.y) <= 0.5f) {
            if (targetPos == originPos) targetPos = goalPos;
            else if (targetPos == goalPos) targetPos = originPos;

            time = 0.0f;
        }
    }

    /// <summary>
    /// アクションを起こすかチェック
    /// </summary>
    protected override void CheckAction()
    {
        // ターゲットがいないならリターン
        if (target == null || canAction == false) return;

        // プレイヤーとの距離を検出
        playerDis = CheckDistanceX(target.transform.position);

        // Y軸方向の距離取得
        float offsetY = CheckDistanceY(target.transform.position);
        // ターゲットが上にいる場合はリターン
        if (offsetY >= 0.0f) return;

        // アクション範囲にいるなら
        if (playerDis <= actionRange) Action();
    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        int playerLayer = LayerMask.GetMask("Player", "PlayerFollower");
        int groundLayer = LayerMask.GetMask("Player", "PlayerFollower", "Ignore Raycast");

        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, 500.0f, ~groundLayer);

        endPos = groundHit.point;

        lineRen.SetPosition(0, startObj.transform.position);
        lineRen.SetPosition(1, endPos);

        lineRen.startWidth = 0.2f;
        lineRen.endWidth = 0.2f;

        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, -transform.up, 500.0f, playerLayer);
        if(playerHit.collider != null) {
            var player = playerHit.collider.gameObject.GetComponent<Matsumoto.Character.Player>();
            if (player != null) player.ApplyDamage(gameObject, DamageType.Enemy);
        }
    }

    /// <summary>
    /// アクションのインターバル
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected override IEnumerator IntervalAction(float _interval)
    {
        yield return new WaitForSeconds(_interval);

        canAction = true;
    }

    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    public void ApplyDamage()
    {
        Destroy(gameObject);
    }
}
