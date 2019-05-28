using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Audio;

namespace Matsumoto.Character {

	public class PlayerFoot : MonoBehaviour {

		public float MinPlay = 0.01f;

		private Player player;
		private float playedTime = 0;

		private void Awake() {
			player = GetComponentInParent<Player>();
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			if(collision.GetComponent<Player>()) return;
			if(player.State != PlayerState.Star) return;
			if(Time.time - playedTime < MinPlay) return;
			playedTime = Time.time;

			AudioManager.PlaySE("FootStep", 1f, position: collision.transform.position);
		}
	}
}

