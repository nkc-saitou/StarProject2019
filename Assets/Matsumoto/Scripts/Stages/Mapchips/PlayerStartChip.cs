using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerStartChip : GimmickChip {

		public Player PlayerPrefab;

		private void Start() {

			var cam = FindObjectOfType<PlayerCamera>();
			var player = Instantiate(PlayerPrefab, transform.position, transform.rotation);
			cam.SetTarget(player);

			Destroy(gameObject);
		}
	}
}