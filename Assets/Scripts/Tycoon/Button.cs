using TMPro;
using UnityEngine;

public class Button : Machine
{
    [SerializeField] private GameObject controlObject;
    [SerializeField] private int price;
    [SerializeField] private bool forcePrice;

    public TMP_Text priceText;
    // Maybe add discounts

    private Machine _controlMachine;
    private bool _canPurchase;
    
    private void Start()
    {
        Setup();
        if (!controlObject) {Debug.LogError("No Object", gameObject);}
        
        _renderer = GetComponent<Renderer>();
        if (!_renderer) {Debug.LogError("No renderer", gameObject);}

        //Set Object
        controlObject.SetActive(false);
        _controlMachine = controlObject.GetComponent<Machine>();
        //transform.SetParent(null, true);
        
        price = forcePrice ? price : _controlMachine ? _controlMachine.Cost : price;
    }

    private void FixedUpdate()
    {
        if (_controlMachine) { GameUpdate(); }
        
        _canPurchase = Bank.balance >= price;
        _renderer.material.color = _canPurchase && _renderer ? Color.green : Color.red;
        priceText.text = controlObject.name + ": " + price.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        var plr = other.gameObject;

        if (plr == null){ return; }

        if (plr.CompareTag("Player"))
        {

            if (Bank.balance >= price)
            {
                Bank.RemoveCash(price);
                controlObject.SetActive(true);

                Debug.Log(name + " is bought", gameObject);
                Destroy(gameObject);
                return;
            }
        }
    }
}