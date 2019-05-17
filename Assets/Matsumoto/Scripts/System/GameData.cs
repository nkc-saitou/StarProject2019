using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// データを保存しておく
/// </summary>
[Serializable]
public class GameData : SingletonMonoBehaviour<GameData> {

	[SerializeField]
	private GameDataContainer _container = new GameDataContainer();

	[SerializeField]
	private static string _jsonText = "";

	public string SavePath {
		get; set;
	}
	
	public void Save() {
		_jsonText = JsonUtility.ToJson(_container);
		Debug.Log(_jsonText);
		File.WriteAllText(GetSaveFilePath(), _jsonText);
	}

	public void Load() {
		_container = JsonUtility.FromJson<GameDataContainer>(GetJson());
	}

	public void Reload() {
		JsonUtility.FromJsonOverwrite(GetJson(), _container);
	}

	public bool GetData<T>(string key, ref T data) {
		return _container.GetData(key, ref data);
	}

	public void SetData<T>(string key, T data) {
		_container.SetData(key, data);
	}

	public bool DeleteData(string key) {
		return _container.DeleteData(key);
	}

	public void DeleteDataAll() {
		_container.DeleteDataAll();
	}

	/// <summary>
	/// 保存しているJsonを取得する
	/// </summary>
	/// <returns></returns>
	private static string GetJson() {
		//既にJsonを取得している場合はそれを返す。
		if(!string.IsNullOrEmpty(_jsonText)) {
			return _jsonText;
		}

		//Jsonを保存している場所のパスを取得。
		string filePath = GetSaveFilePath();

		//Jsonが存在するか調べてから取得し変換する。存在しなければ新たなクラスを作成し、それをJsonに変換する。
		if(File.Exists(filePath)) {
			_jsonText = File.ReadAllText(filePath);
		}
		else {
			_jsonText = JsonUtility.ToJson(new GameDataContainer());
		}

		return _jsonText;
	}

	/// <summary>
	/// 保存する場所のパスを取得。
	/// </summary>
	/// <returns></returns>
	private static string GetSaveFilePath() {

		string filePath = "SaveData";

		//確認しやすいようにエディタではAssetsと同じ階層に保存し、それ以外ではApplication.persistentDataPath以下に保存するように。
		#if UNITY_EDITOR
		filePath += ".json";
		#else
		filePath = Application.persistentDataPath + "/" + filePath;
		#endif

		return filePath;
	}
}

[Serializable]
class GameDataContainer : ISerializationCallbackReceiver {

	private Dictionary<string, string> _dataList
		= new Dictionary<string, string>();

	[SerializeField]
	private string _dictDataJson = "";

	/// <summary>
	/// 引数のオブジェクトをシリアライズして返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj"></param>
	/// <returns></returns>
	private static string Serialize<T>(T obj) {
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, obj);
		return Convert.ToBase64String(memoryStream.GetBuffer());
	}

	/// <summary>
	/// 引数のテキストを指定されたクラスにデシリアライズして返す
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="str"></param>
	/// <returns></returns>
	private static T Deserialize<T>(string str) {
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
		return (T)binaryFormatter.Deserialize(memoryStream);
	}

	public bool GetData<T>(string key, ref T data) {
		if(!_dataList.ContainsKey(key)) return false;

		data = Deserialize<T>(_dataList[key]);
		return true;
	}

	public void SetData<T>(string key, T data) {
		if(!_dataList.ContainsKey(key)) {
			_dataList.Add(key, Serialize(data));
		}
		else {
			_dataList[key] = Serialize(data);
		}
	}

	public bool DeleteData(string key) {
		if(!_dataList.ContainsKey(key)) return false;
		_dataList.Remove(key);
		return true;
	}

	public void DeleteDataAll() {
		_dataList = new Dictionary<string, string>();
	}

	public void OnBeforeSerialize() {
		//Dictionaryはそのままで保存されないので、シリアライズしてテキストで保存。
		_dictDataJson = Serialize(_dataList);
	}

	public void OnAfterDeserialize() {
		//保存されているテキストがあれば、Dictionaryにデシリアライズする。
		if(!string.IsNullOrEmpty(_dictDataJson)) {
			_dataList = Deserialize<Dictionary<string, string>>(_dictDataJson);
		}
	}
}