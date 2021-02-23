using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFeedback : MonoBehaviour
{
    #region variables in inspector

    [Header("On Change Hand")]
    [SerializeField] ParticleSystem particlesOnChangeHand = default;
    [SerializeField] AudioStruct soundOnCHangeHand = default;

    #endregion

    #region poolings

    Pooling<ParticleSystem> poolParticlesOnChangeHand = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolSoundsOnChangeHand = new Pooling<AudioSource>();

    #endregion

    Player player;

    private void OnEnable()
    {
        player = GetComponent<Player>();

        //add events
        if(player)
        {
            player.onChangeHand += OnChangeHand;
        }
    }

    private void OnDisable()
    {
        //remove events
        if(player)
        {
            player.onChangeHand -= OnChangeHand;
        }
    }

    #region events

    void OnChangeHand()
    {
        //instantiate particles and sounds
        ParticlesManager.instance.Play(poolParticlesOnChangeHand, particlesOnChangeHand, transform.position, transform.rotation);
        SoundManager.instance.Play(poolSoundsOnChangeHand, soundOnCHangeHand.audioClip, transform.position, soundOnCHangeHand.volume);
    }

    #endregion
}
