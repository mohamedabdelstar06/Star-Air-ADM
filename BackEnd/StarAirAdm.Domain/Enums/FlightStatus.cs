namespace StarAirAdm.Domain.Enums;

public enum FlightStatus
{
    Pending = 0,    // Assigned to pilot, assessments and prep not completed
    Cleared = 1,    // All 3 assessments (IMSAFE, PAVE, DECIDE) done
    Completed = 2,  // Flight successfully logged/finished
    Cancelled = 3
}
