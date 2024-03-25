using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnMarker : MonoBehaviour, IPoolable
{
    [SerializeField] SpriteRenderer spriteRenderer;
    
    Tweener _springTween;
    Sequence _blinkSequence;

    float _blinkSpeedMultiplier;
    
    public void OnSpawn()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * Random.Range(0.0f, 2.0f * Mathf.PI));
        
        _blinkSpeedMultiplier = 1.0f;
        
        // pop3:play{pitch = 1, volume = 0.15}  // TODO: SFX

        // TODO: replace with a proper conversion of SNKRX spring functionality (e.g. with inputs of f, k, d)
        // https://github.com/a327ex/blog/issues/60
        _springTween = transform.DOPunchScale(Vector3.one, 1.0f, 10, 1.0f);

        _blinkSequence = DOTween.Sequence()
            .AppendInterval(Random.Range(0.195f, 0.24f))
            .AppendCallback(() =>
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                _blinkSpeedMultiplier *= Random.Range(0.84f, 0.87f);
            })
            .SetLoops(-1);
    }

    void Update()
    {
        _blinkSequence.SetEveryMultiplier(_blinkSpeedMultiplier);
    }

    public void OnDespawn()
    {
        _springTween?.Kill();
        _blinkSequence?.Kill();
    }
}
