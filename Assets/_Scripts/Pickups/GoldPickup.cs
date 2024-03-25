using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.AddHTag(HTags.Pickup);
    }
}
