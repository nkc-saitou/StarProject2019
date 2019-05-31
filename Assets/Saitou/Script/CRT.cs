using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CRT : MonoBehaviour
{
    // 自身のAnimator
    [HideInInspector]
    public Animator myAnim;

    // 適用するマテリアル
    [SerializeField]
    Material material;

    // ノイズの揺れ(横)
    [SerializeField]
    [Range(0, 1)]
    float noiseX;
    public float NoiseX { get { return noiseX; } set { noiseX = value; } }

    // ノイズの色
    [SerializeField]
    [Range(0, 1)]
    float rgbNoise;
    public float RGBNoise { get { return rgbNoise; } set { rgbNoise = value; } }

    // 明るさ
    [SerializeField]
    [Range(0, 10)]
    float intencity;
    public float Intencity { get { return intencity; } set { intencity = value; } }

    // 歪みの大きさ
    [SerializeField]
    [Range(0, 1)]
    float sinNoiseScale;
    public float SinNoiseScale { get { return sinNoiseScale; } set { sinNoiseScale = value; } }

    // 歪ませる量
    [SerializeField]
    [Range(0, 10)]
    float sinNoiseWidth;
    public float SinNoiseWidth { get { return sinNoiseWidth; } set { sinNoiseWidth = value; } }

    // ノイズのオフセット
    [SerializeField]
    float sinNoiseOffset;
    public float SinNoiseOffset { get { return sinNoiseOffset; } set { sinNoiseOffset = value; } }

    [SerializeField]
    Vector2 offset;
    public Vector2 Offset { get { return offset; } set { offset = value; } }

    // 線の表示(0以上で表示、最大でなし)
    [SerializeField]
    [Range(0, 2)]
    float scanLineTail = 1.5f;
    public float ScanLineTail { get { return scanLineTail; } set { scanLineTail = value; } }

    // 線の流れるスピード(1でなし)
    [SerializeField]
    [Range(-10, 10)]
    float scanLineSpeed = 10;
    public float ScanLineSpeed { get { return scanLineSpeed; } set { scanLineSpeed = value; } }

    // 透明度
    [SerializeField]
    [Range(-1, 1)]
    float alpha = 1;
    public float Alpha { get { return alpha; } set { alpha = value; } }

    // カメラにアタッチした際に使う用
    //void OnRenderImage(RenderTexture src, RenderTexture dest)
    //{
    //    material.SetFloat("_NoiseX", noiseX);
    //    material.SetFloat("_RGBNoise", rgbNoise);
    //    material.SetFloat("_SinNoiseScale", sinNoiseScale);
    //    material.SetFloat("_SinNoiseWidth", sinNoiseWidth);
    //    material.SetFloat("_SinNoiseOffset", sinNoiseOffset);
    //    material.SetFloat("_ScanLineSpeed", scanLineSpeed);
    //    material.SetFloat("_ScanLineTail", scanLineTail);
    //    material.SetVector("_Offset", offset);
    //    Graphics.Blit(src, dest, material);
    //}

    void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.material = material = Instantiate(material);
    }

    void Update()
    {
        MaterialUpdate();
    }

    /// <summary>
    /// マテリアルのパラメータの更新
    /// </summary>
    void MaterialUpdate()
    {
        material.SetFloat("_NoiseX", noiseX);
        material.SetFloat("_RGBNoise", rgbNoise);
        material.SetFloat("_Intencity", intencity);
        material.SetFloat("_SinNoiseScale", sinNoiseScale);
        material.SetFloat("_SinNoiseWidth", sinNoiseWidth);
        material.SetFloat("_SinNoiseOffset", sinNoiseOffset);
        material.SetFloat("_ScanLineSpeed", scanLineSpeed);
        material.SetFloat("_ScanLineTail", scanLineTail);
        material.SetVector("_Offset", offset);
        material.SetFloat("_Alpha", alpha);
    }
}
