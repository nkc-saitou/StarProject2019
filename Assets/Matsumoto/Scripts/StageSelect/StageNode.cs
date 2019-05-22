using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour {

	private const string StageBasePath = "Stages/";

	public string TargetStageName;

	[SerializeField]
	private StageNode _nextStage;
	public StageNode NextStage {
		get { return _nextStage; }
	}

	public StageNode PrevStage {
		get; private set;
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
	public void SetUpNode (StageNode prevStage) {

		PrevStage = prevStage;

		if(NextStage) {
			NextStage.SetUpNode(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
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
