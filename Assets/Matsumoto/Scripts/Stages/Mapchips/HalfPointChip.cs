using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class HalfPointChip : GimmickChip {

		public Collider2D Collider;
		public SpriteRenderer HalfPointImage;
		public Sprite[] NormalAndActiveSprite;

		private float reuseTime = 0.5f;

		public override void GimmickStart() {
			HalfPointImage.sprite = NormalAndActiveSprite[0];
		}

		public void OnTriggerEnter2D(Collider2D collition) {
			var player = collition.GetComponent<Player>();
			if(!player) return;

			Controller.SetHalfPoint(player.transform.position);
			this.StartPausableCoroutine(ReUseWait());
		}

		private IEnumerator ReUseWait() {
			HalfPointImage.sprite = NormalAndActiveSprite[1];
			Collider.enabled = false;
			yield return new WaitForSeconds(reuseTime);
			Collider.enabled = true;
			HalfPointImage.sprite = NormalAndActiveSprite[0];
		}

	}
}
