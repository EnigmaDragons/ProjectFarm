
public class LogScopeHandler : OnMessage<DisableLogScope, EnableLogScope, ToggleLogScope>
{
    protected override void Execute(DisableLogScope msg) => Log.DisableScope(msg.Name);
    protected override void Execute(EnableLogScope msg) => Log.EnableScope(msg.Name);
    
    protected override void Execute(ToggleLogScope msg) {
        if (Log.ScopeEnabled(msg.Name))
            Log.DisableScope(msg.Name);
        else
            Log.EnableScope(msg.Name);
    }
}
