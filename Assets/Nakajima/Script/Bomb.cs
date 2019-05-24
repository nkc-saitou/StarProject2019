using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 爆破用エフェクト
    [SerializeField]
    private GameObject bombEffect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="col">当たったコリジョン</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        Instantiate(bombEffect, transform.position, transform.rotation);

        // Playerの取得
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player != null) {
            // プレイヤーにダメージを与える
            player.ApplyDamage(gameObject, DamageType.Gimmick);
        }

        Destroy(gameObject);
    }
}
