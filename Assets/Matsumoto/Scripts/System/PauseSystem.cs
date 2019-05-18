using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : SingletonMonoBehaviour<PauseSystem> {

	private List<MonoBehaviour> _pauseMonoBehaviours = new List<MonoBehaviour>();
	private List<IPauseEventReceivable> _pauseReceivables = new List<IPauseEventReceivable>();
	private List<Rigidbody2DInfo> _pauseRigidbody2DInfos = new List<Rigidbody2DInfo>();
	private List<Animator> _pauseAnimators = new List<Animator>();
	private List<ParticleSystem> _pauseParticles = new List<ParticleSystem>();

	public bool StopParticles {
		get; set;
	} = true;

	private bool _isPause;
	public bool IsPause {
		get { return _isPause; }
		set {
			if(value) Pause();
			else Resume();
		}
	}

	protected override void Init() {
		base.Init();

		SceneManager.sceneUnloaded += (s) => {
			_pauseMonoBehaviours.Clear();
			_pauseRigidbody2DInfos.Clear();
		};
	}

	public void AddPauseList(MonoBehaviour target, bool addChilds = false) {

		if(!target) return;
		if(_pauseMonoBehaviours.Contains(target)) return;

		var findRigs = target.GetComponentsInChildren<Rigidbody2D>();
		foreach(var item in findRigs) {
			_pauseRigidbody2DInfos.Add(new Rigidbody2DInfo(item));
		}

		_pauseAnimators.AddRange(target.GetComponentsInChildren<Animator>());

		// TODO _pauseReceivablesに追加
		var receivable = target.GetComponent<IPauseEventReceivable>();
		if(receivable != null) _pauseReceivables.Add(receivable);

		if(addChilds) {
			_pauseMonoBehaviours.AddRange(target.GetComponentsInChildren<MonoBehaviour>());
		}
		else {
			if(target != this) {
				_pauseMonoBehaviours.Add(target);
			}
		}
	}

	public void Pause() {
		if(_isPause) return;
		_isPause = true;

		// インターフェース呼び出し
		foreach(var item in _pauseReceivables) {
			item.OnPauseBegin();
		}

		// Rigidbodyの停止
		foreach(var item in _pauseRigidbody2DInfos) {
			// 速度、角速度も保存しておく
			item.Save();
			item.TargetRigidbody.Sleep();
		}

		// アニメーションの停止
		foreach(var item in _pauseAnimators) {
			item.enabled = false;
		}

		// パーティクルの停止
		if(StopParticles) {
			_pauseParticles.Clear();
			var predicate = new Predicate<ParticleSystem>(x => x.isPlaying);
			_pauseParticles.AddRange(Array.FindAll(FindObjectsOfType<ParticleSystem>(), predicate));
			foreach(var item in _pauseParticles) {
				item.Pause();
			}
		}

		// MonoBehaviourの停止
		foreach(var item in _pauseMonoBehaviours) {
			item.enabled = false;
			
		}

		// インターフェース呼び出し
		foreach(var item in _pauseReceivables) {
			item.OnPauseEnd();
		}
	}

	public void Resume() {
		if(!_isPause) return;
		_isPause = false;

		// インターフェース呼び出し
		foreach(var item in _pauseReceivables) {
			item.OnResumeBegin();
		}

		// Rigidbodyの再開
		foreach(var item in _pauseRigidbody2DInfos) {
			item.TargetRigidbody.WakeUp();
			item.Load();
		}

		// アニメーションの再開
		foreach(var item in _pauseAnimators) {
			item.enabled = true;
		}

		// パーティクルの再開
		if(StopParticles) {
			foreach(var item in _pauseParticles) {
				item.Play();
			}
		}

		// MonoBehaviourの再開
		foreach(var item in _pauseMonoBehaviours) {
			item.enabled = true;
		}

		// インターフェース呼び出し
		foreach(var item in _pauseReceivables) {
			item.OnResumeEnd();
		}
	}
}

/// <summary>
/// Rigidbodyの速度を保存しておく
/// </summary>
class Rigidbody2DInfo {

	public Rigidbody2D TargetRigidbody;

	public Vector2 Velocity {
		get; private set;
	}

	public float AngularVeloccity {
		get;private set;
	}

	public Rigidbody2DInfo(Rigidbody2D rigidbody2d) {
		TargetRigidbody = rigidbody2d;

	}

	public void Save() {
		Velocity = TargetRigidbody.velocity;
		AngularVeloccity = TargetRigidbody.angularVelocity;
	}

	public void Load() {
		TargetRigidbody.velocity = Velocity;
		TargetRigidbody.angularVelocity = AngularVeloccity;
	}
}