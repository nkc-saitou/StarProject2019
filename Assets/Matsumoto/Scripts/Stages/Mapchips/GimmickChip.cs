using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matsumoto.Gimmick {

	public class GimmickChip : MonoBehaviour {

		protected StageController Controller {
			get; private set;
		}

		private void Awake() {

			Controller = FindObjectOfType<StageController>();

		}

		protected virtual void GimmickStart() { }

		[ContextMenu("SnapToGrid")]
		public void SnapToGrid() {
			var pos = transform.position;

			pos.x = Mathf.Floor(pos.x) + 0.5f;
			pos.y = Mathf.Floor(pos.y) + 0.5f;
			pos.z = 0.0f;

			transform.position = pos;
		}
	}
}