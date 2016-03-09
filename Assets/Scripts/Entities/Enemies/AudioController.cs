using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

	public List<AudioClip> clipsFar = new List<AudioClip>();
	public List<AudioClip> clipsClose = new List<AudioClip>();

	public AudioSource source;

	public void PlayAudioFar(bool loop = false) {
		int index = Random.Range(0, clipsFar.Count);

		source.clip = clipsFar[index];
		source.loop = loop;

		source.Play();
	}

	public void PlayAudioClose(bool loop = false) {
		int index = Random.Range(0, clipsClose.Count);

		source.clip = clipsClose[index];
		source.loop = loop;

		source.Play();
	}
}
