using System;
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
    BuyZone2,
    BuyZone3,
    BuyZone4,
    BuyZone5,
    BuyZone6,
    BuyZone7,
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

    [SerializeField] private TextMeshProUGUI _moneyText1;
    [SerializeField] private TextMeshProUGUI _moneyText2;
    [SerializeField] private TextMeshProUGUI _moneyText3;
    
    [SerializeField] private string _moneyPerSecondText = "$/second";

    [SerializeField] private AudioClip _takeMoneySound;
    [SerializeField] private AudioClip _upgradeSound;
    [SerializeField] private AudioClip _haveNoMoneySound;
    
    [SerializeField] private GameObject _gymPanel;
    public GameObject _buyZonePanel;
    [SerializeField] private GameObject _haveNoMoneyPanel;
    [SerializeField] private GameObject _haveNoMoneyPanel2;
    [SerializeField] private TextMeshProUGUI _boxPanelName;
    [SerializeField] private Image _boxPanelIcon;
    [SerializeField] private TextMeshProUGUI _boxPanelMoneyPerSecond;
    [SerializeField] private TextMeshProUGUI _boxPanelTakeMoneyButtonText;
    [SerializeField] private TextMeshProUGUI _boxPanelUpgradeButtonText;

    [Header("UI Elements")]
    [SerializeField] private List<UIElement> uiElements = new List<UIElement>();

    private int _currentIndex;
    
    private Dictionary<UIKey, Transform> uiDictionary;

    private Gym _currentGym;

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

    private void Update()
    {
        _moneyText1.text = WalletController.Instance.Money.ToString();
        _moneyText2.text = WalletController.Instance.Money.ToString();
        _moneyText3.text = WalletController.Instance.Money.ToString();
    }

    public void OpenGymWindow(int index)
    {
        Gym gym = GymController.Instance.gyms[index];

        _currentGym = gym;
        
        _boxPanelIcon.sprite = gym.icon;
        _boxPanelName.text = gym.name;
        _boxPanelMoneyPerSecond.text = $"{gym.level}{_moneyPerSecondText}";
        gym.UpdateUpgradeCost();
        _boxPanelUpgradeButtonText.text = $"UPGRADE ({gym.upgradeCost})";
        _boxPanelTakeMoneyButtonText.text = $"TAKE ({gym.income})";

        _gymPanel.SetActive(true);
    }

    public void TakeMoney()
    {
        WalletController.Instance.Money += _currentGym.income;
        _currentGym.income = 0;
        _boxPanelTakeMoneyButtonText.text = $"TAKE ({_currentGym.income})";
        MusicController.Instance.PlaySpecificSound(_takeMoneySound);
    }

    public void UpgradeGym()
    {
        if (WalletController.Instance.Money >= _currentGym.upgradeCost)
        {
            WalletController.Instance.Money -= _currentGym.upgradeCost;
            _currentGym.LevelUp();
            _boxPanelUpgradeButtonText.text = $"UPGRADE ({_currentGym.upgradeCost})";
            _boxPanelMoneyPerSecond.text = $"{_currentGym.level}{_moneyPerSecondText}";
            MusicController.Instance.PlaySpecificSound(_upgradeSound);
        }
        else
        {
            _haveNoMoneyPanel.SetActive(true);
            MusicController.Instance.PlaySpecificSound(_haveNoMoneySound);
        }
    }

    public void BuyMaterials()
    {
        if (WalletController.Instance.Money >= 100)
        {
            WalletController.Instance.Money -= 100;
            WalletController.Instance.Materials += 50;
            MusicController.Instance.PlaySpecificSound(_upgradeSound);
        }
        else
        {
            _haveNoMoneyPanel2.SetActive(true);
            MusicController.Instance.PlaySpecificSound(_haveNoMoneySound);
        }
    }

    public void OpenBuyZonePanel(int index)
    {
        _currentIndex = index;
        _buyZonePanel.gameObject.SetActive(true);
    }

    public void BuyZone()
    {
        GymController.Instance.BuyZone(_currentIndex);
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