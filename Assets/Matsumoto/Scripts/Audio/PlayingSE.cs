using UnityEngine;
using UnityEngine.Events;

namespace DDD.Matsumoto.Audio {

	/// <summary>
	/// SEに取り付けられている制御用クラス
	/// </summary>
	public class PlayingSE : MonoBehaviour {

		public event UnityAction OnDestroyEvent;

		void OnDestroy() {
			OnDestroyEvent?.Invoke();
		}
	}
}