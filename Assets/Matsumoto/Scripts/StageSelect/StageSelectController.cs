using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectController : MonoBehaviour {

	public const string LoadSceneKey = "LoadScene";

	public StageNode FirstStage;
	public Transform PlayerModel;

	public float MoveSpeed;

	private int _stageProgress;
	private StageNode _currentSelectedStage;
	private float _playerPositionTarget;
	private float _moveSpeedMag = 1;

	public float Position;

	// Use this for initialization
	void Start () {

		// データ読み込み
		GameData.Instance.Load();
		GameData.Instance.GetData("StageProgress", ref _stageProgress);

		// ステージノードのセットアップ
		FirstStage.SetUpNode(null);

		// 進めたステージまで移動
		_currentSelectedStage = GetStageNode(_stageProgress);
		_playerPositionTarget = GetLength(_currentSelectedStage);

		// プレイヤーを移動
		MovePlayer(_playerPositionTarget);

		Position = _playerPositionTarget;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.A)) {
			var prev = _currentSelectedStage.PrevStage;
			if(prev) SetTarget(prev);
		}
		if(Input.GetKeyDown(KeyCode.D)) {
			var next = _currentSelectedStage.NextStage;
			if(next) SetTarget(next);
		}
		if(Input.GetKeyDown(KeyCode.F)) {
			MoveScene();
		}

		Position = Mathf.MoveTowards(Position, _playerPositionTarget, MoveSpeed * _moveSpeedMag * Time.deltaTime);
		MovePlayer(Position);
	}

	private void MoveScene() {
		Debug.Log("MoveScene");
		GameData.Instance.SetData(LoadSceneKey, _currentSelectedStage.TargetStageName);
		SceneMover.MoveScene("GameScene");
	}

	private void MovePlayer(float position) {
		var current = FirstStage;

		while(true) {
			position -= current.Length;
			if(position < 0) break;
			if(!current.NextStage) {
				position = 0;
				break;
			}

			current = current.NextStage;
		}

		position *= -1;

		var nextPos = new Vector3();
		var angle = 0.0f;

		if(position == 0) {
			nextPos = current.transform.position;
		}
		else {
			var ratio = 1 - position / current.Length;
			nextPos = Vector3.Lerp(current.transform.position, current.NextStage.transform.position, ratio);

			angle = (1 - ratio) * Mathf.Floor(current.Length) * 360f;
		}

		PlayerModel.transform.position = nextPos;
		PlayerModel.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private StageNode GetStageNode(int number) {

		var current = FirstStage;
		for(int i = 0;i < number;i++) {
			if(!current.NextStage) break;
			current = current.NextStage;
		}

		return current;
	}

	private float GetLength(StageNode to) {

		var length = 0.0f;
		var current = FirstStage;
		while(current != to) {
			if(!current.NextStage) break;
			length += current.Length;
			current = current.NextStage;
		}

		return length;
	}

	private void SetTarget(StageNode node) {
		_currentSelectedStage = node;
		_playerPositionTarget = GetLength(_currentSelectedStage);
		_moveSpeedMag = 1 + Mathf.Abs(Position - _playerPositionTarget) * 0.8f;
	}
}
