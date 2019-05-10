using System.Collections;
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

    private Vector2 goalPos;

    // 攻撃用オブジェクト
    [SerializeField]
    private GameObject bombObj;

    // 移動に使う変数
    float time = 0.0f;

    // Use this for initialization
    void Start()
    {
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
        transform.position = Vector2.Lerp(transform.position, targetPos, time * Time.deltaTime);
        // 現在位置の更新
        currentPos = transform.position;

        // 目標地点に近づいたら行き先変更
        if(Mathf.Abs(currentPos.x - targetPos.x) <= 0.5f) {
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
        Instantiate(bombObj, transform.position - new Vector3(0.0f, 1.1f), Quaternion.identity);

        canAction = false;

        StartCoroutine(IntervalAction(2.0f));
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
