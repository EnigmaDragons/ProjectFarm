using System;

public class Permissible
{
    public string[] NotPermissibleReasons { get; }

    public static Permissible Allowed() => new Permissible(Array.Empty<string>());

    public Permissible(params string[] reasons) => NotPermissibleReasons = reasons;
    
    public static implicit operator bool(Permissible p) => p.NotPermissibleReasons.Length == 0;
}
