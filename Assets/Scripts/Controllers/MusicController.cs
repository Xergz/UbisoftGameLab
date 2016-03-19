using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour {
	public AudioMixerSnapshot ChasedInSnapshot;
	public AudioMixerSnapshot ChasedOutSnapshot;
	public AudioClip OpenWorldClip;
	public AudioClip LevelClip;


	public AudioSource MainTrackSource;
	public AudioSource SwapTrackSource;

	public float bpm = 128;

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;
	private int m_ChasingEnnemiesCount;
	private bool m_Fade;
	private float m_FadeAcc;

	public void NewEnnemyStartedChasing() {
		m_ChasingEnnemiesCount++;

		if (m_ChasingEnnemiesCount > 0) {
			ChasedInSnapshot.TransitionTo (m_TransitionIn);
		}
	}

	public void EnnemyStoppedChasing() {
		m_ChasingEnnemiesCount--;

		if (m_ChasingEnnemiesCount <= 0) {
			m_ChasingEnnemiesCount = 0;

			ChasedOutSnapshot.TransitionTo (m_TransitionOut);
		}
	}

	/// <summary>
	/// Called when the zone changed
	/// </summary>
	/// <param name="newZone">The new zone where the player entered.</param>
	public void OnZoneChanged(EnumZone newZone) {
		switch (newZone) {
		case EnumZone.OPEN_WORLD:
			TargetClip (OpenWorldClip);
			break;
		case EnumZone.LEVEL_1:
			TargetClip (LevelClip);
			break;
		case EnumZone.LEVEL_2:
			TargetClip (LevelClip);
			break;
		case EnumZone.LEVEL_3:
			TargetClip (LevelClip);
			break;
		case EnumZone.LEVEL_4:
			TargetClip (LevelClip);
			break;
		default:
			break;
		}
	}

	private void TargetClip(AudioClip target) {
		SwapTrackSource.volume = 0;
		SwapTrackSource.clip = target;
		SwapTrackSource.Play ();

		MainTrackSource.volume = 1;
		m_Fade = true;

		m_FadeAcc = 0;
	}

	private void SwapTracks() {
		AudioSource temp = MainTrackSource;
		MainTrackSource = SwapTrackSource;
		SwapTrackSource = temp;

		MainTrackSource.volume = 1;
		SwapTrackSource.volume = 0;

		m_Fade = false;
	}

	// UNITY CALLBACKS
	void Start() {
		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote;
		m_TransitionOut = m_QuarterNote * 2;// * 16;

		m_ChasingEnnemiesCount = 0;
		m_Fade = false;
	}

	void Update() {
		// TODO: Remove these
		if (Input.GetKeyDown ("p")) {
			NewEnnemyStartedChasing ();
		}
		if (Input.GetKeyDown ("l")) {
			EnnemyStoppedChasing ();
		}

		float FadeDuration = 10.0f;

		// Fade musics
		if (m_Fade) {
			SwapTrackSource.volume = Mathf.Lerp (0, 1, m_FadeAcc / FadeDuration);
			MainTrackSource.volume = Mathf.Lerp (1, 0, m_FadeAcc / FadeDuration);

			if (m_FadeAcc >= FadeDuration) {
				SwapTracks ();
			}

			m_FadeAcc += Time.deltaTime;
		}
	}
}
