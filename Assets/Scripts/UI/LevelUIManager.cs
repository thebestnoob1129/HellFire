using UnityEngine;

namespace CFS
{
    public class LevelUIManager : MonoBehaviour
    {

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void StartWorld()
        {
            WorldManager.Instance.StartGame();
        }
    }
}
