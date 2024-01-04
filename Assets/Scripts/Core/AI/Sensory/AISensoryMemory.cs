using System.Collections.Generic;
using UnityEngine;

public class AIMemory
{
    public GameObject GameObject { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Direction { get; set; }
    public float Distance { get; set; }
    public float Angle { get; set; }
    public float LastSeen { get; set; }
    public float Score { get; set; }

    public float Age => Time.time - LastSeen;
}

public class AISensoryMemory
{
    private List<AIMemory> aiMemoryList = new List<AIMemory>();
    private GameObject[] pawns;
    private LayerMask targetLayer;

    public List<AIMemory> Memories => new List<AIMemory>(aiMemoryList);

    public AISensoryMemory(int maxPlayers)
    {
        pawns = new GameObject[maxPlayers];
    }

    public void UpdateSensory(AISightSensor sensor)
    {
        int targets = sensor.Filter(pawns, "Pawn");

        for (int i = 0; i < targets; i++)
        {
            GameObject target = pawns[i];
            RefreshMemory(sensor.gameObject, target);
        }
    }

    public void RefreshMemory(GameObject agent, GameObject target)
    {
        AIMemory memory = FetchMemory(target);
        memory.GameObject = target;
        memory.Position = target.transform.position;
        memory.Direction = target.transform.position - agent.transform.position;
        memory.Distance = memory.Direction.magnitude;
        memory.Angle = Vector3.Angle(agent.transform.forward, memory.Direction);
        memory.LastSeen = Time.time;
    }

    private AIMemory FetchMemory(GameObject gameObject)
    {
        AIMemory memory = aiMemoryList.Find(m => m.GameObject == gameObject);

        if (memory == null)
        {
            memory = new AIMemory();
            aiMemoryList.Add(memory);
        }

        return memory;
    }

    public void ForgetMemories(float olderThan)
    {
        aiMemoryList.RemoveAll(m => m.Age > olderThan || m.GameObject == null);
    }
}