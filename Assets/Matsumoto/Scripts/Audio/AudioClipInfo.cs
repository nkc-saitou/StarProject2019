using System;
using System.Collections.Generic;
using UnityEngine;

namespace Matsumoto.Audio {

	/// <summary>
	/// 再生されるSEの音量の制御を行う
	/// </summary>
	class AudioClipInfo {

		public const int SEMaxConcurrentPlayCount = 50;	//SEの最大同時再生数
		const float VolumeDefaultValue = 0.2f;			//音量の初期値

		public AudioClip Clip;
		public SortedList<int, SEInfo> StockList = new SortedList<int, SEInfo>();

		public float Attenuate = 0.0f;   // 合成時減衰率

		public AudioClipInfo(AudioClip clip) {
			Clip = clip;
			Attenuate = CalcAttenuateRate();
			// create stock list
			for(int i = 0;i < SEMaxConcurrentPlayCount;i++) {
				SEInfo seInfo = new SEInfo(i, 0.0f, Mathf.Pow(Attenuate, i));
				StockList.Add(seInfo.Index, seInfo);
			}
		}

		/// <summary>
		/// 音量の減衰率を求める
		/// </summary>
		/// <returns></returns>
		float CalcAttenuateRate() {
			return NewtonMethod((p) => {
				return (1.0f - Mathf.Pow(p, SEMaxConcurrentPlayCount)) / (1.0f - p) - 1.0f / VolumeDefaultValue;
			},
				(p) => {
					float ip = 1.0f - p;
					float t0 = -SEMaxConcurrentPlayCount * Mathf.Pow(p, SEMaxConcurrentPlayCount - 1.0f) / ip;
					float t1 = (1.0f - Mathf.Pow(p, SEMaxConcurrentPlayCount)) / ip / ip;
					return t0 + t1;
				},
				0.9f, 100
			);
		}

		/// <summary>
		/// ニュートン法で方程式の解を求める
		/// </summary>
		/// <param name="func"></param>
		/// <param name="derive"></param>
		/// <param name="initX"></param>
		/// <param name="maxLoop"></param>
		/// <returns></returns>
		static float NewtonMethod(Func<float, float> func, Func<float, float> derive, float initX, int maxLoop) {
			float x = initX;
			for(int i = 0;i < maxLoop;i++) {
				float curY = func(x);
				if(curY < 0.00001f && curY > -0.00001f)
					break;
				x = x - curY / derive(x);
			}
			return x;
		}
	}
}
