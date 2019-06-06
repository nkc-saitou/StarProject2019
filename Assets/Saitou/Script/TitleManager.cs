using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

    float time = 0;

    bool isFirst = true;

    void Update()
    {
        time += Time.deltaTime;

        if (time <= 0.5f) return;

        if (Input.GetButtonDown("Morph") && isFirst)
        {
            SceneChanger.Instance.MoveScene("StageSelect", 1.0f, 1.0f, SceneChangeType.WhiteFade);
            isFirst = false;
        }
    }
}
