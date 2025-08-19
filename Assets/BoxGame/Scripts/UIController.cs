using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UIKey
{
    Shop,
    BuyZone1,
    TakeMoney1,
}

[System.Serializable]
public class UIElement
{
    public UIKey key;
    public Transform target;
}

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private string _moneyPerSecondText = "$/second";
    
    [SerializeField] private TextMeshProUGUI _boxPanelName;
    [SerializeField] private Image _boxPanelIcon;
    [SerializeField] private TextMeshProUGUI _boxPanelMoneyPerSecond;
    [SerializeField] private TextMeshProUGUI _boxPanelTakeMoneyButtonText;
    [SerializeField] private TextMeshProUGUI _boxPanelUpgradeButtonText;

    [Header("UI Elements")]
    [SerializeField] private List<UIElement> uiElements = new List<UIElement>();

    private Dictionary<UIKey, Transform> uiDictionary;

    private void Awake()
    {
        Instance = this;

        uiDictionary = new Dictionary<UIKey, Transform>();
        foreach (var element in uiElements)
        {
            if (element.target != null && !uiDictionary.ContainsKey(element.key))
            {
                uiDictionary.Add(element.key, element.target);
                element.target.localScale = Vector3.zero;
            }
        }
    }

    public void OpenGymWindow(int index)
    {
        Gym gym = GymController.Instance.gyms[index];

        _boxPanelIcon.sprite = gym.icon;
        _boxPanelName.text = gym.name;
        _boxPanelMoneyPerSecond.text = $"{gym.level}{_moneyPerSecondText}";
        gym.UpdateUpgradeCost();
        _boxPanelUpgradeButtonText.text = $"UPGRADE ({gym.upgradeCost})";
        _boxPanelTakeMoneyButtonText.text = $"TAKE ({gym.income})";
    }
    
    public void Show(UIKey key, float duration = 0.3f)
    {
        if (uiDictionary.TryGetValue(key, out Transform target))
        {
            target.gameObject.SetActive(true);
            target.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
        }
        else
        {
            Debug.LogWarning($"UIController: Элемент {key} не найден!");
        }
    }

    public void Hide(UIKey key, float duration = 0.2f)
    {
        if (uiDictionary.TryGetValue(key, out Transform target))
        {
            target.DOScale(Vector3.zero, duration).SetEase(Ease.InBack)
                .OnComplete(() => target.gameObject.SetActive(false));
        }
        else
        {
            Debug.LogWarning($"UIController: Элемент {key} не найден!");
        }
    }
}