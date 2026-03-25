using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace CFS
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // PROCESS INSTANT EFFECTS (TAKE DAMAGE, HEAL)

        // PROCESS OVER TIME EFFECTS (BURNING, POISONING)

        // PROCESS STATIC EFFECTS (BUFFS, DEFFECTS) FROM Equipment

        private CharacterManager character;

        [Header("VFX")]
        [SerializeField] public GameObject bloodSplatterVFX;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            // Take in an effect
            // Process it
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            if (bloodSplatterVFX)
            {
                var bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else // World Effects Manager default
            {
                var bloodSplatter = Instantiate(WorldCharacterEffectsManager.Instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }



        }

    }
}
