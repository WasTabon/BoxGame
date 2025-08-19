using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance;

    [SerializeField] private int _money;
    [SerializeField] private int _materials;

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
    public int Materials
    {
        get => _materials;
        set
        {
            _materials = value;
            PlayerPrefs.SetInt("Materials", _materials);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        Instance = this;
        
        _money = PlayerPrefs.GetInt("Money", 0);
        _materials = PlayerPrefs.GetInt("Materials", 0);
    }
}