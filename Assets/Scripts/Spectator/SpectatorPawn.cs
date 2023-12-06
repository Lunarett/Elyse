using UnityEngine;

public class SpectatorPawn : Pawn
{
    private void Start()
    {
        ShowMouseCursor(false);
        HUD.Instance.ShowEmpty();
    }
}
