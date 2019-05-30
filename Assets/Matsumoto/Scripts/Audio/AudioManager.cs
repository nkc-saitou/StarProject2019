using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace Matsumoto.Audio {

	/// <summary>
	/// 音の管理をする
	/// </summary>
	public sealed class AudioManager : SingletonMonoBehaviour<AudioManager> {

		private const string MixerPath = "Sounds/MainAudioMixer";	//ミキサーのパス
		private const string BGMPath = "Sounds/BGM/";               //BGMのフォルダーパス
		private const string SEPath = "Sounds/SE/";                 //SEのフォルダーパス

		private readonly AudioMixerGroup[] _mixerGroups = new AudioMixerGroup[2];//ミキサーのグループ [0]SE [1]BGM

		private float _masterVolumeRatio = 1;
		private float _SEVolumeRatio = 1;
		private float _BGMVolumeRatio = 1;

		private AudioSource _SESourcePrefab;
		private AudioSource _BGMSourcePrefab;

		private Dictionary<string, AudioClipInfo> _SEclips;			//SE再生用リスト
		private Dictionary<string, AudioClip> _BGMclips;			//BGM再生用リスト

		private AudioSource _currentPlayingBGM;						//現在再生されているBGM
		private string _currentPlayedBGMName = "";					//再生されているBGMの名前

		private Coroutine _fadeInCol;								//BGMフェードインのコルーチン
		private AudioSource _fadeInAudio;							//BGMフェードイン用のAudioSource

		public AudioMixer Mixer { get; private set; }				//ミキサー

		/// <summary>
		/// 各音情報を読み込み
		/// </summary>
		public static void Load() {

			// LoadPrefab
			Instance._SESourcePrefab = Resources.Load<AudioSource>("Sounds/SESourcePrefab");
			Instance._BGMSourcePrefab = Resources.Load<AudioSource>("Sounds/BGMSourcePrefab");

			// LoadMixer
			Instance.Mixer = Resources.Load<AudioMixer>(MixerPath);
			if(Instance.Mixer) {
				Instance._mixerGroups[0] = Instance.Mixer.FindMatchingGroups("SE")[0];
				Instance._mixerGroups[1] = Instance.Mixer.FindMatchingGroups("BGM")[0];
			}
			else {
				Debug.LogError("Failed Load AudioMixer! Path=" + MixerPath);
			}


			// BGM読み込み
			Instance._BGMclips = new Dictionary<string, AudioClip>();
			foreach(var item in Resources.LoadAll<AudioClip>(BGMPath)) {
				Instance._BGMclips.Add(item.name, item);
			}

			// SE読み込み
			Instance._SEclips = new Dictionary<string, AudioClipInfo>();
			foreach(var item in Resources.LoadAll<AudioClip>(SEPath)) {
				Instance._SEclips.Add(item.name, new AudioClipInfo(item));
			}

		}

		public static bool IsPlayingBGM()
		{
			if (Instance == null) return false;
			if (Instance._currentPlayingBGM == null) return false;
			
			return Instance._currentPlayingBGM.isPlaying;
		}

		/// <summary>
		/// SEを再生する
		/// </summary>
		/// <param name="type">SEの名前</param>
		/// <param name="vol">音量</param>
		/// <param name="autoDelete">再生終了時にSEを削除するか</param>
		/// <returns>再生しているSE</returns>
		public static AudioSource PlaySE(string SEName, float vol = 1.0f, bool autoDelete = true, Vector3 position = new Vector3()) {

			// SE取得
			var info = GetSEInfo(SEName);
			if(info == null) return null;

			if(info.StockList.Count > 0) {
				// stockListから空で且つ番号が一番若いSEInfoを受け取る
				var seInfo = info.StockList.Values[0];

				// ストックを削除
				info.StockList.Remove(seInfo.Index);

				// 情報を取り付ける
				// Poolしたい
				var src = Instantiate(Instance._SESourcePrefab);
				src.name = "[Audio BGM - " + SEName + "]";
				src.transform.SetParent(Instance.transform);
				src.transform.position = position;
				src.clip = info.Clip;
				src.volume = seInfo.Volume * vol;
				src.outputAudioMixerGroup = Instance._mixerGroups[0];
				src.Play();

				// 管理用情報を付加
				var playSE = src.gameObject.AddComponent<PlayingSE>();
				playSE.OnDestroyEvent += () => { info.StockList.Add(seInfo.Index, seInfo); };

				// 自動削除の場合は遅延で削除を実行する
				if(autoDelete)
					Destroy(src.gameObject, src.clip.length + 0.1f);

				return src;
			}

			return null;
		}

		/// <summary>
		/// BGMを再生する
		/// </summary>
		/// <param name="BGMName">BGMの名前</param>
		/// <param name="vol">音量</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <returns>再生しているSE</returns>
		public static AudioSource PlayBGM(string BGMName, float vol = 1.0f, bool isLoop = true, Vector3 position = new Vector3()) {

			// BGM取得
			var clip = GetBGM(BGMName);
			if(!clip) return null;
			if(Instance._currentPlayingBGM) Destroy(Instance._currentPlayingBGM.gameObject);

			var src = Instantiate(Instance._BGMSourcePrefab);
			src.name = "[Audio BGM - " + BGMName + "]";
			src.transform.SetParent(Instance.transform);
			src.transform.position = position;
			src.clip = clip;
			src.volume = vol;
			src.outputAudioMixerGroup = Instance._mixerGroups[1];
			src.Play();

			if(isLoop) {
				src.loop = true;
			}
			else {
				Destroy(src.gameObject, clip.length + 0.1f);
			}

			Instance._currentPlayingBGM = src;
			Instance._currentPlayedBGMName = BGMName;

			return src;
		}

		/// <summary>
		/// マスターのボリュームを返す
		/// </summary>
		/// <param name="ratio"></param>
		public static float GetMasterVolume() {
			return Instance._masterVolumeRatio;
		}

		/// <summary>
		/// マスターのボリュームを設定
		/// </summary>
		/// <param name="ratio"></param>
		public static void SetMasterVolume(float ratio) {
			Instance.Mixer
				.SetFloat("MasterVolume", Instance._masterVolumeRatio = PowerRatioToDecibels(ratio));
		}

		/// <summary>
		/// SEのボリュームを返す
		/// </summary>
		/// <param name="ratio"></param>
		public static float GetSEVolume() {
			return Instance._SEVolumeRatio;
		}

		/// <summary>
		/// SEのボリュームを設定
		/// </summary>
		/// <param name="ratio"></param>
		public static void SetSEVolume(float ratio) {
			Instance.Mixer
				.SetFloat("SEVolume", Instance._SEVolumeRatio = PowerRatioToDecibels(ratio));
		}

		/// <summary>
		/// BGMのボリュームを返す
		/// </summary>
		/// <param name="ratio"></param>
		public static float GetBGMVolume() {
			return Instance._BGMVolumeRatio;
		}

		/// <summary>
		/// BGMのボリュームを設定
		/// </summary>
		/// <param name="ratio"></param>
		public static void SetBGMVolume(float ratio) {
			Instance.Mixer
				.SetFloat("BGMVolume", Instance._BGMVolumeRatio = PowerRatioToDecibels(ratio));
		}

		/// <summary>
		/// BGMをフェードインさせる
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="type">新しいBGMのタイプ</param>
		/// <param name="vol">新しいBGMの大きさ</param>
		/// <param name="isLoop">新しいBGMがループするか</param>
		public static void FadeIn(float fadeTime, string BGMName, float vol = 1.0f, bool isLoop = true) {
			Instance._fadeInCol = Instance.StartCoroutine(Instance.FadeInAnim(fadeTime, BGMName, vol, isLoop));
		}

		/// <summary>
		/// BGMをフェードアウトさせる
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		public static void FadeOut(float fadeTime) {
			Instance.StartCoroutine(Instance.FadeOutAnim(fadeTime));
		}

		/// <summary>
		/// BGMをクロスフェードする
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="type">新しいBGMのタイプ</param>
		/// <param name="vol">新しいBGMの大きさ</param>
		/// <param name="isLoop">新しいBGMがループするか</param>
		public static void CrossFade(float fadeTime, string fadeInBGMName, float vol = 1.0f, bool isLoop = true) {
			Instance.StartCoroutine(Instance.FadeOutAnim(fadeTime));
			Instance._fadeInCol = Instance.StartCoroutine(Instance.FadeInAnim(fadeTime, fadeInBGMName, vol, isLoop));
		}

		protected override void Init() {
			base.Init();

			Load();
		}

		/// <summary>
		/// SEを取得する
		/// </summary>
		/// <param name="SEName">SEの名前</param>
		/// <returns>SE</returns>
		static AudioClipInfo GetSEInfo(string SEName) {

			if(!Instance._SEclips.ContainsKey(SEName)) {
				Debug.LogWarning("SEName:" + SEName + " is not found.");
				return null;
			}
			return Instance._SEclips[SEName];
		}

		/// <summary>
		/// BGMを取得する
		/// </summary>
		/// <param name="BGMName">BGMの名前</param>
		/// <returns>BGM</returns>
		static AudioClip GetBGM(string BGMName) {

			if(!Instance._BGMclips.ContainsKey(BGMName)) {
				Debug.LogError("BGMName:" + BGMName + " is not found.");
				return null;
			}
			return Instance._BGMclips[BGMName];
		}

		private static float PowerRatioToDecibels(float power) {
			if (power == 0) return -80;
			return 20 * Mathf.Log10(power);
		}

		[ContextMenu("DebugShowLoadedSE")]
		void DebugShowLoadedSE() {
			foreach(var clip in _SEclips) {
				Debug.Log(clip.Key);
			}
		}

		[ContextMenu("DebugShowLoadedBGM")]
		void DebugShowLoadedBGM() {
			foreach(var clip in _BGMclips) {
				Debug.Log(clip.Key);
			}
		}

		/// <summary>
		/// フェードインを行う
		/// </summary>
		/// <param name="fadeTime"></param>
		/// <param name="BGMName"></param>
		/// <param name="vol"></param>
		/// <param name="isLoop"></param>
		/// <returns></returns>
		IEnumerator FadeInAnim(float fadeTime, string BGMName, float vol, bool isLoop) {

			// BGM取得
			var clip = GetBGM(BGMName);
			if(!clip) yield break;

			// 初期設定
			_fadeInAudio = new GameObject("[Audio BGM - " + BGMName + " - FadeIn ]").AddComponent<AudioSource>();
			_fadeInAudio.transform.SetParent(Instance.transform);
			_fadeInAudio.clip = clip;
			_fadeInAudio.volume = 0;
			_fadeInAudio.outputAudioMixerGroup = _mixerGroups[1];
			_fadeInAudio.Play();

			// フェードイン
			var t = 0.0f;
			while((t += Time.deltaTime / fadeTime) < 1.0f) {
				_fadeInAudio.volume = t * vol;
				yield return null;
			}

			_fadeInAudio.volume = vol;
			_fadeInAudio.name = "[Audio BGM - " + BGMName + "]";

			if(_currentPlayingBGM) Destroy(_currentPlayingBGM.gameObject);

			if(isLoop) {
				_fadeInAudio.loop = true;
			}
			else {
				Destroy(_fadeInAudio.gameObject, clip.length + 0.1f);
			}

			_currentPlayingBGM = _fadeInAudio;
		}

		/// <summary>
		/// フェードアウトを行う
		/// </summary>
		/// <param name="fadeTime"></param>
		/// <returns></returns>
		IEnumerator FadeOutAnim(float fadeTime) {

			var src = _currentPlayingBGM;

			// フェードイン中にフェードアウトが呼ばれた場合
			if(!src) {
				// フェードイン処理停止
				Instance.StopCoroutine(_fadeInCol);
				src = _fadeInAudio;

				if(!src) yield break;
			}

			src.name = "[Audio BGM - " + _currentPlayedBGMName + " - FadeOut ]";
			_currentPlayingBGM = null;

			// フェードアウト
			var t = 0.0f;
			float vol = src.volume;
			while((t += Time.deltaTime / fadeTime) < 1.0f) {
				src.volume = (1 - t) * vol;
				yield return null;
			}

			Destroy(src.gameObject);
		}
	}
}
