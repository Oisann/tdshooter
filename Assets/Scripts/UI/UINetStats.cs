using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Oisann.UI {
	public class UINetStats : MonoBehaviour {
		public static UINetStats instance;
		public UpdateGraph graph;
		public bool graphSent = false;
		public bool useBytes = true;

		private Text textField;

		private string format = "";

		private long ping, recieved, sent;
		private long startTime = 0;
		
		private void Awake() {
			startTime = CurrentTime();
			instance = this;
			textField = GetComponent<Text>();
			StartCoroutine(UpdateGraph());
			UpdateString();
		}

		public void UpdatePing(long ms) {
			ping = ms;
			UpdateString();
		}

		public void UpdateRecieved(long bytes) {
			recieved = bytes;
			UpdateString();
		}

		public void UpdateSent(long bytes) {
			sent = bytes;
			UpdateString();
		}

		private IEnumerator UpdateGraph() {
			while(true) {
				yield return new WaitForSeconds(.1f);
				long rps = recieved / (long) Mathf.Max(1, CurrentTime() - startTime);
				long sps = sent / (long) Mathf.Max(1, CurrentTime() - startTime);

				if(graph != null) {
					graph.UpdatePoint(graphSent ? sps : rps);
				}
			}
		}

		private void UpdateString() {
			if(string.IsNullOrEmpty(format))
				format = textField.text;

			long rps = recieved / (long) Mathf.Max(1, CurrentTime() - startTime);
			long sps = sent / (long) Mathf.Max(1, CurrentTime() - startTime);

			textField.text = format.Replace("{$0}", ping.ToString("0.####")).
							Replace("{$1}", formatNetworkTraffic(recieved)).
							Replace("{$2}", formatNetworkTraffic(sent)).
							Replace("{$3}", formatNetworkTraffic(rps)).
							Replace("{$4}", formatNetworkTraffic(sps));
		}

		private long CurrentTime() {
			return (long) Time.time;
		}

		private string formatNetworkTraffic(long bytes) {
			if(useBytes)
				return formatBytes(bytes);
			return formatBits(bytes);
		}

		private string formatBytes(long bytes) {
			return formatData(bytes, new string[]{ "B", "KB", "MB", "GB", "TB" });
		}

		private string formatBits(long bytes) {
			return formatData(bytes * 8, new string[]{ "b", "Kb", "Mb", "Gb", "Tb" });
		}

		private string formatData(long bytes, string[] Suffix, long k = 1000) {
			int i;
			double dblSByte = bytes;
			for (i = 0; i < Suffix.Length && bytes >= k; i++, bytes /= k) {
				dblSByte = bytes / (double) k;
			}
			return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
		}
	}
}
