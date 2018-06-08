using UnityEngine;
using Oisann.Networking;

public class LocalPlayerLayer : MonoBehaviour {
	public LayerMask localLayer;
	public LayerMask remoteLayer;

	void Start() {
		int local = layermask_to_layer(localLayer);
		int remote = layermask_to_layer(remoteLayer);
		int layer = GetComponent<Identity>().isLocalPlayer ? local : remote;
		int opposite = layer == local ? remote : local;

		gameObject.layer = layer;
		foreach(Transform trans in gameObject.GetComponentsInChildren<Transform>()) {
			if(trans.gameObject.layer == opposite)
				trans.gameObject.layer = layer;
		}

		Destroy(this);
	}

	private int layermask_to_layer(LayerMask layerMask) {
         int layerNumber = 0;
         int layer = layerMask.value;
         while(layer > 0) {
             layer = layer >> 1;
             layerNumber++;
         }
         return layerNumber - 1;
     }
}
