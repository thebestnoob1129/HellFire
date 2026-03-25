using UnityEngine;

namespace CFS
{
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")]
        [SerializeField] private InstantCharacterEffect testEffect;
        [SerializeField] private bool processEffect;

        private void Update()
        {
            if (processEffect)
            {
                processEffect = false;
                var effect = Instantiate(testEffect);
                ProcessInstantEffect(effect);
            }
        }
    }
}