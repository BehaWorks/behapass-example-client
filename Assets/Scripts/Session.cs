using System;

internal sealed class Session
{
    public readonly string Id = Guid.NewGuid().ToString().Substring(0, 16);
    private readonly DateTime _start = DateTime.UtcNow;

    public double GetTimeStamp
    {
        get { return (DateTime.UtcNow - _start).TotalSeconds; }
    }
}