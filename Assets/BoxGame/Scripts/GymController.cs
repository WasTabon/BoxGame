using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Gym
{
    public string name;
    public int level;
    public Sprite icon;
    public int income;
    public int upgradeCost;
    public bool isAvialiable = false;

    private const int baseCost = 500;
    private const float costMultiplier = 1.75f;

    public void UpdateUpgradeCost()
    {
        upgradeCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level - 1));
    }

    public void LevelUp()
    {
        level++;
        UpdateUpgradeCost();
    }
}

public class GymController : MonoBehaviour
{
    public static GymController Instance;

    [SerializeField] private GameObject _haveNoMoneyPanel;
    
    [SerializeField] private AudioClip _upgradeSound;
    [SerializeField] private AudioClip _haveNoMoneySound;
    
    [SerializeField] private List<GameObject> zones;
    [SerializeField] private List<GameObject> _signs;
    [SerializeField] private List<GameObject> _colliders;

    public List<Gym> gyms;

    private float timer;

    private void Awake()
    {
        Instance = this;

        LoadGyms();

        // Включаем доступные зоны
        for (int i = 0; i < gyms.Count && i < zones.Count; i++)
        {
            if (i != 0)
                zones[i].SetActive(gyms[i].isAvialiable);
        }

        foreach (var gym in gyms)
        {
            if (gym.isAvialiable)
                gym.UpdateUpgradeCost();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            foreach (var gym in gyms)
            {
                if (gym.isAvialiable)
                    gym.income += gym.level;
            }

            timer = 0f;
            SaveGyms();
        }
    }

    [Serializable]
    private class GymListWrapper
    {
        public List<Gym> gyms;
    }

    public void BuyZone(int index)
    {
        if (index < 0 || index >= gyms.Count || index >= zones.Count)
            return;

        if (WalletController.Instance.Materials >= 350)
        {
            WalletController.Instance.Materials -= 350;
            gyms[index + 1].isAvialiable = true;
            zones[index].SetActive(true);

            MusicController.Instance.PlaySpecificSound(_upgradeSound);
            
            UIController.Instance._buyZonePanel.gameObject.SetActive(false);
            
            SaveGyms();
        }
        else
        {
            _haveNoMoneyPanel.SetActive(true);
            MusicController.Instance.PlaySpecificSound(_haveNoMoneySound);
        }
    }

    public void SaveGyms()
    {
        GymListWrapper wrapper = new GymListWrapper { gyms = gyms };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("GymsData", json);
        PlayerPrefs.Save();
    }

    public void LoadGyms()
    {
        if (PlayerPrefs.HasKey("GymsData"))
        {
            string json = PlayerPrefs.GetString("GymsData");
            GymListWrapper wrapper = JsonUtility.FromJson<GymListWrapper>(json);
            gyms = wrapper.gyms;
        }
    }
}
