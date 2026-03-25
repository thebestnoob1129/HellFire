using UnityEngine;
using UnityEngine.Serialization;

namespace CFS
{
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager Instance;

        [Header("Damage SFX")]
        public AudioClip[] physicalDamageSFX;

        [Header("Action SFX")]
        public AudioClip rollSFX;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        public AudioClip ChooseRandomSFX(AudioClip[] array)
        {
            var index = Random.Range(0, array.Length);
            return array[index];
        }
    }
}