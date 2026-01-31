using UnityEngine;

public class Collector : Machine
{
    private void Start() => Setup();
    private void FixedUpdate() => GameUpdate();

    private void OnCollisionEnter(Collision collision)
    {
        var collider = collision.gameObject;
        if (collider.GetComponent<Valuable>())
        {
            var val = collider.GetComponent<Valuable>(); 
            this.Bank.AddCash(val);
            
            Destroy(collider);
        }
    }
}
