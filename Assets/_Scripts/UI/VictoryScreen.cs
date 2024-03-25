using UnityEngine;

[RequireComponent(typeof(PauseWhileActive))]
public class VictoryScreen : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.Victory.OnSetIsActive += OnActivateVictory;
        OnActivateVictory(GameManager.Instance.Victory.IsActive);
    }
    
    private void OnActivateVictory(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.Victory.OnSetIsActive -= OnActivateVictory;
    }
}
