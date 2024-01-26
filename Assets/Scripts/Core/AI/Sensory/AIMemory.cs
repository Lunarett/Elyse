using System.Collections.Generic;
using UnityEngine;

public class AIMemory : MonoBehaviour
{
    [SerializeField] private int memoryCapacity = 5; // Maximum number of targets to remember
    private Queue<MemoryItem> memoryQueue = new Queue<MemoryItem>();

    public struct MemoryItem
    {
        public GameObject target;
        public float lastSeenTime; // Timestamp when the target was last seen
        public Vector3 lastKnownPosition; // Last known position of the target

        public MemoryItem(GameObject target, float lastSeenTime, Vector3 lastKnownPosition)
        {
            this.target = target;
            this.lastSeenTime = lastSeenTime;
            this.lastKnownPosition = lastKnownPosition;
        }
    }

    // Call this method to add or update a target in memory
    public void UpdateMemory(GameObject target)
    {
        Vector3 position = target != null ? target.transform.position : Vector3.zero;
        MemoryItem newItem = new MemoryItem(target, Time.time, position);

        // Check if this target is already in memory
        bool found = false;
        MemoryItem[] items = memoryQueue.ToArray();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].target == target)
            {
                memoryQueue = new Queue<MemoryItem>(items);
                memoryQueue.Enqueue(newItem);
                found = true;
                break;
            }
        }

        if (!found)
        {
            // If memory queue is full, dequeue the oldest item
            if (memoryQueue.Count >= memoryCapacity)
            {
                memoryQueue.Dequeue();
            }
            memoryQueue.Enqueue(newItem);
        }
    }

    // Retrieve the most recently seen target that's still in memory
    public GameObject GetMostRecentTarget()
    {
        MemoryItem? mostRecent = null;
        foreach (var item in memoryQueue)
        {
            if (mostRecent == null || item.lastSeenTime > mostRecent.Value.lastSeenTime)
            {
                mostRecent = item;
            }
        }

        return mostRecent?.target;
    }

    // Retrieve the last known position of the most recently seen target
    public Vector3? GetLastKnownPosition(GameObject target)
    {
        foreach (var item in memoryQueue)
        {
            if (item.target == target)
            {
                return item.lastKnownPosition;
            }
        }

        return null;
    }
}
