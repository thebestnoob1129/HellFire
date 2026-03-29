using System.Collections.Generic;
using UnityEngine;

namespace CFS
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager Instance { get; private set; }

        [Header("VFX")]
        public GameObject bloodSplatterVFX;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;

        [SerializeField] private List<InstantCharacterEffect> instantEffects;

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

            GenerateEffectIDs();
            DontDestroyOnLoad(gameObject);
        }

        private void GenerateEffectIDs()
        {
            for (var i = 0; i < instantEffects.Count; i++)
            {
                instantEffects[i].instantEffectID = i;
            }
        }

    }
}