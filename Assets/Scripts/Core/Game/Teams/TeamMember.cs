using UnityEngine;

public interface ITeamMember
{
    int TeamId { get; }
}

public class TeamMember : MonoBehaviour, ITeamMember
{
    [SerializeField] private int teamId;

    public int TeamId => teamId;

    private void OnEnable()
    {
        TeamManager.Instance.RegisterTeamMember(this);
    }

    private void OnDisable()
    {
        TeamManager.Instance.UnregisterTeamMember(this);
    }
}
