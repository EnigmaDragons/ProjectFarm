using UnityEngine;

public class OnHeroPathUpdateGameState : OnMessage<HeroPathBegun>
{
    [SerializeField] private GameState gameState;

    protected override void Execute(HeroPathBegun msg) => gameState.SetIsGenius();
}
