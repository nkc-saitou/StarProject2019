using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.SceneManagement;
using Matsumoto.Gimmick;

public enum GameState {
	StartUp,
	Playing,
	GameClear,
	GameOver,
}

public class StageController : MonoBehaviour {

	public event Action<StageController> OnGameStart;
	public event Action<StageController> OnGameClear;
	public event Action<StageController> OnGameOver;

	public bool IsCreateStage = true;

	private string _followerDataKey;
	private List<GimmickChip> _gimmicks = new List<GimmickChip>();

	public GameState State {
		get; private set;
	} = GameState.StartUp;

	private FollowerFindData _followerData = new FollowerFindData();
	public FollowerFindData FollowerData {
		get { return _followerData; }
	}

	private void Awake() {

		var stagePath = "TestStage";
		GameData.Instance.GetData(StageSelectController.LoadSceneKey, ref stagePath);

		// ステージ生成
		CreateStage(stagePath);

		// フォロワーのデータ取得
		_followerDataKey = stagePath + "_FollowerData";
		GameData.Instance.GetData(_followerDataKey, ref _followerData);

		// ステージにないデータを削除
		var followerChipIndex = FindObjectsOfType<FollowPlayerChip>()
			.Select(item => item.FollowerIndex)
			.ToArray();

		_followerData.FindedIndexList = _followerData.FindedIndexList
			.Where(data => Array.Exists(followerChipIndex, x => x == data))
			.ToList();

		// ギミック
		_gimmicks = FindObjectsOfType<GimmickChip>().ToList();
		foreach(var item in _gimmicks) {
			item.Controller = this;
			item.GimmickStart();
		}
	}

	// Use this for initialization
	void Start () {
		GameStart();
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Escape))
			PauseSystem.Instance.IsPause = !PauseSystem.Instance.IsPause;

		if(State == GameState.GameOver) {
			//リトライ
			if(Input.GetButtonDown("Attack")) {
				SceneChanger.Instance.MoveScene("GameScene", 0.2f, 0.2f, SceneChangeType.BlackFade);
			}
		}

	}

	private void CreateStage(string stagePath) {

		if(!IsCreateStage) return;
		Instantiate(Resources.Load("Stages/" + stagePath));
	}

	public void GameStart() {

		Debug.Log("GameStart!");

		State = GameState.Playing;

		OnGameStart?.Invoke(this);
	}

	public void GameClear() {

		Debug.Log("GameClear!");

		State = GameState.GameClear;

		GameData.Instance.SetData(_followerDataKey, _followerData);
		GameData.Instance.Save();

		OnGameClear?.Invoke(this);
	}

	public void GameOver() {

		Debug.Log("GameOver!");
		OnGameOver?.Invoke(this);

		StartCoroutine(GameOverWait());
	}

	IEnumerator GameOverWait() {
		yield return new WaitForSeconds(1);
		State = GameState.GameOver;
	}
}
