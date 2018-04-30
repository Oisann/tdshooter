using UnityEngine;
using Oisann.Networking;

namespace Oisann.Player {
	public class Movement : MonoBehaviour {
		public float movementSpeed = 10f;
		public float sprintSpeed = 15f;
		public float stepSoundDistance = .5f;
		public AudioSource footstepSource;

		private Rigidbody rb;
		private Identity identity;
		private float yaw = 0f;
		private Vector3 velocity;

		public float stepDistance = .5f;
		private bool leftFootSound = true;

		private void Start() {
			footstepSource = GetComponentInChildren<AudioSource>();
			rb = GetComponent<Rigidbody>();
			identity = GetComponent<Identity>();
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update() {
			if(identity.isLocalPlayer) {
				yaw += 2f * Input.GetAxis("Mouse X");
				transform.rotation = Quaternion.Euler(0f, yaw, 0f);
				float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : movementSpeed;
				velocity = rb.transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * speed);

				if(Cursor.lockState != CursorLockMode.Locked && Input.GetButton("Fire1")) {
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
		}

		private void FixedUpdate() {
			Vector3 oldPos = rb.position;
			Vector3 newPos = rb.position + velocity * Time.fixedDeltaTime;
			rb.MovePosition(newPos);
			stepDistance -= Vector3.Distance(oldPos, newPos);
			if(stepDistance <= 0f) {
				stepDistance += stepSoundDistance;
				leftFootSound = !leftFootSound;
				if(identity.isLocalPlayer) {
					Client.instance.SendData("sound { \"id\": \"" + identity.ID + 
							"\", \"pos\": { \"x\": " + Data.ToStandardNotationString(newPos.x) +
							", \"y\": " + Data.ToStandardNotationString(newPos.y) +
							", \"z\": " + Data.ToStandardNotationString(newPos.z) +
							"}, \"sound\": " + (leftFootSound ? 0 : 1) + ", \"lifespan\": 1 }");
					footstepSource.Play();
				}
			}

			if(!identity.isLocalPlayer)
				velocity = Vector3.zero;
		}
	}
}