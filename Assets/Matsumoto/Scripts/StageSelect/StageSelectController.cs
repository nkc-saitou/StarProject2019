using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StageSelectController : MonoBehaviour {

	public const string LoadSceneKey = "LoadScene";

	public StageNode FirstNode;
	public Transform PlayerModel;

	public float MoveSpeed;

	private int _stageProgress;
	private StageNode _currentSelectedStage;
	private StageNode _targetStage;
	private float _playerPositionTarget;
	private float _moveSpeedMag = 1;
	private List<IStageMoveEvent> _eventList;

	public float Position;

	public bool IsFreeze {
		get; set;
	}

	// Use this for initialization
	void Start () {

		// イベント読み込み
		_eventList = GetComponentsInChildren<MonoBehaviour>()
			.Select(x => x as IStageMoveEvent)
			.Where(x => x != null)
			.ToList();

		// 進行度読み込み
		GameData.Instance.GetData("StageProgress", ref _stageProgress);

		// ステージノードのセットアップ
		FirstNode.SetUpNode(null);

		// 進めたステージまで移動
		_currentSelectedStage = _targetStage = GetStageNode(_stageProgress);
		_playerPositionTarget = GetLength(_targetStage);

		// プレイヤーを移動
		MovePlayer(_playerPositionTarget);

		Position = _playerPositionTarget;
	}
	
	// Update is called once per frame
	void Update () {

		if(IsFreeze) return;

		if(Input.GetKeyDown(KeyCode.A)) {
			var prev = _currentSelectedStage.PrevStage;
			if(prev) SetTarget(prev);
		}
		if(Input.GetKeyDown(KeyCode.D)) {
			var next = _currentSelectedStage.NextStage;
			if(next) SetTarget(next);
		}
		if(Input.GetKeyDown(KeyCode.F)) {
			if(_targetStage != FirstNode)
				MoveScene();
		}

		var p = Mathf.MoveTowards(Position, _playerPositionTarget, MoveSpeed * _moveSpeedMag * Time.deltaTime);
		MovePlayer(p);

		var forward = p >= Position;
		var low = forward ? Position : p;
		var high = forward ? p : Position;
		foreach(var e in _eventList) {
			var c = e.GetPosition();
			if(low <= c && c <= high) {
				e.OnExecute(this, forward);
			}
		}

		if(p == Position) {
			_currentSelectedStage = _targetStage;
		}

		Position = p;
	}

	private void MoveScene() {
		Debug.Log("MoveScene");
		GameData.Instance.SetData(LoadSceneKey, _targetStage.TargetStageName);
		SceneChanger.Instance.MoveScene("GameScene", 1.0f, 1.0f, SceneChangeType.BlackFade);
	}

	private void MovePlayer(float position) {
		var current = FirstNode;

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

		var current = FirstNode;
		for(int i = 0;i < number;i++) {
			if(!current.NextStage) break;
			current = current.NextStage;
		}

		return current;
	}

	private float GetLength(StageNode to) {

		var length = 0.0f;
		var current = FirstNode;
		while(current != to) {
			if(!current.NextStage) break;
			length += current.Length;
			current = current.NextStage;
		}

		return length;
	}

	private void SetTarget(StageNode node) {
		_targetStage = node;
		_playerPositionTarget = GetLength(_targetStage);
		_moveSpeedMag = 1 + Mathf.Abs(Position - _playerPositionTarget) * 0.8f;
	}

	public Vector3 GetPosition(float position) {

		var current = FirstNode;

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

		var pos = new Vector3();

		if(position == 0) {
			pos = current.transform.position;
		}
		else {
			var ratio = 1 - position / current.Length;
			pos = Vector3.Lerp(current.transform.position, current.NextStage.transform.position, ratio);
		}
		return pos;

	}
}
