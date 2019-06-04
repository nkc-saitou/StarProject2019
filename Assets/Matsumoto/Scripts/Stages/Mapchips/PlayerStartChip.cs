using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerStartChip : GimmickChip {

		public override void GimmickStart() {

			var player = FindObjectOfType<Player>();
			player.transform.position = transform.position;
			var cam = FindObjectOfType<PlayerCamera>();
			cam.SetTarget(player);

			Destroy(gameObject);
		}
	}
}