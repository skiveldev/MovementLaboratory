using UnityEngine;

public sealed class LevelProgress : MonoBehaviour
{
    public static LevelProgress Instance { get; private set; }

    private int collectedCoins;
    private int totalCoins;
    private bool completed;

    private void Awake()
    {
        Instance = this;
        totalCoins = FindObjectsByType<CoinCollectible>().Length;
    }

    public void CollectCoin()
    {
        collectedCoins++;
    }

    public void CompleteLevel()
    {
        completed = true;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(16f, 16f, 180f, 28f), $"Coins: {collectedCoins}/{totalCoins}");

        if (completed)
        {
            GUI.Label(new Rect(Screen.width * 0.5f - 70f, 16f, 160f, 28f), "Level Complete!");
        }
    }
}
