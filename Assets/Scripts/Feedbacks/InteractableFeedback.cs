using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableFeedback : MonoBehaviour
{
    #region variables in inspector

    [Header("On Pick Rope")]
    [SerializeField] ParticleSystem particlesOnPick = default;
    [SerializeField] AudioStruct soundOnPick = default;

    [Header("On Attach")]
    [SerializeField] ParticleSystem particlesOnAttach = default;
    [SerializeField] AudioStruct soundOnAttach = default;

    [Header("On Detach")]
    [SerializeField] ParticleSystem particlesOnDetach = default;
    [SerializeField] AudioStruct soundOnDetach = default;

    [Header("On Rewind")]
    [SerializeField] ParticleSystem particlesOnRewind = default;
    [SerializeField] AudioStruct soundOnRewind = default;

    #endregion

    #region poolings

    Pooling<ParticleSystem> poolParticlesOnPick = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnPick = new Pooling<AudioSource>();

    Pooling<ParticleSystem> poolParticlesOnAttach = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnAttach = new Pooling<AudioSource>();

    Pooling<ParticleSystem> poolParticlesOnDetach = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnDetach = new Pooling<AudioSource>();

    Pooling<ParticleSystem> poolParticlesOnRewind = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnRewind = new Pooling<AudioSource>();

    #endregion

    Interactable interactable;

    void OnEnable()
    {
        interactable = GetComponent<Interactable>();

        //add events
        if(interactable)
        {
            interactable.onPickRope += OnPickRope;
            interactable.onAttach += OnAttach;
            interactable.onDetach += OnDetach;
            interactable.onRewind += OnRewind;
        }
    }

    void OnDisable()
    {
        //remove events
        if (interactable)
        {
            interactable.onPickRope -= OnPickRope;
            interactable.onAttach -= OnAttach;
            interactable.onDetach -= OnDetach;
            interactable.onRewind -= OnRewind;
        }
    }

    void InstantiateFeedback(Pooling<ParticleSystem> poolParticles, ParticleSystem particle, Pooling<AudioSource> poolSounds, AudioStruct sound, Vector3 position, Quaternion rotation)
    {
        //instantiate particles and sounds
        ParticlesManager.instance.Play(poolParticles, particle, position, rotation);
        SoundManager.instance.Play(poolSounds, sound.audioClip, position, sound.volume);
    }

    #region events

    void OnAttach(Vector3 position, Quaternion rotation)
    {
        InstantiateFeedback(poolParticlesOnAttach, particlesOnAttach, poolSoundsOnAttach, soundOnAttach, position, rotation);
    }

    void OnPickRope(Vector3 position, Quaternion rotation)
    {
        InstantiateFeedback(poolParticlesOnPick, particlesOnPick, poolSoundsOnPick, soundOnPick, position, rotation);
    }

    void OnDetach(Vector3 position, Quaternion rotation)
    {
        InstantiateFeedback(poolParticlesOnDetach, particlesOnDetach, poolSoundsOnDetach, soundOnDetach, position, rotation);
    }

    void OnRewind(Vector3 position, Quaternion rotation)
    {
        InstantiateFeedback(poolParticlesOnRewind, particlesOnRewind, poolSoundsOnRewind, soundOnRewind, position, rotation);
    }

    #endregion
}
