using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausableCoroutineSystem : SingletonMonoBehaviour<PausableCoroutineSystem>, IPauseEventReceivable {

	private List<CoroutineInfo> _coroutines = new List<CoroutineInfo>();

	protected override void Init() {
		base.Init();

		PauseSystem.Instance.AddPauseList(this);

		SceneManager.sceneUnloaded += (s) => {
			_coroutines.Clear();
		};
	}

	public Coroutine StartPausableCoroutine(MonoBehaviour target, IEnumerator routine) {
		var r = target.StartCoroutine(routine);
		var info = new CoroutineInfo(target, routine, r);
		_coroutines.Add(info);
		// コルーチン終了検知を追加する
		StartCoroutine(AddDeleter(info));
		return r;
	}

	public void OnPauseBegin() {

		var temp = new List<CoroutineInfo>();
		foreach(var item in _coroutines) {
			if(item == null || item.Coroutine == null) {
				temp.Add(item);
				continue;
			}

			item.Target.StopCoroutine(item.Coroutine);
		}

		foreach(var item in temp) {
			_coroutines.Remove(item);
		}
	}

	public void OnPauseEnd() {
		
	}

	public void OnResumeBegin() {

	}

	public void OnResumeEnd() {
		foreach(var item in _coroutines) {
			item.Coroutine = item.Target.StartCoroutine(item.Routine);
			StartCoroutine(AddDeleter(item));
		}
	}

	private IEnumerator AddDeleter(CoroutineInfo info) {
		yield return info.Coroutine;
		_coroutines.Remove(info);
	}
}

static class CoroutineExtension {

	public static Coroutine StartPausableCoroutine(this MonoBehaviour target, IEnumerator routine) {
		return PausableCoroutineSystem.Instance.StartPausableCoroutine(target, routine);
	}
}

class CoroutineInfo {

	public MonoBehaviour Target {
		get; set;
	}

	public IEnumerator Routine {
		get; set;
	}

	public Coroutine Coroutine {
		get; set;
	}

	public CoroutineInfo(MonoBehaviour target, IEnumerator routine, Coroutine coroutine) {
		Target = target;
		Routine = routine;
		Coroutine = coroutine;
	}
}