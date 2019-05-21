using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class FadeShaderView : MonoBehaviour {

	[Range(0, 1)]
	public float Ratio;

	public Camera TargetCamera;
	public Material Target;

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		Target.SetFloat("_Ratio", Ratio);
		Graphics.Blit(source, destination, Target);
	}
}
