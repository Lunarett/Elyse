using TMPro;
using UnityEngine;

public class GameFeedElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _causerText;
    [SerializeField] private TMP_Text _affectedText;

    public void SetCauserName(string name)
    {
        _causerText.text = name;
    }

    public void SetAffectedName(string name)
    {
        _affectedText.text = name;
    }
}
