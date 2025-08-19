using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance;

    [SerializeField] private int _money;

    public int Money
    {
        get => _money;
        set
        {
            _money = value;
            PlayerPrefs.SetInt("Money", _money);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        Instance = this;
        
        _money = PlayerPrefs.GetInt("Money", 0);
    }
}