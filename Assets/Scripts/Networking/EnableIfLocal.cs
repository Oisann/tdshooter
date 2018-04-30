using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oisann.Networking {
	public class EnableIfLocal : MonoBehaviour {
		public Behaviour[] behaviours;

		private void Awake() {
			Identity id = GetComponentInParent<Identity>();
			foreach(Behaviour b in behaviours) {
				b.enabled = id.isLocalPlayer;
			}
		}
	}
}
