using TMPro;
using UnityEngine;

public class PlayerScoreElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _deathsText;
    
    public void SetPlayerName(string name)
    {
        _playerNameText.text = name;
    }

    public void SetKills(int kills)
    {
        _killsText.text = kills.ToString();
    }
    
    public void SetDeaths(int deaths)
    {
        _deathsText.text = deaths.ToString();
    }
}
