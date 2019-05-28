using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    /// <summary>
    /// BGMステートの処理
    /// </summary>
    private void BGM_Action()
    {
        if (Input.GetKeyDown(KeyCode.S)) menuState += 1;

        if (Input.GetKeyDown(KeyCode.A)) VolumeChange(menuState, -0.1f);
        if (Input.GetKeyDown(KeyCode.D)) VolumeChange(menuState, 0.1f);

        SetPosition();
    }

    /// <summary>
    /// SEステートの処理
    /// </summary>
    private void SE_Action()
    {
        if (Input.GetKeyDown(KeyCode.S)) menuState += 1;
        if (Input.GetKeyDown(KeyCode.W)) menuState -= 1;

        if (Input.GetKeyDown(KeyCode.A)) VolumeChange(menuState, -0.1f);
        if (Input.GetKeyDown(KeyCode.D)) VolumeChange(menuState, 0.1f);

        SetPosition();
    }

    /// <summary>
    /// STAGESELECTステートの処理
    /// </summary>
    private void STAGESELECT_Action()
    {
        if (Input.GetKeyDown(KeyCode.W)) menuState -= 1;
        if (Input.GetKeyDown(KeyCode.D)) menuState += 1;

        SetPosition();

        if (Input.GetKeyDown(KeyCode.Space)) SceneChange(menuState);
    }

    /// <summary>
    /// RETRYステートの処理
    /// </summary>
    private void RETRY_Action()
    {
        if (Input.GetKeyDown(KeyCode.W)) menuState -= 2;
        if (Input.GetKeyDown(KeyCode.A)) menuState -= 1;

        SetPosition();

        if (Input.GetKeyDown(KeyCode.Space)) SceneChange(menuState);
    }

    // メニューの操作
    private void MenuSelect()
    {
        // 項目の更新
        switch (menuState)
        {
            case MenuState.BGM:
                BGM_Action();
                break;
            case MenuState.SE:
                SE_Action();
                break;
            case MenuState.STAGESELECT:
                STAGESELECT_Action();
                break;
            case MenuState.RETRY:
                RETRY_Action();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// どこの項目を選択しているかの表示
    /// </summary>
    private void SetPosition()
    {
        // 項目選択画像の位置の更新
        Vector3 imagePos = new Vector3(menuList[(int)menuState].transform.position.x, menuList[(int)menuState].transform.position.y, selectImage.transform.position.z);
        selectImage.transform.position = imagePos;
    }

    /// <summary>
    /// シーンの変更
    /// </summary>
    /// <param name="_currentState">現在のステート</param>
    private void SceneChange(MenuState _currentState)
    {
        switch (_currentState)
        {
            case MenuState.STAGESELECT:
                SceneChanger.Instance.MoveScene("StageSelect", 0.2f, 0.2f, SceneChangeType.StarBlackFade);
                break;
            case MenuState.RETRY:
                var sceneName = SceneManager.GetActiveScene().name;
                SceneChanger.Instance.MoveScene(sceneName, 0.2f, 0.2f, SceneChangeType.BlackFade);
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
