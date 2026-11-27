using UnityEngine;
using UnityEngine.UI;

public class BalanceManager : MonoBehaviour
{
    public static BalanceManager instance { get; private set; }

    public Text MoneyText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RefreshMoney();
    }

    public void RefreshMoney()
    {
        MoneyText.text = SaveManager.LoadGame(GameFlowData.CurrentPlayer).Money.ToString();
    }

    public void ChangeMoney(int amount)
    {
        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        data.Money = Mathf.Max(0, data.Money + amount);
        SaveManager.SaveGame(data);
        MoneyText.text = data.Money.ToString();
    }
}
