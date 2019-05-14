using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// データを保存しておく
/// 参考
/// http://kan-kikuchi.hatenablog.com/entry/Json_SaveData
/// </summary>
public class GameData : SingletonMonoBehaviour<GameData> {

	private Dictionary<string, object> _dataList;


	public string SavePath {
		get; set;
	}
	
	public void Save() {
		//JsonUtility.
	}

	public void Load() {
	
	}

	public bool SetData<T> (string key, T data) {
		if(_dataList.ContainsKey(key)) return false;

		_dataList.Add(key, data);
		return true;
	}

	public bool GetData<T>(string key, ref T data) {
		if(!_dataList.ContainsKey(key)) return false;

		data = (T)_dataList[key];
		return true;
	}
}
