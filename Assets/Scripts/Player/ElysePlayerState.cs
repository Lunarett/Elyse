using UnityEngine;

public class ElysePlayerState : PlayerState
{
    private int _numberOfKills = 0;
    private int _numberOfDeaths = 0;
    
    public int Kills => _numberOfKills;
    public int Deaths => _numberOfDeaths;

    public void AddDeath()
    {
        _numberOfDeaths++;
    }
    
    public void AddKill()
    {
        _numberOfKills++;
    }

    public void ResetStats()
    {
        _numberOfKills = 0;
        _numberOfDeaths = 0;
    }
}
