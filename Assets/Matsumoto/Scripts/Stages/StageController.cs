using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.SceneManagement;
using Matsumoto.Gimmick;
using Matsumoto.Audio;

public enum GameState {
	StartUp,
	Playing,
	GameClear,
	GameOver,
}

public class StageController : MonoBehaviour {

	public const string StageFollowerDataTarget = "_FollowerData";

	public event Action<StageController> OnGameStart;
	public event Action<StageController> OnGameClear;
	public event Action<StageController> OnGameOver;

	public PauseMenu PauseMenuCamvas;

	public bool IsCreateStage = true;
	public bool IsReturnToSelect = true;
	public bool CanPause = false;

	public string StagePath = "TestStage";
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

		GameData.Instance.GetData(StageSelectController.LoadSceneKey, ref StagePath);

		// ステージ生成
		CreateStage(StagePath);

		// フォロワーのデータ取得
		_followerDataKey = StagePath + StageFollowerDataTarget;
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

		PauseMenuCamvas.SetStageController(this);
		PauseMenuCamvas.gameObject.SetActive(false);

	}

	// Use this for initialization
	void Start () {

		// BGMを鳴らす
		AudioManager.FadeIn(1.0f, "Comet_Highway");

		GameStart();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape) && CanPause) {
			PauseSystem.Instance.IsPause = !PauseSystem.Instance.IsPause;
			PauseMenuCamvas.gameObject.SetActive(!PauseMenuCamvas.gameObject.activeSelf);
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

		CanPause = true;
	}

	public void GameClear() {

		Debug.Log("GameClear!");

		State = GameState.GameClear;

		// フォロワーとクリアデータを保存
		GameData.Instance.SetData(_followerDataKey, _followerData);

		var clearedStages = new HashSet<string>();
		GameData.Instance.GetData(StageSelectController.StageProgressKey, ref clearedStages);
		clearedStages.Add(StagePath);
		GameData.Instance.SetData(StageSelectController.StageProgressKey, clearedStages);
		GameData.Instance.Save();

		OnGameClear?.Invoke(this);
		CanPause = false;

		if(IsReturnToSelect) {
			AudioManager.FadeOut(1.0f);
			SceneChanger.Instance.MoveScene("StageSelect", 1.0f, 1.0f, SceneChangeType.WhiteFade);
		}
	}

	public void GameOver() {

		Debug.Log("GameOver!");
		State = GameState.GameOver;

		OnGameOver?.Invoke(this);
		CanPause = false;

		StartCoroutine(GameOverWait());
	}

	IEnumerator GameOverWait() {
		yield return new WaitForSeconds(1);

		// BGMを止める
		AudioManager.FadeOut(0.2f);

		var sceneName = SceneManager.GetActiveScene().name;
		SceneChanger.Instance.MoveScene(sceneName, 0.2f, 0.2f, SceneChangeType.BlackFade);
	}
}
