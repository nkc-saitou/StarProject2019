using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 潜伏型の敵クラス
/// </summary>
public class HidingEnemy : EnemyBase, IEnemy
{
    // 自身のSpriteRenderer
    private Material myMaterial;

    // 爆破用エフェクト
    [SerializeField]
    private GameObject bombEffect;

    // 時間用
    float time = 0.0f;
    // 透過処理
    float alpha = 0.0f;

    // Use this for initialization
    void Start()
    {
        target = GameObject.Find("Player");
        myRig = GetComponent<Rigidbody2D>();
        myMaterial = GetComponent<SpriteRenderer>().material;
        canAction = true;
    }

    // Update is called once per frame
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
        if (playerDis <= actionRange) StartCoroutine(IntervalAction(2.0f));

    }

    /// <summary>
    /// 爆破までの点滅
    /// </summary>
    IEnumerator ExplosionLight()
    {
        time = 0.0f;

        // RendererのColorを取得
        //var color = myMaterial.SetVector("FuildColor", new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
        // アルファフェードインアウト
        while (time < 0.5f) {
            time += Time.deltaTime;
            myMaterial.SetVector("_FluidColor", new Vector4(Mathf.Lerp(myMaterial.GetVector("_FluidColor").x, 0.0f, time * 100.0f * Time.deltaTime), 0.0f, 0.0f, 0.0f));
            yield return null;
        }

        time = 0.0f;

        while (time < 0.5f) {
            time += Time.deltaTime;
            myMaterial.SetVector("_FluidColor", new Vector4(Mathf.Lerp(myMaterial.GetVector("_FluidColor").x, 1.0f, time * 100.0f * Time.deltaTime), 0.0f, 0.0f, 0.0f));
            yield return null;
        }

        StartCoroutine(ExplosionLight());
    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        Instantiate(bombEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    /// <summary>
    /// アクションのインターバル
    /// </summary>
    /// <param name="_interval">待機時間</param>
    /// <returns></returns>
    protected override IEnumerator IntervalAction(float _interval)
    {
        canAction = false;

        StartCoroutine(ExplosionLight());

        yield return new WaitForSeconds(_interval);

        Action();
    }

    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    public void ApplyDamage()
    {
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
