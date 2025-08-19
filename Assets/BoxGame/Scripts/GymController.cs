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
    public bool isAvialiable;

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

    public List<Gym> gyms;

    private float timer;

    private void Awake()
    {
        Instance = this;

        LoadGyms(); 
        
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
