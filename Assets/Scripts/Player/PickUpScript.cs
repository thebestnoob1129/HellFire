using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static PickUpScript Instance { get; private set; }

    public GameObject player;
    public Transform holdPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }



}
