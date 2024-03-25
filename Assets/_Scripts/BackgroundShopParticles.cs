using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BackgroundShopParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    
    // private void OnEnable()
    // {
    //     E.Shop.OnOpenShop.OnRaise += OnOpenShop;
    //     E.Shop.OnCloseShop.OnRaise += OnCloseShop;
    // }
    //
    // private void OnOpenShop(E.Shop.OpenShopParams openShopParams)
    // {
    //     _particles.Play();
    // }
    //
    // private void OnCloseShop(E.Shop.CloseShopParams closeShopParams)
    // {
    //     _particles.Stop();
    // }
    //
    // private void OnDisable()
    // {
    //     E.Shop.OnOpenShop.OnRaise -= OnOpenShop;
    //     E.Shop.OnCloseShop.OnRaise -= OnCloseShop;
    // }

    private void Reset()
    {
        _particles = GetComponent<ParticleSystem>();
    }
}
