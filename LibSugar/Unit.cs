namespace LibSugar;

public readonly record struct Unit
{
    public static readonly Unit Instance = default;

    public override string ToString() => "Unit";
}
