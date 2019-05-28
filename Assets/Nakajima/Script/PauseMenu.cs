using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // 現在のステート
    public enum MenuState
    {
        BGM = 0,
        SE,
        STAGESELECT,
        RETRY
    }
    public MenuState menuState;

    // 項目の位置
    [SerializeField]
    private GameObject[] menuList;

    // BGM,SEの音量調整用のSlider
    [SerializeField]
    private Slider[] volumeSlider; 

    // 選択項目イメージ
    [SerializeField]
    private GameObject selectImage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        MenuSelect();
	}

    // メニューの操作
    private void MenuSelect()
    {
        // 項目の更新
        switch (menuState)
        {
            case MenuState.BGM:
                if (Input.GetKeyDown(KeyCode.S)) menuState += 1;
                break;
            case MenuState.SE:
                if (Input.GetKeyDown(KeyCode.S)) menuState += 1;
                if (Input.GetKeyDown(KeyCode.W)) menuState -= 1;
                //if (Input.GetKeyDown(KeyCode.A)) menuState += 1;
                //if (Input.GetKeyDown(KeyCode.D)) menuState -= 1;
                break;
            case MenuState.STAGESELECT:
                if (Input.GetKeyDown(KeyCode.W)) menuState -= 1;
                if (Input.GetKeyDown(KeyCode.D)) menuState += 1;
                break;
            case MenuState.RETRY:
                if (Input.GetKeyDown(KeyCode.W)) menuState -= 2;
                if (Input.GetKeyDown(KeyCode.A)) menuState -= 1;
                break;
            default:
                break;
        }
        Vector3 imagePos = new Vector3(menuList[(int)menuState].transform.position.x, menuList[(int)menuState].transform.position.y, selectImage.transform.position.z);
        selectImage.transform.position = imagePos;

        MenuAction(menuState);
    }

    /// <summary>
    /// メニューごとのアクション
    /// </summary>
    /// <param name="_currentState">現在のステート</param>
    private void MenuAction(MenuState _currentState)
    {
        switch (_currentState)
        {
            case MenuState.BGM:
                if (Input.GetKeyDown(KeyCode.A)) VolumeChange(_currentState, -0.1f);
                if (Input.GetKeyDown(KeyCode.D)) VolumeChange(_currentState, 0.1f);
                break;
            case MenuState.SE:
                if (Input.GetKeyDown(KeyCode.A)) VolumeChange(_currentState, -0.1f);
                if (Input.GetKeyDown(KeyCode.D)) VolumeChange(_currentState, 0.1f);
                break;
            case MenuState.STAGESELECT:
                break;
            case MenuState.RETRY:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 音量調整
    /// </summary>
    /// <param name="_currentState">現在のステート</param>
    /// <param name="_value">入力値</param>
    private void VolumeChange(MenuState _currentState, float _value)
    {
        volumeSlider[(int)_currentState].value += _value;

        switch (_currentState)
        {
            case MenuState.BGM:
                Matsumoto.Audio.AudioManager.SetBGMVolume(volumeSlider[(int)_currentState].value);
                Matsumoto.Audio.AudioManager.GetBGMVolume();
                break;
            case MenuState.SE:
                Matsumoto.Audio.AudioManager.SetSEVolume(volumeSlider[(int)_currentState].value);
                Matsumoto.Audio.AudioManager.GetSEVolume();
                break;
        }
    } 
}
