using UnityEngine;
using TMPro;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private int storedCoins = 0;
    [SerializeField] private int coinsPerTick = 1;
    [SerializeField] private float tickInterval = 3f;
    [SerializeField] private int maxStoredCoins = 50;

    [Header("Show Text")]
    [SerializeField] private Transform player;
    [SerializeField] private float showDistance = 5f;
    [SerializeField] private TextMeshPro worldText;

    private float timer;

    public int StoredCoins => storedCoins;

    void Start()
    {
        if (worldText != null)
        {
            worldText.gameObject.SetActive(false);
            UpdateText();
        }
    }

    void Update()
    {
        HandleCoinGeneration();
        HandleTextVisibility();
    }

    void HandleCoinGeneration()
    {
        timer += Time.deltaTime;

        if (timer < tickInterval) return;

        timer -= tickInterval;

        storedCoins += coinsPerTick;
        if (storedCoins > maxStoredCoins)
            storedCoins = maxStoredCoins;

        UpdateText();
    }

    void HandleTextVisibility()
    {
        if (player == null || worldText == null)
            return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool shouldShow = dist <= showDistance;

        if (worldText.gameObject.activeSelf != shouldShow)
            worldText.gameObject.SetActive(shouldShow);

        if (shouldShow)
        {
            if (player != null)
            {
                Vector3 dir = player.transform.position - worldText.transform.position;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.0001f)
                    worldText.transform.rotation = Quaternion.LookRotation(-dir);
            }

            UpdateText();
        }
    }

    void UpdateText()
    {
        if (worldText == null) return;
        worldText.text = $"Coin : {storedCoins}";
    }

    public int CollectAllCoins()
    {
        int amount = storedCoins;
        storedCoins = 0;
        UpdateText();
        return amount;
    }
}