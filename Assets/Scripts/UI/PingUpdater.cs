using UnityEngine;
using UnityEngine.UI;

namespace Oisann.UI {
	public class PingUpdater : MonoBehaviour {
		public static PingUpdater instance;

		private Text textField;

		private string format = "";
		
		private void Awake() {
			instance = this;
			textField = GetComponent<Text>();
		}

		public void UpdatePing(long ms) {
			if(string.IsNullOrEmpty(format))
				format = textField.text;
			textField.text = format.Replace("{$0}", ms.ToString("0.####"));
		}

	}
}
