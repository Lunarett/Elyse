using System.Collections.Generic;

public class TeamManager
{
    private static TeamManager _instance;
    public static TeamManager Instance => _instance ?? (_instance = new TeamManager());

    private Dictionary<int, List<ITeamMember>> teams = new Dictionary<int, List<ITeamMember>>();

    private TeamManager() {}

    public void RegisterTeamMember(ITeamMember member)
    {
        if (!teams.ContainsKey(member.TeamId))
        {
            teams[member.TeamId] = new List<ITeamMember>();
        }
        teams[member.TeamId].Add(member);
    }

    public void UnregisterTeamMember(ITeamMember member)
    {
        if (teams.ContainsKey(member.TeamId))
        {
            teams[member.TeamId].Remove(member);
        }
    }

    public bool AreAllies(ITeamMember member1, ITeamMember member2)
    {
        return member1.TeamId == member2.TeamId;
    }
}