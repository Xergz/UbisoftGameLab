using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioController : MonoBehaviour
{

    public enum soundType
    {
        far,
        close,
        receiveStun,
        receiveHit,
        enterOpenWorld,
        collectFragment,
        death,
        collectLife,
        collision,
        useBoost,
        reverseStream,
        spotFragment
    }

    public List<AudioClip> clipsFar = new List<AudioClip>();
    public List<AudioClip> clipsClose = new List<AudioClip>();
    public List<AudioClip> clipsReceiveStun = new List<AudioClip>();
    public List<AudioClip> clipsReceiveHit = new List<AudioClip>();
    public List<AudioClip> clipsEnterOpenWorld = new List<AudioClip>();
    public List<AudioClip> clipsCollectFragments = new List<AudioClip>();
    public List<AudioClip> clipsDeath = new List<AudioClip>();
    public List<AudioClip> clipsCollectLife = new List<AudioClip>();
    public List<AudioClip> clipsCollision = new List<AudioClip>();
    public List<AudioClip> clipsUseBoost = new List<AudioClip>();
    public List<AudioClip> clipsReverseStream = new List<AudioClip>();
    public List<AudioClip> clipsSpotFragment = new List<AudioClip>();

    //Pass parameters to coroutine
    private float _delay = 0.0f;
    private float _volume = 0.0f;
    private bool _loop = false;
    private soundType _type;

    public AudioSource source;

    public void PlayAudio(soundType type, bool loop = false, float delay = 0.0f, float volume = 1.0f)
    {
        if (delay > 0.0f)
        {
            _type = type;
            _loop = loop;
            _delay = delay;
            _volume = volume;
            StartCoroutine("PlayDelayedAudio");
        }
        else
            PlaySoundType(type, loop, volume);
    }

    public void PlaySpecificAudio(AudioClip clip, bool loop)
    {
        source.clip = clip;
        source.loop = loop;

        source.Play();
    }

    public void PlaySoundType(soundType type, bool loop, float volume)
    {
        switch (type)
        {
            case soundType.far:
                playSoundFromList(clipsFar, loop, volume);
                break;
            case soundType.close:
                playSoundFromList(clipsClose, loop, volume);
                break;
            case soundType.receiveHit:
                playSoundFromList(clipsReceiveHit, loop, volume);
                break;
            case soundType.receiveStun:
                playSoundFromList(clipsReceiveStun, loop, volume);
                break;
            case soundType.enterOpenWorld:
                playSoundFromList(clipsEnterOpenWorld, loop, volume);
                break;
            case soundType.collectFragment:
                playSoundFromList(clipsCollectFragments, loop, volume);
                break;
            case soundType.death:
                playSoundFromList(clipsDeath, loop, volume);
                break;
            case soundType.collectLife:
                playSoundFromList(clipsCollectLife, loop, volume);
                break;
            case soundType.collision:
                playSoundFromList(clipsCollision, loop, volume);
                break;
            case soundType.useBoost:
                playSoundFromList(clipsUseBoost, loop, volume);
                break;
            case soundType.reverseStream:
                playSoundFromList(clipsReverseStream, loop, volume);
                break;
            case soundType.spotFragment:
                playSoundFromList(clipsSpotFragment, loop, volume);
                break;
            default: break;
        }
    }

    public IEnumerator PlayDelayedAudio()
    {
        yield return new WaitForSeconds(_delay);
        PlaySoundType(_type, _loop, _volume);
    }

    public void playSoundFromList(List<AudioClip> list, bool loop, float volume)
    {
        if (list.Count > 0)
        {
            int index = Random.Range(0, list.Count);

            source.volume = volume;
            source.clip = list[index];
            source.loop = loop;

            source.Play();
        }
    }


}
