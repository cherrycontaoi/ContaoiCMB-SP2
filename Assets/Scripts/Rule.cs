using UnityEngine;

[CreateAssetMenu(fileName = "New Rule", menuName = "Rule")]
public class Rule : ScriptableObject
{
    public Item item1;
    public Item item2;
    public RuleType ruleType;
}

public enum RuleType
{
    CannotBeAdjacent,
    MustBeAdjacent,
    CannotBeAtEdge,
    PlacedLast,
    PlacedFirst
}
