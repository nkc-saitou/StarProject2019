using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;
using Matsumoto.Audio;

namespace Matsumoto.Gimmick {

	public class HalfPointChip : GimmickChip {

		public Collider2D Collider;
		public SpriteRenderer HalfPointImage;
		public Sprite[] NormalAndActiveSprite;

		private float _createTime;
		private float _activateDelay = 1.0f;
		private float _reuseTime = 0.5f;

		public override void GimmickStart() {
			_createTime = Time.time;
			HalfPointImage.sprite = NormalAndActiveSprite[0];
		}

		public void OnTriggerEnter2D(Collider2D collition) {

			if(_activateDelay > Time.time - _createTime) return;
			var player = collition.GetComponent<Player>();
			if(!player) return;
			AudioManager.PlaySE("HalfPoint", position: transform.position);
			Controller.SetHalfPoint(player.transform.position);
			this.StartPausableCoroutine(ReUseWait());
		}

		private IEnumerator ReUseWait() {
			HalfPointImage.sprite = NormalAndActiveSprite[1];
			yield return new WaitForSeconds(_reuseTime);
			HalfPointImage.sprite = NormalAndActiveSprite[0];
		}

	}
}
