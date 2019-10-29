using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 潜伏型の敵クラス
/// </summary>
public class HidingEnemy : EnemyBase, IEnemy
{
    // 自身のMaterial
    private Material myMaterial;

    // 爆破用エフェクト
    [SerializeField, Header("<爆発エフェクト>")]
    private GameObject bombEffect;

    // 時間用
    float time = 0.0f;

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        target = GameObject.Find("Player");
        myRig = GetComponent<Rigidbody2D>();
        myMaterial = GetComponent<SpriteRenderer>().material;
        canAction = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
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
        if (playerDis <= actionRange) StartCoroutine(IntervalAction(0.5f));

    }

    /// <summary>
    /// 爆破までの点滅
    /// </summary>
    IEnumerator ExplosionLight()
    {
        time = 0.0f;

        // 赤色に変更
        while (time < 0.05f)
        {
            time += Time.deltaTime;
            myMaterial.SetVector("_FluidColor", new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
            yield return null;
        }

        time = 0.0f;

        // 黒色に変更
        while (time < 0.05f)
        {
            time += Time.deltaTime;
            myMaterial.SetVector("_FluidColor", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
            yield return null;
        }

        StartCoroutine(ExplosionLight());
    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        // 爆破エフェクトの位置
        var explosionPos = new Vector3(transform.position.x, transform.position.y, -1.0f);
        Instantiate(bombEffect, explosionPos, transform.rotation);

        // 爆発後は消滅
        Destroy(gameObject);
    }

    /// <summary>
    /// アクションのインターバル
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected override IEnumerator IntervalAction(float _interval)
    {
        // アクション状態を切る
        canAction = false;

        // 点滅処理
        StartCoroutine(ExplosionLight());

        // インターバル分待機
        yield return new WaitForSeconds(_interval);

        // 待機後にアクションを行う
        Action();
    }

    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    public void ApplyDamage()
    {
        // 攻撃を受けたら消滅
        Destroy(gameObject);
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">当たったコリジョン</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // プレイヤーの取得
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player == null) return;

        // プレイヤーに当たったら即爆破
        Action();
    }
}
