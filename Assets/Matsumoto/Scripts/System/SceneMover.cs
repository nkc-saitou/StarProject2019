using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover {

	public static void MoveScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}
}
