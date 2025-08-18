using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum UIKey
{
    Shop,
    BuyZone1,
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