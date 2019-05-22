using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEvent : MonoBehaviour, IStageMoveEvent {

	public float EventPosition;
	public StageSelectController Controller;
	public Vector3 MovePosition;
	public float Speed = 10.0f;

	private Vector3 _startPosition;
	private Camera _targetCamera;

	private void Start() {
		_targetCamera = Camera.main;
		_startPosition = _targetCamera.transform.position;
	}

	public float GetPosition() {
		return EventPosition;
	}

	public void OnExecute(StageSelectController controller, bool forward) {
		Debug.Log("Call");
		StartCoroutine(CameraMoveAnim(controller, forward));
	}

	private IEnumerator CameraMoveAnim(StageSelectController controller, bool forward) {

		controller.IsFreeze = true;
		var start = forward ? _startPosition : MovePosition;
		var end = forward ? MovePosition : _startPosition;
		var t = 0.0f;
		while(t < 1.0f) {

			t = Mathf.Min(t + Time.deltaTime * Speed, 1.0f);
			_targetCamera.transform.position = Vector3.Lerp(start, end, t);
			yield return null;
		}

		controller.IsFreeze = false;
	}

	private void OnDrawGizmos() {
		if(!Controller) return;

		var pos = Controller.GetPosition(EventPosition);

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(pos, 0.1f);
	}

	private void OnDrawGizmosSelected() {
		if(!Controller) return;

		var pos = Controller.GetPosition(EventPosition);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pos, 0.2f);
	}
}
