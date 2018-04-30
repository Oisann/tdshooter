using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {
	public int selected = 0;
	public int lifespan = 10;
	public List<AudioClip> sounds = new List<AudioClip>();

	void Start() {
		AudioSource AS = GetComponent<AudioSource>();
		
		AS.clip = sounds[selected];
		AS.Play();

		Destroy(gameObject, lifespan);
	}
}
