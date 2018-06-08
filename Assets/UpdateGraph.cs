using UnityEngine;
using Oisann.UI;

public class UpdateGraph : MonoBehaviour {
	private UILineRenderer lineRenderer;
	private int arrayLength = 0;

	private void Awake() {
		lineRenderer = GetComponent<UILineRenderer>();
		arrayLength = lineRenderer.Points.Length;
	}

	public void UpdatePoint(long bytes) {
		float lastData = bytes / 10f;
		for(int i = 0; i < arrayLength; i++) {
			Vector2 currentData = new Vector2(i * 10, lastData);
			lastData = lineRenderer.Points[i].y;
			lineRenderer.Points[i] = currentData;
		}
		lineRenderer.SetVerticesDirty();
	}
}
