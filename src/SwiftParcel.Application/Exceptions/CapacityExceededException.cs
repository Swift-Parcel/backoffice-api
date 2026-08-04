using System;

public class CapacityExceededException : Exception
{
    public int HandlerId { get; }
    public int MaxCases { get; }

    public CapacityExceededException(int handlerId, int maxCases) 
        : base($"Handler {handlerId} has reached their maximum capacity of {maxCases} active cases.")
    {
        HandlerId = handlerId;
        MaxCases = maxCases;
    }
}