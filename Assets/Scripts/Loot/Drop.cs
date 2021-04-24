using UnityEngine;

public interface IDropOwner
{
    void SetDrop(Drop drop);
    Drop GetDrop();
}

[CreateAssetMenu]
public class Drop : ScriptableObject
{
    [Header("Drop")]
    public string DropName;

    [TextArea(5,10)]
    public string Description;

    [TextArea(2,5)]
    public string PickupText;

    public DropRarity Rarity;
}

public enum DropRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public static class DropRarityExtensions
{
    public static Color ToColor(this DropRarity rarity)
    {
        switch (rarity)
        {
            case DropRarity.Common:
                return Color.white;

            case DropRarity.Rare:
                return Color.blue;

            case DropRarity.Epic:
                return new Color(1, 0, 1, 1);

            case DropRarity.Legendary:
                return new Color(1, 0.5f, 0, 1);

            default:
                return Color.magenta;
        }
    }
}