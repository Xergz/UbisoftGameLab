using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class MusicController : MonoBehaviour {
	public AudioMixerSnapshot ChasedInSnapshot;
	public AudioMixerSnapshot ChasedOutSnapshot;

	public float bpm = 128;

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;
	private int m_ChasingEnnemiesCount;

	/// <summary>
	/// Called when the zone changed
	/// </summary>
	/// <param name="newZone">The new zone where the player entered.</param>
	public void OnZoneChanged(EnumZone newZone) {
		// TODO: Fade audio clip
	}

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

	// UNITY CALLBACKS
	void Start() {
		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote;
		m_TransitionOut = m_QuarterNote * 2;// * 16;

		m_ChasingEnnemiesCount = 0;
	}

	void Update() {
		if (Input.GetKeyDown ("p")) {
			NewEnnemyStartedChasing ();
		}
		if (Input.GetKeyDown ("l")) {
			EnnemyStoppedChasing ();
		}
	}
}
