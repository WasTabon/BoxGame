using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance;

    [Header("UI Elements")]
    [SerializeField] private Transform rewardsParent;   // Родитель с наградами
    [SerializeField] private GameObject rewardPrefab;   // Префаб обычной награды (деньги)
    [SerializeField] private GameObject materialsRewardPrefab; // Префаб награды за 1000 материалов

    private int[] rewardThresholds;   // Сколько денег нужно для награды
    private bool[] rewardClaimed;     // Получена ли награда (по деньгам)
    private int totalMoneyEarned;     // Сколько всего игрок заработал денег (накопительно)

    private bool autoMaterialsStarted;
    private bool materialsRewardClaimed;

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

        materialsRewardClaimed = PlayerPrefs.GetInt("MaterialsRewardClaimed", 0) == 1;

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

        // Создаём элементы UI для наград по деньгам
        for (int i = 0; i < rewardThresholds.Length; i++)
        {
            GameObject obj = Instantiate(rewardPrefab, rewardsParent);
            TextMeshProUGUI text = obj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
            Button button = obj.transform.Find("ClaimButton").GetComponent<Button>();

            int index = i;
            button.onClick.AddListener(() => ClaimReward(index));

            UpdateRewardUI(index, text, button);
        }

        // Создаём отдельный элемент UI для награды за 1000 материалов
        GameObject matObj = Instantiate(materialsRewardPrefab, rewardsParent);
        TextMeshProUGUI matText = matObj.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        Button matButton = matObj.transform.Find("ClaimButton").GetComponent<Button>();
        matButton.onClick.AddListener(ClaimMaterialsReward);

        UpdateMaterialsRewardUI(matText, matButton);
    }

    private void UpdateRewardUI(int index, TextMeshProUGUI text, Button button)
    {
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

    private void UpdateMaterialsRewardUI(TextMeshProUGUI text, Button button)
    {
        text.text = $"{WalletController.Instance.Materials}/1000";

        if (materialsRewardClaimed)
        {
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Received";
        }
        else
        {
            button.interactable = WalletController.Instance.Materials >= 1000;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Take";
        }
    }

    private void HandleMoneyEarned(int delta)
    {
        if (delta > 0)
        {
            totalMoneyEarned += delta;
            PlayerPrefs.SetInt("TotalMoneyEarned", totalMoneyEarned);
            PlayerPrefs.Save();
            RefreshAllUI();
        }
    }

    private void HandleMaterialsChanged(int newValue)
    {
        RefreshMaterialsUI();

        if (!autoMaterialsStarted && newValue >= 1000 && materialsRewardClaimed)
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
    }

    private void ClaimMaterialsReward()
    {
        if (materialsRewardClaimed) return;
        if (WalletController.Instance.Materials < 1000) return;

        materialsRewardClaimed = true;
        PlayerPrefs.SetInt("MaterialsRewardClaimed", 1);
        PlayerPrefs.Save();

        if (!autoMaterialsStarted)
        {
            autoMaterialsStarted = true;
            PlayerPrefs.SetInt("AutoMaterials", 1);
            PlayerPrefs.Save();
            StartCoroutine(AutoMaterialsCoroutine());
        }

        RefreshMaterialsUI();
    }

    private void RefreshAllUI()
    {
        for (int i = 0; i < rewardsParent.childCount; i++)
        {
            Transform child = rewardsParent.GetChild(i);

            if (i < rewardThresholds.Length) // обычные награды
            {
                TextMeshProUGUI text = child.Find("ProgressText").GetComponent<TextMeshProUGUI>();
                Button button = child.Find("ClaimButton").GetComponent<Button>();
                UpdateRewardUI(i, text, button);
            }
        }

        RefreshMaterialsUI();
    }

    private void RefreshMaterialsUI()
    {
        Transform matReward = rewardsParent.GetChild(rewardThresholds.Length); // последний объект — материалы
        TextMeshProUGUI text = matReward.Find("ProgressText").GetComponent<TextMeshProUGUI>();
        Button button = matReward.Find("ClaimButton").GetComponent<Button>();
        UpdateMaterialsRewardUI(text, button);
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
