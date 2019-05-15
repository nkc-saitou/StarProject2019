using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectController : MonoBehaviour {

	public StageNode FirstStage;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.A)) {
			GameData.Instance.SetData("test", 1);
		}

		if(Input.GetKeyDown(KeyCode.B)) {
			int a = 0;
			GameData.Instance.GetData("test", ref a);
			Debug.Log(a);
		}

		if(Input.GetKeyDown(KeyCode.C)) {
			GameData.Instance.Save();
		}

		if(Input.GetKeyDown(KeyCode.D)) {
			GameData.Instance.Load();
		}
	}
}
