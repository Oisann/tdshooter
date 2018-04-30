using UnityEngine;

public class Bounce : MonoBehaviour {

	private AudioSource audioSource;
	private Rigidbody rb;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody>();
	}
	
	void OnCollisionEnter(Collision coll) {
		audioSource.Play();
		rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
	}
}
