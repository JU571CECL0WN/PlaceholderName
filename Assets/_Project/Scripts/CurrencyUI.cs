using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    private PlayerState localPlayer;

    private void Start()
    {
        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        while (localPlayer == null)
        {
            foreach (var player in FindObjectsByType<PlayerState>(FindObjectsSortMode.None))
            {
                if (player.IsOwner)
                {
                    localPlayer = player;
                    localPlayer.money.OnValueChanged += OnMoneyChanged;
                    UpdateMoney(localPlayer.money.Value);
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void OnMoneyChanged(int oldValue, int newValue)
    {
        UpdateMoney(newValue);
    }

    private void UpdateMoney(int value)
    {
        moneyText.text = $"Money: {value}";
    }

    private void OnDestroy()
    {
        if (localPlayer != null)
            localPlayer.money.OnValueChanged -= OnMoneyChanged;
    }
}