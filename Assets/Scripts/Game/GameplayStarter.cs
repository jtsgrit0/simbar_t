using UnityEngine;

public class GameplayStarter : MonoBehaviour
{
    [SerializeField] private bool autoStartGame = true;

    private void Start()
    {
        if (!autoStartGame)
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isOpen = true;
        }

        if (FindObjectOfType<BarUIBuilder>() == null)
        {
            var uiBuilder = new GameObject("BarUIBuilder");
            uiBuilder.AddComponent<BarUIBuilder>();
        }
    }
}
