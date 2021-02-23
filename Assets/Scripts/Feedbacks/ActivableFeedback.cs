using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Activable))]
public class ActivableFeedback : MonoBehaviour
{
    #region variables in inspector

    [Header("On Active")]
    [SerializeField] ParticleSystem particlesOnActive = default;
    [SerializeField] AudioStruct soundOnActive = default;

    [Header("On Deactive")]
    [SerializeField] ParticleSystem particlesOnDeactive = default;
    [SerializeField] AudioStruct soundOnDeactive = default;

    #endregion

    #region poolings

    Pooling<ParticleSystem> poolParticlesOnActive = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnActive = new Pooling<AudioSource>();

    Pooling<ParticleSystem> poolParticlesOnDeactive = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnDeactive = new Pooling<AudioSource>();

    #endregion

    Activable activable;

    private void OnEnable()
    {
        activable = GetComponent<Activable>();

        //add events
        if (activable)
        {
            activable.onActive += OnActive;
            activable.onDeactive += OnDeactive;
        }
    }

    private void OnDisable()
    {
        //remove events
        if (activable)
        {
            activable.onActive -= OnActive;
            activable.onDeactive -= OnDeactive;
        }
    }

    #region events

    void OnActive()
    {
        //instantiate particles and sounds
        ParticlesManager.instance.Play(poolParticlesOnActive, particlesOnActive, transform.position, transform.rotation);
        SoundManager.instance.Play(poolSoundsOnActive, soundOnActive.audioClip, transform.position, soundOnActive.volume);
    }

    void OnDeactive()
    {
        //instantiate particles and sounds
        ParticlesManager.instance.Play(poolParticlesOnDeactive, particlesOnDeactive, transform.position, transform.rotation);
        SoundManager.instance.Play(poolSoundsOnDeactive, soundOnDeactive.audioClip, transform.position, soundOnDeactive.volume);
    }

    #endregion
}
