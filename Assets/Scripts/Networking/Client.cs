using UnityEngine;
using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using Oisann.Player;
using Oisann.UI;
using SimpleJSON;

namespace Oisann.Networking {
	public class Client : MonoBehaviour {

		public static Client instance;

		public string IP = "127.0.0.1";
		public int PORT = 8080;
		public float pingInterval = 1f;

		public long bytesSent = 0;
		public long bytesRecieved = 0;

		[Header("Prefabs")]
		public GameObject soundPrefab;
		public GameObject playerPrefab;

		[Header("Players")]
		public Dictionary<string, Identity> players = new Dictionary<string, Identity>();

		private long pingInMs = 0;

		private float gameLoop = 1f / 64f;
		private float lastPing = 0f;
		private IPEndPoint endPoint;
		private Socket client;
		private Thread receiveThread;

		private Movement playerMovement;
		private Identity identity;

		private void Start() {
			instance = this;
			endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
			client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			client.SendTimeout = 1;
			client.Connect(endPoint);

			receiveThread = new Thread(new ThreadStart(OnRecieve));
        	receiveThread.IsBackground = true;
       		receiveThread.Start();

			playerMovement = FindObjectOfType<Movement>();
			identity = playerMovement.GetComponent<Identity>();

			SendData("r " + identity.username);

			StartCoroutine(Gameloop());
		}

		private void OnDisable() { 
			if(receiveThread != null) 
				receiveThread.Abort(); 

			client.Close();
		} 

		private void OnRecieve() {
			while (true) {
				try {
					byte[] buffer = new byte[1024];
					int iRx = client.Receive(buffer);
					char[] chars = new char[iRx];
					bytesRecieved += chars.LongLength;
					System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
					int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
					System.String text = new System.String(chars);
					//Debug.Log(text);

					string command = text.Split(" ".ToCharArray())[0];
					string rest = text.Replace(command + " ", "");

					UnityMainThreadDispatcher.Instance().Enqueue(ExecuteCommand(command, rest));
				} catch (Exception err) {
					print(err.ToString());
				}
			}
		}

		public IEnumerator ExecuteCommand(string command, string arguments) {
			var json = JSON.Parse(arguments);
			Vector3 pos = Vector3.zero;
			Vector3 rot = Vector3.zero;
			switch (command) {
				case "ping":
					long time = long.Parse(arguments);
					long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					pingInMs = now - time;
					break;
				case "r":
					identity.ID = arguments;
					break;
				case "sound":
					//var json = JSON.Parse(arguments);
					pos = new Vector3(float.Parse(json["pos"]["x"]), float.Parse(json["pos"]["y"]), float.Parse(json["pos"]["z"]));
					GameObject soundGO = Instantiate(soundPrefab, pos, Quaternion.identity) as GameObject;
					SoundController sc = soundGO.GetComponent<SoundController>();
					sc.selected = int.Parse(json["id"]);
					sc.lifespan = int.Parse(json["lifespan"]);
					break;
				case "spawn":
					if(!players.ContainsKey(json["id"])) {
						GameObject spawnGO = Instantiate(playerPrefab, pos, Quaternion.identity) as GameObject;
						Identity spawnID = spawnGO.GetComponent<Identity>();
						spawnID.username = json["username"];
						spawnID.ID = json["id"];
						players.Add(spawnID.ID, spawnID);
					} else {
						Debug.LogWarning("Player (" + json["username"] + ") already exists.");
					}
					break;
				case "move":
					if(players.ContainsKey(json["id"])) {
						Identity playerID;
						if(players.TryGetValue(json["id"], out playerID)) {
							playerID.transform.position = new Vector3(float.Parse(json["pos"]["x"]), float.Parse(json["pos"]["y"]), float.Parse(json["pos"]["z"]));
							playerID.transform.eulerAngles = new Vector3(float.Parse(json["rot"]["x"]), float.Parse(json["rot"]["y"]), float.Parse(json["rot"]["z"]));
						}
					} else if(identity.ID != json["id"]) {
						Debug.LogWarning("Player (" + json["id"] + ") doesn't exists.");
					}
					break;
				default:
					Debug.LogError("Command Error: " + command + " - " + arguments);
					break;
			}
			yield return null;
		}
		
		public void SendData(byte[] bytes) {
			client.SendTo(bytes, endPoint);
			bytesSent += bytes.LongLength;
			//Debug.Log("Sent data to " + IP + ":" + PORT + ". Size: " + bytes.Length);
		}

		public void SendData(string text) {
			SendData(System.Text.UTF8Encoding.UTF8.GetBytes(text));
		}
		private IEnumerator Gameloop() {
			while(string.IsNullOrEmpty(identity.ID)) {
				yield return null;
			}
			while(!string.IsNullOrEmpty(identity.ID)) {
				SendData("p { \"id\": \"" + identity.ID + 
							"\", \"pos\": { \"x\": " + Data.ToStandardNotationString(playerMovement.transform.position.x) +
							", \"y\": " + Data.ToStandardNotationString(playerMovement.transform.position.y) +
							", \"z\": " + Data.ToStandardNotationString(playerMovement.transform.position.z) +
							"}, \"rot\": { \"x\": " + Data.ToStandardNotationString(playerMovement.transform.eulerAngles.x) +
							", \"y\": " + Data.ToStandardNotationString(playerMovement.transform.eulerAngles.y) +
							", \"z\": " + Data.ToStandardNotationString(playerMovement.transform.eulerAngles.z) +
							"} }");
				yield return new WaitForSecondsRealtime(gameLoop);
			}
		}
		
		private void LateUpdate() {
			if(Time.time > lastPing + pingInterval) {
				SendData("ping " + DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
				//SendData("a");
				lastPing = Time.time;
			}
			PingUpdater.instance.UpdatePing(pingInMs);
		}
	}
}