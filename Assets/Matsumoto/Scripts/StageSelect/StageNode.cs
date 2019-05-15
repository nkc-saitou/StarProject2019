using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour {

	private const string StageBasePath = "Stages/";

	public StageNode NextStage;
	public string TargetStageName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void OnDrawGizmos() {
		if(!NextStage) return;

		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, NextStage.transform.position);
	}
}
