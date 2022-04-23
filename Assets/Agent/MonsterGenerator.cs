using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate the monsters
/// </summary>
public class MonsterGenerator : MonoBehaviour
{

    public GameObject monsterPrefab;
    
    public List<GameObject> monstersList = new List<GameObject>();

    public void GenerateMonsters(List<DungeonNode> nodes){
        for (int i = 1; i < nodes.Count; i++)
        {
            Vector3 centerPos = nodes[i].center;
            SpawnMonster(centerPos);
        }
    }

    private void SpawnMonster(Vector3 pos)
    {
        GameObject mon = Instantiate(monsterPrefab, pos, Quaternion.identity);
        monstersList.Add(mon);
    }
}
