﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class StageController : MonoBehaviour {

	public event Action<StageController> OnGameStart;
	public event Action<StageController> OnGameClear;
	public event Action<StageController> OnGameOver;

	// Use this for initialization
	void Start () {
		GameStart();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameStart() {

		Debug.Log("GameStart!");
		OnGameStart?.Invoke(this);
	}

	public void GameClear() {

		Debug.Log("GameClear!");
		OnGameClear?.Invoke(this);
	}

	public void GameOver() {

		Debug.Log("GameOver!");
		OnGameOver?.Invoke(this);
	}
}