using UnityEngine;
using System.Collections;

namespace CFS
{
	[RequireComponent(typeof(AudioSource))]
	public class CharacterSoundFXManager: MonoBehaviour
    {

        private AudioSource audioSource;

        [Header("Damage Grunts")]
        [SerializeField] protected AudioClip[] damageGrunts;

        [Header("Attack Grunts")]
        [SerializeField] protected AudioClip[] attackGrunts;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1;
        }

        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.Instance.rollSFX);
        }

        public void PlaySoundFX(AudioClip soundFx, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
        {
            audioSource.PlayOneShot(soundFx, volume);
            
            // Reset
            audioSource.pitch = 1;

            if (randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
            }
        }

        public virtual void PlayDamageGrunt()
        {
            PlaySoundFX(WorldSoundFXManager.Instance.ChooseRandomSFX(damageGrunts));
        }

        public virtual void PlayAttackGrunt()
        {
            PlaySoundFX(WorldSoundFXManager.Instance.ChooseRandomSFX(attackGrunts));
        }

	}
}