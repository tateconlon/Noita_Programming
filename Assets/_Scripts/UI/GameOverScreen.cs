using UnityEngine;

[RequireComponent(typeof(PauseWhileActive))]
public class GameOverScreen : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.GameOver.OnSetIsActive += OnActivateGameOver;
        OnActivateGameOver(GameManager.Instance.GameOver.IsActive);
    }
    
    private void OnActivateGameOver(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.GameOver.OnSetIsActive -= OnActivateGameOver;
    }
}
