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
    private Vector2 goalPos;

    // 攻撃用オブジェクト
    [SerializeField]
    private GameObject bombObj;

    // Use this for initialization
    void Start()
    {
        target = GameObject.Find("Player");
        currentPos = transform.position;
        originPos = transform.position;
        targetPos = goalPos;
        canAction = true;

        Move();
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
        StartCoroutine(PatrolMove());
    }

    /// <summary>
    /// 徘徊移動
    /// </summary>
    /// <returns></returns>
    IEnumerator PatrolMove()
    {
        float time = 0.0f;

        while(time <= 1.0f) {
            // 目標地点に移動
            transform.position = Vector2.Lerp(transform.position, originPos + goalPos, time);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0.0f;

        while (time <= 1.0f)
        {
            // 目標地点に移動
            transform.position = Vector2.Lerp(transform.position, originPos, time);
            time += Time.deltaTime;
            yield return null;
        }

        Move();
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
