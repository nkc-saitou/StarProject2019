using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;
using UnityEngine.Experimental.U2D.Animation;

namespace Matsumoto.Gimmick {
	public class FollowPlayerChip : GimmickChip {

		public PlayerFollower PlayerFollowerPrefab;
		public int FollowerIndex;
		public float Amplitude = 0.1f;
		public float Speed = 2.0f;

		private string _dataKey;
		private float _randomTime;
		private float _startY;

		public override void GimmickStart() {
			base.GimmickStart();

			if(Controller.FollowerData.FindedIndexList
				.Exists(x => x == FollowerIndex)) {
				Destroy(gameObject);
			}
		}

		private void Start() {
			_randomTime = Random.Range(0, 1);
			_startY = transform.position.y;

			GetComponentInChildren<SpriteSkin>().enabled = true;
		}

		private void Update() {
			// 移動
			var pos = transform.position;
			pos.y = _startY + Mathf.Abs(Mathf.Sin((Time.time + _randomTime) * Speed)) * Amplitude;
			transform.position = pos;
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			var player = collision.GetComponent<Player>();
			if(player) {

				var follower = Instantiate(PlayerFollowerPrefab, transform.position, transform.rotation);
				follower.Target = player;

				Controller.FollowerData.FindedIndexList
					.Add(FollowerIndex);

				Destroy(gameObject);
			}
		}
	}
}
