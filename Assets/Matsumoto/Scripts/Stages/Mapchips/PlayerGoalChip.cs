using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matsumoto.Character;

namespace Matsumoto.Gimmick {

	public class PlayerGoalChip : GimmickChip {

		// Use this for initialization
		void Start() {

		}

		// Update is called once per frame
		void Update() {

		}

		private void OnTriggerEnter2D(Collider2D collision) {

			if(!collision.GetComponent<Player>()) return;
			Controller.GameClear();

			Destroy(gameObject);
		}

	}
}
