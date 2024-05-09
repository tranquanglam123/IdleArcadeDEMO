using TMPro;
using UnityEngine;

namespace DemoGame.Models
{
    public class MoneyStorage : MonoBehaviour, IPickupable
    {
        
        public void PickUp(GameObject pickup)
        {
            GameManager.instance.IncreaseMoney();
            Destroy(pickup);
        }

    }
}
