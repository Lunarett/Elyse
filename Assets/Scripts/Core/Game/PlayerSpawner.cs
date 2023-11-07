using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

    private List<Transform> spawnpoints = new List<Transform>();
    private Dictionary<Player, Transform> playerSpawnpoints = new Dictionary<Player, Transform>();

    void Awake()
    {
        Instance = this;
        // Find all spawn points in the scene by tag
        GameObject[] spawnpointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (var spawnpointObject in spawnpointObjects)
        {
            spawnpoints.Add(spawnpointObject.transform);
        }
    }

    public Transform GetSpawnpoint(Player player)
    {
        // Check if this player already has an assigned spawn point
        if (playerSpawnpoints.TryGetValue(player, out Transform spawnpoint))
        {
            return spawnpoint;
        }
        
        // If not, assign a new spawn point to the player
        var availableSpawnpoints = new List<Transform>(spawnpoints);
        foreach (var assignedSpawnpoint in playerSpawnpoints.Values)
        {
            availableSpawnpoints.Remove(assignedSpawnpoint);
        }
        
        if (availableSpawnpoints.Count > 0)
        {
            spawnpoint = availableSpawnpoints[Random.Range(0, availableSpawnpoints.Count)];
            playerSpawnpoints[player] = spawnpoint;
            return spawnpoint;
        }
        
        Debug.LogWarning("No available spawn points.");
        return null;
    }

    // Call this method if a player leaves and you want to free up their spawn point
    public void RemovePlayerSpawnpoint(Player player)
    {
        playerSpawnpoints.Remove(player);
    }
}