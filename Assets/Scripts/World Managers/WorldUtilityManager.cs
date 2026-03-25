using Unity.InferenceEngine;
using UnityEngine;

namespace CFS
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager Instance;

        [Header("Layers")]
        [SerializeField] private LayerMask characterLayers;
        [SerializeField] private LayerMask enviroLayers;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }

        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }

        public bool CanIDamageTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if (attackingCharacter == CharacterGroup.Team01)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01: return false;
                    case CharacterGroup.Team02: return true;
                    default: break;
                }
            }
            
            if (attackingCharacter == CharacterGroup.Team02)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01: return true;
                    case CharacterGroup.Team02: return false;
                    default: break;
                }
            }

            return false;

        }

        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
        {
            targetDirection.y = 0;
            var viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection); // Absolute Value Only
            var cross = Vector3.Cross(characterTransform.forward, targetDirection);

            if (cross.y < 0) viewableAngle = -viewableAngle;

            return viewableAngle;
        }

    }
}