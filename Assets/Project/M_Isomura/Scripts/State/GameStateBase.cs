public abstract class GameStateBase
{
    //ステータス開始時
    public virtual void OnEnter(GameManager owner, GameStateBase playerState) { }

    //ステータス中の判定
    public virtual void OnUpDate(GameManager owner) { }

    //ステータス終了判定
    public virtual void OnExit(GameManager owner, GameStateBase nextState) { }
}
