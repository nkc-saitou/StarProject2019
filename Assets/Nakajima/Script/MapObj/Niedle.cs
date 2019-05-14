using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Niedle : MonoBehaviour
{
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
        // プレイヤーでないならリターン
        var player = col.gameObject.GetComponent<Matsumoto.Character.Player>();
        if (player == null) return;
        

    }
}
