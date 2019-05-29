using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerGoalChip : GimmickChip {

		public Transform ToPlayerAnchor;
		public SpriteMask MaskSprite;

		private Animator _gateAnimator;
		private PlayerCamera _playerCamera;

		// Use this for initialization
		void Start() {
			_gateAnimator = GetComponentInChildren<Animator>();
			_playerCamera = FindObjectOfType<PlayerCamera>();

			MaskSprite.enabled = false;
		}

		// Update is called once per frame
		void Update() {

		}

		private void OnTriggerEnter2D(Collider2D collision) {

			var player = collision.GetComponent<Player>();
			if(!player) return;
			StartCoroutine(GoalAnim(player));
		}

		private IEnumerator GoalAnim(Player player) {
			
			// プレイヤーを止める
			player.Stop();

			// スピードを設定して開けさせる
			_gateAnimator.SetFloat("DoorSpeed", 1.0f);

			// 開くまで待つ
			var ratio = 0.0f;
			while (ratio < 1.0f) {
				ratio = _gateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				yield return null;
			}

			MaskSprite.enabled = true;

			_playerCamera.IsFreeze = true;

			// プレイヤーの移動
			var pos = ToPlayerAnchor.position - player.transform.position;
			Debug.Log(pos);
			pos.y = 0.0f;
			yield return StartCoroutine(player.MoveTo(pos));

			Controller.GameClear();
		}

	}
}
