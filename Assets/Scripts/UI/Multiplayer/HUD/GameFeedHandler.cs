using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFeedHandler : MonoBehaviour
{
    [SerializeField] private GameFeedElement _gameFeedElementPrefab;
    [SerializeField] private Transform _feedContainer;

    private Queue<GameFeedElement> _messageQueue = new Queue<GameFeedElement>();

    public void AddMessage(string causerName, string affectedName, int maxFeedCount, float feedLifetime)
    {
        if (_messageQueue.Count >= maxFeedCount)
        {
            DestroyOldestMessage();
        }

        var newMessage = Instantiate(_gameFeedElementPrefab, _feedContainer);
        newMessage.SetCauserName(causerName);
        newMessage.SetAffectedName(affectedName);

        _messageQueue.Enqueue(newMessage);

        StartCoroutine(RemoveMessageAfterTime(newMessage, feedLifetime));
    }

    public void ClearAllMessages()
    {
        Debug.Log(_messageQueue.Count);
        while (_messageQueue.Count > 0)
        {
            var message = _messageQueue.Dequeue();
            Destroy(message.gameObject);
        }
    }

    private IEnumerator RemoveMessageAfterTime(GameFeedElement message, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!_messageQueue.Contains(message)) yield break;
        _messageQueue.Dequeue();
        Destroy(message.gameObject);
    }

    private void DestroyOldestMessage()
    {
        var oldestMessage = _messageQueue.Dequeue();
        Destroy(oldestMessage.gameObject);
    }
}