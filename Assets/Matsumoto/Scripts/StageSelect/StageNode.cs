using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour {

	private const string StageBasePath = "Stages/";

	public string TargetStageName;

	public GameObject FollowerModelPrefab;
	public List<Transform> FindedFollowerPositions = new List<Transform>();

	private float _freq = 1.0f;
	private float _amp = 1.0f;
	private Transform _lab;
	private SpriteRenderer _labRenderer;
	private bool _isPlayAnim;

	[SerializeField]
	private StageNode _nextStage;
	public StageNode NextStage {
		get { return _nextStage; }
	}

	public StageNode PrevStage {
		get; private set;
	}

	public bool IsSelected {
		get; set;
	}

	public bool IsCleared {
		get; set;
	}

	private float _length = 0;
	public float Length {
		get {
			if(!NextStage) return 0;
			_length = (NextStage.transform.position - transform.position).magnitude;
			return _length;
		}
		private set { _length = value; }
	}

	// Use this for initialization
	public void SetUpNode(StageNode prevStage, int clearedCount, ref int followerCount) {

		if(transform.childCount > 0)
			_lab = transform.GetChild(0);

		_labRenderer = GetComponentInChildren<SpriteRenderer>();

		IsCleared = clearedCount > 0;

		PrevStage = prevStage;

		// 簡単にクリア状態を表示
		var spr = GetComponentInChildren<SpriteRenderer>();
		if(spr) {
			spr.color = IsCleared ? Color.white : Color.gray;
		}

		// 助けた数を表示
		FollowerFindData followerData = new FollowerFindData();
		GameData.Instance.GetData(TargetStageName + StageController.StageFollowerDataTarget, ref followerData);
		var count = followerData.FindedIndexList.Count;
		followerCount += count;
		for (int i = 0; i < count; i++) {
			if(FindedFollowerPositions.Count < i) break;
			var t = FindedFollowerPositions[i];
			var f = Instantiate(FollowerModelPrefab, t.position, t.rotation);
			f.transform.localScale = Vector3.one * 0.5f;
		}


		if(NextStage) {
			NextStage.SetUpNode(this, --clearedCount, ref followerCount);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(IsSelected && _lab && !_isPlayAnim) {
			StartCoroutine(SelectedAnim());
		}
	}

	private IEnumerator SelectedAnim() {
		_isPlayAnim = true;
		_labRenderer.sortingOrder = 100;

		var t = 0.0f;

		while(t < 0.5f) {

			t = Mathf.Min(t + Time.deltaTime * _freq, 0.5f);
			_lab.transform.localScale = Vector3.one * (1 + Mathf.Sin(t * 2 * Mathf.PI) * _amp);
			yield return null;
		}

		if(IsSelected && _lab && !_isPlayAnim) {
			StartCoroutine(SelectedAnim());
		}
		else {
			_isPlayAnim = false;
			_labRenderer.sortingOrder = 0;
		}
	}

	public void OnDrawGizmos() {
		if(!NextStage) return;

		Gizmos.color = Color.white;
		Gizmos.DrawSphere(transform.position, 0.1f);
		Gizmos.DrawLine(transform.position, NextStage.transform.position);

		var arrowLength = 0.2f;
		var deg = 30.0f;
		var vec = (transform.position - NextStage.transform.position).normalized;

		Gizmos.DrawLine(NextStage.transform.position, NextStage.transform.position + Quaternion.AngleAxis(deg, Vector3.forward) * vec * arrowLength);
		Gizmos.DrawLine(NextStage.transform.position, NextStage.transform.position + Quaternion.AngleAxis(-deg, Vector3.forward) * vec * arrowLength);
	}
}
