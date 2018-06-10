using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Oisann.Networking;

namespace Oisann.UI {
	public class UICompass : MonoBehaviour {
		public Identity localPlayer;

		private Text textField;

		private void Awake() {
			textField = GetComponent<Text>();
			localPlayer = GameObject.FindObjectsOfType<Identity>().FirstOrDefault(component => component.isLocalPlayer);
		}

		private void FixedUpdate() {
			int rot = Mathf.RoundToInt(localPlayer.transform.eulerAngles.y);
			textField.text = rot + "";
		}
	}
}