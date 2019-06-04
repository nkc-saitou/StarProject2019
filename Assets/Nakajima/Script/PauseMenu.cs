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

    // StageControllerのインスタンス
    private StageController stageCon;

    // 項目の位置
    [SerializeField]
    private GameObject[] menuList;

    // BGM,SEの音量調整用のSlider
    [SerializeField]
    private Slider[] volumeSlider; 

    // 選択項目イメージ
    [SerializeField]
    private GameObject selectImage;

    // 入力値
    Vector2 inputVec;
    // 押し続けた時間
    private float axisTime; 
    
	void Start () {
    }
	
	void Update () {
        MenuSelect();
	}

    /// <summary>
    /// 表示にされたとき
    /// </summary>
    void OnEnable()
    {
        MenuSound();
    }
    /// <summary>
    /// 非表示にされたとき
    /// </summary>
    void OnDisable()
    {
        MenuSound();
    }
    /// <summary>
    /// メニュー表示、非表示音
    /// </summary>
    void MenuSound()
    {
        // 音の鳴る位置をプレイヤーの位置にする
        var player = FindObjectOfType<Matsumoto.Character.Player>();
        if (player == null) return;

        // プレイヤーが居る場合のみ音を鳴らす
        var SoundPos = player.transform.position;
        Matsumoto.Audio.AudioManager.PlaySE("OpenMenu", position: SoundPos);
    }

    /// <summary>
    /// StageControllerのセットアップ
    /// </summary>
    /// <param name="_stageCon">StageControllerのインスタンス</param>
    public void SetStageController(StageController _stageCon)
    {
        stageCon = _stageCon;
    }

    /// <summary>
    /// BGMステートの処理
    /// </summary>
    private void BGM_Action()
    {
        // ステートの更新
        if (inputVec.y < 0.0f && axisTime == 0.0f) menuState += 1;

        // 音量調整
        if (inputVec.x < 0.0f && axisTime == 0.0f || inputVec.x < 0.0f && axisTime > 0.25f) VolumeChange(menuState, -0.1f);
        if (inputVec.x > 0.0f && axisTime == 0.0f || inputVec.x > 0.0f && axisTime > 0.25f) VolumeChange(menuState, 0.1f);

        SetPosition();
    }

    /// <summary>
    /// SEステートの処理
    /// </summary>
    private void SE_Action()
    {
        // ステートの更新
        if (inputVec.y < 0.0f && axisTime == 0.0f) menuState += 1;
        if (inputVec.y > 0.0f && axisTime == 0.0f) menuState -= 1;

        // 音量調整
        if (inputVec.x < 0.0f && axisTime == 0.0f || inputVec.x < 0.0f && axisTime > 0.25f) VolumeChange(menuState, -0.1f);
        if (inputVec.x > 0.0f && axisTime == 0.0f || inputVec.x > 0.0f && axisTime > 0.25f) VolumeChange(menuState, 0.1f);

        SetPosition();
    }

    /// <summary>
    /// STAGESELECTステートの処理
    /// </summary>
    private void STAGESELECT_Action()
    {
        // ステートの更新
        if (inputVec.y > 0.0f && axisTime == 0.0f) menuState -= 1;
        if (inputVec.x > 0.0f && axisTime == 0.0f) menuState += 1;

        SetPosition();

        // ステージセレクト
        if (Input.GetButtonDown("Attack")) SceneChange(menuState);
    }

    /// <summary>
    /// RETRYステートの処理
    /// </summary>
    private void RETRY_Action()
    {
        // ステートの更新
        if (inputVec.y > 0.0f && axisTime == 0.0f) menuState -= 2;
        if (inputVec.x < 0.0f && axisTime == 0.0f) menuState -= 1;

        SetPosition();

        // リトライ
        if (Input.GetButtonDown("Attack")) SceneChange(menuState);
    }

    // メニューの操作
    private void MenuSelect()
    {
        // 入力値を格納
        inputVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 項目ごとの処理を実行
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

        // インプットの入力時間の更新
        axisTime = GetInputTime();
    }

    /// <summary>
    /// 入力時間を返す
    /// </summary>
    /// <returns>入力し続けている時間</returns>
    float GetInputTime()
    {
        // 入力がないなら0を返す
        if (inputVec == Vector2.zero) return 0.0f;

        // 時間の更新
        var time = axisTime;
        time += Time.unscaledDeltaTime;

        return time;
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
        // シーン遷移中はポーズ不可
        stageCon.CanPause = false;

        var SoundPos = FindObjectOfType<Matsumoto.Character.Player>().transform.position;
        Matsumoto.Audio.AudioManager.PlaySE("MenuSelect_3", position: SoundPos);

        // ステートごとにシーン遷移
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
        // Sliderの見た目の更新
        volumeSlider[(int)_currentState].value += _value;

        // 入力時間をリセット
        axisTime = 0.0f;

        // ステートに合わせて音量調整
        switch (_currentState)
        {
            case MenuState.BGM:
                Matsumoto.Audio.AudioManager.SetBGMVolume(volumeSlider[(int)_currentState].value);
                break;
            case MenuState.SE:
                Matsumoto.Audio.AudioManager.SetSEVolume(volumeSlider[(int)_currentState].value);
                break;
        }
    } 
}
