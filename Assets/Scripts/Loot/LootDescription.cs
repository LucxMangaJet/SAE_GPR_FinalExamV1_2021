using UnityEngine;

[CreateAssetMenu]
public class LootDescription : ScriptableObject
{
    [SerializeField] private DropProbabilityPair[] drops;

    public void SetDrops(params DropProbabilityPair[] drops)
    {
        this.drops = drops;
    }

    public Drop SelectDropRandomly()
    {
        float rnd = Random.value;
        float probabilitySum = 0;

        for (int i = 0; i < drops.Length; i++)
        {
            DropProbabilityPair pair = drops[i];
            probabilitySum += pair.Probability;

            if (rnd <= probabilitySum)
            {
                return pair.Drop;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct DropProbabilityPair
{
    public Drop Drop;

    [Range(0, 1)]
    public float Probability;
}
