using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour {
	public AudioMixerSnapshot ChasedInSnapshot;
	public AudioMixerSnapshot ChasedOutSnapshot;

	public AudioClip[] OpenWorldClips;
	public AudioClip[] Level1Clips;
	public AudioClip[] Level2Clips;
	public AudioClip[] Level3Clips;

	public AudioClip[] Stings;
	public AudioSource StingSource;

	public AudioSource MainTrackSource;
	public AudioSource SwapTrackSource;

	public float bpm = 128;
	public float FadeDuration = 5.0f;

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;
	private int m_ChasingEnnemiesCount;
	private bool m_Fade;
	private float m_FadeAcc;
	private AudioClip m_NextClip;
	private int[] m_LevelClipIndex;
	private EnumZone m_CurrentZone;

	public void NewEnnemyStartedChasing() {
		m_ChasingEnnemiesCount++;

		if (m_ChasingEnnemiesCount > 0) {
			ChasedInSnapshot.TransitionTo (m_TransitionIn);
			PlaySting();
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
		m_CurrentZone = newZone;

		switch (newZone) {
		case EnumZone.OPEN_WORLD:
			TargetClip (OpenWorldClips[m_LevelClipIndex[0]++ % OpenWorldClips.Length]);
			break;
		case EnumZone.LEVEL_1:
			if(Level1Clips.Length > 0) TargetClip(Level1Clips[m_LevelClipIndex[1]++ % Level1Clips.Length]);
			break;
		case EnumZone.LEVEL_2:
			if(Level2Clips.Length > 0) TargetClip(Level2Clips[m_LevelClipIndex[2]++ % Level2Clips.Length]);
			break;
		case EnumZone.LEVEL_3:
			if(Level3Clips.Length > 0) TargetClip(Level3Clips[m_LevelClipIndex[3]++ % Level3Clips.Length]);
			break;
		default:
			break;
		}
	}

	private void PlaySting()
	{
		if (StingSource && Stings.Length > 0) {
			int randClip = Random.Range (0, Stings.Length);
			StingSource.clip = Stings [randClip];
			StingSource.Play ();
		}
	}

	private void TargetClip(AudioClip target) {

		if (!m_Fade) {
			SwapTrackSource.volume = 0;
			SwapTrackSource.clip = target;
			SwapTrackSource.Play ();

			MainTrackSource.volume = 1;
			m_Fade = true;

			m_FadeAcc = 0;
		} else {
			m_NextClip = target;
		}
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
	void Awake() {
		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote;
		m_TransitionOut = m_QuarterNote * 2;// * 16;

		m_ChasingEnnemiesCount = 0;
		m_Fade = false;

		m_LevelClipIndex = new int[4];
		// Set the starting location for every zones
		m_LevelClipIndex [0] = Random.Range (0, OpenWorldClips.Length);
		m_LevelClipIndex [1] = Random.Range (0, Level1Clips.Length);
		m_LevelClipIndex [2] = Random.Range (0, Level2Clips.Length);
		m_LevelClipIndex [3] = Random.Range (0, Level3Clips.Length);
	}

	void Update() {
		// Fade musics
		if (m_Fade) {
			SwapTrackSource.volume = Mathf.Lerp (0, 1, m_FadeAcc / FadeDuration);
			MainTrackSource.volume = Mathf.Lerp (1, 0, m_FadeAcc / FadeDuration);

			if (m_FadeAcc >= FadeDuration) {
				SwapTracks ();
			}

			m_FadeAcc += Time.deltaTime;
		} else if (!m_Fade && m_NextClip != null) {
			TargetClip (m_NextClip);
			m_NextClip = null;
		} else if (!m_Fade && MainTrackSource.time >= MainTrackSource.clip.length - FadeDuration) {
			OnZoneChanged(m_CurrentZone);
		}
	}
}
