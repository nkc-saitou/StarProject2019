using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

	public Player TargetPlayer;
	public float CameraSpeed = 0.3f;
	public Vector2 Offset;
	public float FollowView = 3;
	public float FollowSpeed = 1;
	public bool IsFreeze = false;

	private float _zPosition;
	private Vector2 _angleOffset;
	private Vector2 _screenRatio;

	// Use this for initialization
	void Start () {
		_zPosition = transform.position.z;
		_screenRatio = new Vector2(1, (float)Screen.height / Screen.width);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateCamera();
	}

	public void UpdateCamera() {
		if(IsFreeze) return;
		if(!TargetPlayer) return;

		var target = TargetPlayer.transform.position;

		// 移動方向に寄せる
		var targetOffset = TargetPlayer.Rig.velocity.normalized * FollowView;
		var magSpeed = Vector2.Angle(_angleOffset, targetOffset) / 360 * 2 + 1;
		_angleOffset = Vector3.MoveTowards(_angleOffset, targetOffset, FollowSpeed * Time.deltaTime * magSpeed);

		// オフセット
		target += (Vector3)Offset;
		target += (Vector3)(_angleOffset * _screenRatio);
		target.z = _zPosition;

		// 追従
		transform.position = Vector3.Lerp(transform.position, target, CameraSpeed);
	}
}
