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
    private bool _loop = false;
    private soundType _type;

    public AudioSource source;

    public void PlayAudio(soundType type, bool loop = false, float delay = 0.0f)
    {
        if (delay > 0.0f)
        {
            _type = type;
            _loop = loop;
            _delay = delay;
            StartCoroutine("PlayDelayedAudio");
        }
        else
            PlaySoundType(type, loop);
    }

    public void PlaySpecificAudio(AudioClip clip, bool loop)
    {
        source.clip = clip;
        source.loop = loop;

        source.Play();
    }

    public void PlaySoundType(soundType type, bool loop)
    {
        switch (type)
        {
            case soundType.far:
                playSoundFromList(clipsFar, loop);
                break;
            case soundType.close:
                playSoundFromList(clipsClose, loop);
                break;
            case soundType.receiveHit:
                playSoundFromList(clipsReceiveHit, loop);
                break;
            case soundType.receiveStun:
                playSoundFromList(clipsReceiveStun, loop);
                break;
            case soundType.enterOpenWorld:
                playSoundFromList(clipsEnterOpenWorld, loop);
                break;
            case soundType.collectFragment:
                playSoundFromList(clipsCollectFragments, loop);
                break;
            case soundType.death:
                playSoundFromList(clipsDeath, loop);
                break;
            case soundType.collectLife:
                playSoundFromList(clipsCollectLife, loop);
                break;
            case soundType.collision:
                playSoundFromList(clipsCollision, loop);
                break;
            case soundType.useBoost:
                playSoundFromList(clipsUseBoost, loop);
                break;
            case soundType.reverseStream:
                playSoundFromList(clipsReverseStream, loop);
                break;
            case soundType.spotFragment:
                playSoundFromList(clipsSpotFragment, loop);
                break;
            default: break;
        }
    }

    public IEnumerator PlayDelayedAudio()
    {
        yield return new WaitForSeconds(_delay);
        PlaySoundType(_type, _loop);
    }

    public void playSoundFromList(List<AudioClip> list, bool loop)
    {
        if (list.Count > 0)
        {
            int index = Random.Range(0, list.Count);

            source.clip = list[index];
            source.loop = loop;

            source.Play();
        }
    }


}
