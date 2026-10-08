using System.Collections.Generic;
using UnityEngine;

// declares the rules to be used in each level
public class RulesTracker : MonoBehaviour
{
    
    public static RulesTracker Instance { get; private set; }
    public Dictionary<Rule, bool> activeRulesInLevel = new Dictionary<Rule, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);   // make only 1 instance
    }

    public void SetRules(Dictionary<Rule, bool> levelRules)
    {
        activeRulesInLevel.Clear();     // remove previous level's rules
        foreach (var keyVal in levelRules)
        {
            activeRulesInLevel.Add(keyVal.Key, keyVal.Value);
        }
    }
}
