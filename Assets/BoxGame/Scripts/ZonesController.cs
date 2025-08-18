using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZoneUIRule
{
    public string tag;
    public UIKey key;
}

public class ZonesController : MonoBehaviour
{
    [Header("Правила зон -> UI")]
    [SerializeField] private List<ZoneUIRule> zoneRules = new List<ZoneUIRule>();

    private Dictionary<string, UIKey> rulesDictionary;

    private void Awake()
    {
        rulesDictionary = new Dictionary<string, UIKey>();
        foreach (var rule in zoneRules)
        {
            if (!rulesDictionary.ContainsKey(rule.tag))
            {
                rulesDictionary.Add(rule.tag, rule.key);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rulesDictionary.TryGetValue(other.tag, out UIKey key))
        {
            UIController.Instance.Show(key);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (rulesDictionary.TryGetValue(other.tag, out UIKey key))
        {
            UIController.Instance.Hide(key);
        }
    }
}