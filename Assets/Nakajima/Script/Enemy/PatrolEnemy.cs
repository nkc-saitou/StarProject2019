using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : EnemyBase, IEnemy
{
    // EnemyのRenderer
    [SerializeField]
    private SpriteRenderer mySprite;
    // カメラに範囲に存在するか
    private bool visible;

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

    // レーザー用のパーティクル
    [SerializeField]
    private ParticleSystem razerCore;
    [SerializeField]
    private ParticleSystem razerHit;

    // 移動に使う変数
    [SerializeField]
    private float speed;
    float time = 0.0f;

    // Use this for initialization
    void Start()
    {
        lineRen = GetComponent<LineRenderer>();
        lineRen.enabled = false;
        target = FindObjectOfType<Matsumoto.Character.Player>().gameObject;
        currentPos = transform.position;
        originPos = transform.position;
        goalPos = originPos + moveValue;
        targetPos = goalPos;
        canAction = true;
    }
	
	// Update is called once per frame
	void Update () {
        // カメラに写っているか判定
        if (mySprite.isVisible) visible = true;
        else visible = false;

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
        // ターゲットがいない、アクション不可能、カメラ範囲外ならリターン
        if (target == null || canAction == false) return;

        // プレイヤーとの距離を検出
        playerDis = CheckDistanceX(target.transform.position);

        // Y軸方向の距離取得
        float offsetY = CheckDistanceY(target.transform.position);
        // ターゲットが上にいる場合はリターン
        if (offsetY >= 0) return;

        // アクション範囲にいるなら
        if (playerDis <= actionRange) Action();
    }

    /// <summary>
    /// アクション(攻撃など)
    /// </summary>
    public void Action()
    {
        // プレイヤー判定用レイヤー
        int playerLayer = LayerMask.GetMask("Player", "PlayerFollower");
        // 地面判定用レイヤー
        int groundLayer = LayerMask.GetMask("Player", "PlayerFollower", "Ignore Raycast");

        // LineRendererをアクティブにする
        if (lineRen.enabled == false && visible == true) {
            // 音のなる位置を定義
            var SoundPos = new Vector3(transform.position.x, target.transform.position.y, 0.0f);
            Matsumoto.Audio.AudioManager.PlaySE("Laser", position:SoundPos);

            SetLineRenderer(lineRen.enabled);
            StartCoroutine(IntervalAction(2.0f));
        }

        // 地面判定
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, 500.0f, ~groundLayer);
        // 地面の地点取得
        endPos = groundHit.point;
        razerHit.transform.position = endPos;

        // Lineの位置を定義
        lineRen.SetPosition(0, startObj.transform.position);
        lineRen.SetPosition(1, endPos);
        // Lineの長さを定義
        lineRen.startWidth = 0.1f;
        lineRen.endWidth = 0.1f;

        if (lineRen.enabled == true) {
            // プレイヤー判定
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, -transform.up, 500.0f, playerLayer);
            if (playerHit.collider != null)
            {
                var player = playerHit.collider.gameObject.GetComponent<Matsumoto.Character.Player>();
                if (player != null) player.ApplyDamage(gameObject, DamageType.Enemy);
            }
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

        // 次のアクションまでのインターバルを設ける
        if (canAction == true) {
            canAction = false;
            StartCoroutine(IntervalAction(1.0f));
        }
        else canAction = true;

        // 描画されているLineRendererを非表示に変更
        if (lineRen.enabled == true) SetLineRenderer(lineRen.enabled);
    }

    /// <summary>
    /// LineRendererのアクティブ設定
    /// </summary>
    /// <param name="enabled">現在の状態</param>
    private void SetLineRenderer(bool enabled)
    {
        // エフェクトの停止
        if (enabled) {
            razerCore.gameObject.SetActive(!enabled);
            razerHit.Stop();
        }
        // エフェクトの再生
        else {
            razerCore.gameObject.SetActive(!enabled);
            razerHit.Play();
        }

        // 非アクティブならアクティブ化、アクティブなら非アクティブ化
        lineRen.enabled = !enabled;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">当たったコリジョン</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // Playerの取得
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player == null) return;

        // ダメージを与える
        player.ApplyDamage(gameObject, DamageType.Enemy);
    }

    /// <summary>
    /// ダメージを受けた際の処理
    /// </summary>
    public void ApplyDamage()
    {
        Destroy(gameObject);
    }
}
