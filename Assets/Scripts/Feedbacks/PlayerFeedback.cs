using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFeedback : MonoBehaviour
{
    #region variables in inspector

    [Header("On Change Hand")]
    [SerializeField] ParticleSystem particlesOnChangeHand = default;
    [SerializeField] AudioStruct soundOnChangeHand = default;

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

    void OnChangeHand(Transform hand)
    {
        //instantiate particles and sounds
        ParticlesManager.instance.Play(poolParticlesOnChangeHand, particlesOnChangeHand, hand.position, hand.rotation);
        SoundManager.instance.Play(poolSoundsOnChangeHand, soundOnChangeHand.audioClip, hand.position, soundOnChangeHand.volume);
    }

    #endregion
}
