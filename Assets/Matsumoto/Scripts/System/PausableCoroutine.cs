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
		_coroutines.Add(new CoroutineInfo(target, routine, r));
		return r;
	}

	public void OnPauseBegin() {

		Debug.Log(_coroutines.Count);

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
			item.Target.StartCoroutine(item.Routine);
		}
	}
}

static class CoroutineExtension {

	public static Coroutine StartPausableCoroutine(this MonoBehaviour target, IEnumerator routine) {
		return PausableCoroutineSystem.Instance.StartPausableCoroutine(target, routine);
	}
}

class CoroutineInfo {
	public MonoBehaviour Target;
	public IEnumerator Routine;
	public Coroutine Coroutine;

	public CoroutineInfo(MonoBehaviour target, IEnumerator routine, Coroutine coroutine) {
		Target = target;
		Routine = routine;
		Coroutine = coroutine;
	}
}