using System;
using UnityEngine;

public class WalletController : MonoBehaviour
{
    public event Action<int> OnMoneyChanged;     // сообщает новое значение денег
    public event Action<int> OnMaterialsChanged; // сообщает новое значение материалов
    
    public static WalletController Instance;

    [SerializeField] private int _money;
    [SerializeField] private int _materials;

    public int Money
    {
        get => _money;
        set
        {
            int delta = value - _money;
            
            _money = value;
            PlayerPrefs.SetInt("Money", _money);
            PlayerPrefs.Save();
            
            OnMoneyChanged?.Invoke(delta);
        }
    }
    public int Materials
    {
        get => _materials;
        set
        {
            _materials = value;
            PlayerPrefs.SetInt("Materials", _materials);
            PlayerPrefs.Save();
            
            OnMaterialsChanged?.Invoke(_materials);
        }
    }

    private void Awake()
    {
        Instance = this;
        
        _money = PlayerPrefs.GetInt("Money", 0);
        _materials = PlayerPrefs.GetInt("Materials", 0);
    }
}