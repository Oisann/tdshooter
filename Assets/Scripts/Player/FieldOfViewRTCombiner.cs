using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FieldOfViewRTCombiner : MonoBehaviour {
	public RenderTexture fieldOfView;
	public RenderTexture insideOfFOW;
	public Shader shader;

	public Color fogColor = Color.black;

	private Material material;

	// Creates a private material used to the effect
	void Awake () {
		material = new Material( shader );
	}
	
	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetTexture("_SourceTex", source);
		material.SetTexture("_fow", fieldOfView);
		material.SetTexture("_ifow", insideOfFOW);
		material.SetColor("_FogColor", fogColor);
		Graphics.Blit(source, destination, material);
	}
}