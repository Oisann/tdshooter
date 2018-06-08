using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Oisann.UI {
	public class UIFramesPerSecond : MonoBehaviour {
		public float frequency = 0.5f;
		public int FramesPerSec { get; protected set; }

		private Text textField;
		private string format;

		private void Awake() {
			textField = GetComponent<Text>();

			StartCoroutine(FPS());
		}
 
		private IEnumerator FPS() {
			for(;;){
				int lastFrameCount = Time.frameCount;
				float lastTime = Time.realtimeSinceStartup;
				yield return new WaitForSeconds(frequency);
				float timeSpan = Time.realtimeSinceStartup - lastTime;
				int frameCount = Time.frameCount - lastFrameCount;
	
				// for some reason this is half of what the stats in Unity says, so I double the result until it is resolved.
				FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan) * 2;
				UpdateString();
			}
		}

		private void UpdateString() {
			if(string.IsNullOrEmpty(format))
				format = textField.text;

			textField.text = format.Replace("{$0}", FramesPerSec.ToString("0.####"));
		}
	}
}