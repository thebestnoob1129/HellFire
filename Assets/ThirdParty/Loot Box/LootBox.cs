using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [SerializeField] private int xp;
    [SerializeField] private int coins;

    [SerializeField] private Item[] possibleItems;
    [SerializeField] private PlayerManager player;

    public void Start()
    {
        player = FindFirstObjectByType<PlayerManager>();
    }

    public void Interact()
    {
        //player.AddExperience(xp);
        //player.AddCoins(coins);
        
        var rewardedItem = possibleItems[Random.Range(0, possibleItems.Length)];
        player.AddItem(rewardedItem);
        Destroy(gameObject, 1f);
    }

}
