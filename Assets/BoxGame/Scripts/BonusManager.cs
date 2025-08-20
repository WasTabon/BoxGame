using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance;

    [Header("UI Elements")]
    [SerializeField] private Transform rewardsParent; // Родитель с наградами
    [SerializeField] private GameObject rewardPrefab; // Префаб награды (текст + кнопка)

    private int[] rewardThresholds;   // Сколько денег нужно для награды
    private bool[] rewardClaimed;     // Получена ли награда
    private int totalMoneyEarned;     // Сколько всего игрок заработал денег (накопительно)

    private bool autoMaterialsStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        WalletController.Instance.OnMoneyChanged += HandleMoneyEarned;
        WalletController.Instance.OnMaterialsChanged += HandleMaterialsChanged;

        // 10 наград: 500, 1000, 1500, ..., 5000
        rewardThresholds = new int[10];
        for (int i = 0; i < rewardThresholds.Length; i++)
            rewardThresholds[i] = 500 * (i + 1);

        rewardClaimed = new bool[rewardThresholds.Length];

        // Загружаем прогресс
        totalMoneyEarned = PlayerPrefs.GetInt("TotalMoneyEarned", 0);
        for (int i = 0; i < rewardClaimed.Length; i++)
            rewardClaimed[i] = PlayerPrefs.GetInt($"RewardClaimed{i}", 0) == 1;

        autoMaterialsStarted = PlayerPrefs.GetInt("AutoMaterials", 0) == 1;
        if (autoMaterialsStarted)
            StartCoroutine(AutoMaterialsCoroutine());

        CreateUI();
    }

    private void CreateUI()
    {
        // Очищаем старое
        foreach (Transform child in rewardsParent)
            Destroy(child.gameObject);

        // Создаём элементы UI для каждой награды
        for (int i = 0; i < rewardThresholds.Length; i++)
        {
            GameObject obj = Instantiate(rewardPrefab, rewardsParent);
            TextMeshProUGUI text = obj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
            Button button = obj.transform.Find("ClaimButton").GetComponent<Button>();

            int index = i;
            button.onClick.AddListener(() => ClaimReward(index));

            UpdateRewardUI(index, text, button);
        }
    }

    private void UpdateRewardUI(int index, TextMeshProUGUI text, Button button)
    {
        if (text == null)
        {
            Debug.LogError($"[BonusManager] ProgressText is NULL for reward {index}. Проверь, есть ли объект 'ProgressText' в префабе rewardPrefab.");
            return;
        }

        if (button == null)
        {
            Debug.LogError($"[BonusManager] ClaimButton is NULL for reward {index}. Проверь, есть ли объект 'ClaimButton' в префабе rewardPrefab.");
            return;
        }

        TextMeshProUGUI buttonLabel = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonLabel == null)
        {
            Debug.LogError($"[BonusManager] ClaimButton {index} не имеет дочернего Text. Возможно, у тебя используется TextMeshPro (TMP_Text).");
            return;
        }
        
        int need = rewardThresholds[index];
        text.text = $"{totalMoneyEarned}/{need}";

        if (rewardClaimed[index])
        {
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Received";
        }
        else
        {
            button.interactable = totalMoneyEarned >= need;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Take";
        }
    }
    
    private void HandleMoneyEarned(int delta)
    {
        if (delta > 0) // важно: считаем только полученные, не траты
        {
            totalMoneyEarned += delta;
            PlayerPrefs.SetInt("TotalMoneyEarned", totalMoneyEarned);
            PlayerPrefs.Save();
            RefreshAllUI();
        }
    }

    private void HandleMaterialsChanged(int newValue)
    {
        if (!autoMaterialsStarted && newValue >= 1000)
        {
            autoMaterialsStarted = true;
            PlayerPrefs.SetInt("AutoMaterials", 1);
            PlayerPrefs.Save();

            StartCoroutine(AutoMaterialsCoroutine());
        }
    }

    private void ClaimReward(int index)
    {
        if (rewardClaimed[index]) return;
        if (totalMoneyEarned < rewardThresholds[index]) return;

        rewardClaimed[index] = true;
        WalletController.Instance.Materials += 250;

        PlayerPrefs.SetInt($"RewardClaimed{index}", 1);
        PlayerPrefs.Save();

        RefreshAllUI();

        // Проверяем авто-материалы
        if (!autoMaterialsStarted && WalletController.Instance.Materials >= 1000)
        {
            autoMaterialsStarted = true;
            PlayerPrefs.SetInt("AutoMaterials", 1);
            PlayerPrefs.Save();

            StartCoroutine(AutoMaterialsCoroutine());
        }
    }

    private void RefreshAllUI()
    {
        for (int i = 0; i < rewardsParent.childCount; i++)
        {
            TextMeshProUGUI text = rewardsParent.GetChild(i).Find("ProgressText").GetComponent<TextMeshProUGUI>();
            Button button = rewardsParent.GetChild(i).Find("ClaimButton").GetComponent<Button>();
            UpdateRewardUI(i, text, button);
        }
    }

    private IEnumerator AutoMaterialsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            WalletController.Instance.Materials += 1;
        }
    }
}
