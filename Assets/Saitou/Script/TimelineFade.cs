using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineFade : MonoBehaviour {

    bool isFirst = true;

    void FadeOut()
    {
        if(isFirst)
        {
            isFirst = false;


            SceneChanger.Instance.MoveScene("StageSelect", 1.0f, 1.0f, SceneChangeType.WhiteFade);
        }

    }
}
