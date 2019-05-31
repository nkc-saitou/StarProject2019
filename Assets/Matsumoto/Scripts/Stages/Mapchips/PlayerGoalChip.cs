using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerGoalChip : GimmickChip {

		public Transform ToPlayerAnchor;
		public SpriteMask MaskSprite;
		public SpriteRenderer[] GateRenderers;

		private Animator _gateAnimator;
		private Material _gateMaterial;
		private PlayerCamera _playerCamera;

		// Use this for initialization
		void Start() {

			
			if(GateRenderers.Length > 1) {
				_gateMaterial = GateRenderers[0].material;
				foreach(var item in GateRenderers) {
					item.sharedMaterial = _gateMaterial;
				}
			}

			_gateMaterial.EnableKeyword("EMISSION");
			_gateMaterial.SetColor("_EmissionColor", new Color(2, 0, 0));

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
				_gateMaterial.SetColor("_EmissionColor", Color.Lerp(new Color(2, 0, 0), new Color(2, 2, 0), ratio));

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
