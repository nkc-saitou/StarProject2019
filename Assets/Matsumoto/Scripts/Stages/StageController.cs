using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

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

	public GameState State {
		get; private set;
	} = GameState.StartUp;

	private void Awake() {
		// ステージ生成
		CreateStage();
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

	private void CreateStage() {

		if(!IsCreateStage) return;

		var stagePath = "TestStage";
		GameData.Instance.GetData(StageSelectController.LoadSceneKey, ref stagePath);
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
