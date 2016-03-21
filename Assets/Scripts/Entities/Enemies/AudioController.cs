using UnityEngine;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {

    public enum soundType{
        far,
        close,
        receiveStun,
        receiveHit,
        enterOpenWorld,
        collectFragment
    }

	public List<AudioClip> clipsFar = new List<AudioClip>();
	public List<AudioClip> clipsClose = new List<AudioClip>();
    public List<AudioClip> clipsReceiveStun = new List<AudioClip>();
    public List<AudioClip> clipsReceiveHit = new List<AudioClip>();
    public List<AudioClip> clipsEnterOpenWorld = new List<AudioClip>();
    public List<AudioClip> clipsCollectFragments = new List<AudioClip>();

    public AudioSource source;

    public void PlayAudio(soundType type, bool loop = false)
    {
        switch (type)
        {
            case soundType.far : PlayAudioFar(loop);
                break;
            case soundType.close : PlayAudioClose(loop);
                break;
            case soundType.receiveHit: PlayAudioReceiveHit(loop);
                break;
            case soundType.receiveStun: PlayAudioReceiveStun(loop);
                break;
            case soundType.enterOpenWorld:
                PlayAudioEnterOpenWorld(loop);
                break;
            case soundType.collectFragment:
                PlayAudioCollectFragment(loop);
                break;
        }
    }

    public void PlayAudioFar(bool loop)
    {

        if (clipsFar.Count > 0)
        {
            int index = Random.Range(0, clipsFar.Count);

            source.clip = clipsFar[index];
            source.loop = loop;

            source.Play();
        }

    }

	public void PlayAudioClose(bool loop) {
        if (clipsClose.Count > 0)
        {
            int index = Random.Range(0, clipsClose.Count);

            source.clip = clipsClose[index];
            source.loop = loop;

            source.Play();
        }
	}

    public void PlayAudioReceiveHit(bool loop)
    {
        if (clipsReceiveHit.Count > 0)
        {
            int index = Random.Range(0, clipsReceiveHit.Count);

            source.clip = clipsReceiveHit[index];
            source.loop = loop;

            source.Play();
        }
    }

    public void PlayAudioReceiveStun(bool loop)
    {
        if (clipsReceiveStun.Count > 0)
        {
            int index = Random.Range(0, clipsReceiveStun.Count);

            source.clip = clipsReceiveStun[index];
            source.loop = loop;

            source.Play();
        }
    }
    public void PlayAudioEnterOpenWorld(bool loop)
    {
        if (clipsEnterOpenWorld.Count > 0)
        {
            int index = Random.Range(0, clipsEnterOpenWorld.Count);

            source.clip = clipsEnterOpenWorld[index];
            source.loop = loop;

            source.Play();
        }
    }
    public void PlayAudioCollectFragment(bool loop)
    {
        if (clipsCollectFragments.Count > 0)
        {
            int index = Random.Range(0, clipsCollectFragments.Count);

            source.clip = clipsCollectFragments[index];
            source.loop = loop;

            source.Play();
        }
    }


}
