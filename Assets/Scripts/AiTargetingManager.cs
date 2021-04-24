using System.Collections.Generic;
using UnityEngine;

public class AiTargetingManager : MonoBehaviour
{
    [SerializeField] private Transform aiTarget;

    List<IEnemy> enemies = new List<IEnemy>();

    public void RegisterEnemy(IEnemy enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(IEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public List<IEnemy> GetAllEnemies()
    {
        return enemies;
    }

    public Transform GetDefaultAITarget()
    {
        return aiTarget;
    }
}
