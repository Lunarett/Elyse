using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

    private List<Transform> _spawnpoints = new List<Transform>();

    void Awake()
    {
        Instance = this;

        GameObject[] spawnpointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (var spawnpointObject in spawnpointObjects)
        {
            _spawnpoints.Add(spawnpointObject.transform);
        }
    }

    public Transform GetSpawnpoint()
    {
        int randomIndex = Random.Range(0, _spawnpoints.Count);
        return _spawnpoints[randomIndex];
    }
}