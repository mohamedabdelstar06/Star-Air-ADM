namespace StarAirAdm.Application.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalPilots { get; set; }
    public int ActivePilots { get; set; }
    public int PendingPilots { get; set; }
    public int TotalAircraft { get; set; }
    public int AirworthyAircraft { get; set; }
    public int TotalImSafeAssessments { get; set; }
    public int TotalPaveAssessments { get; set; }
    public int GoCount { get; set; }
    public int CautionCount { get; set; }
    public int NoGoCount { get; set; }
    public List<RecentAssessmentDto> RecentAssessments { get; set; } = new();
}

public class RecentAssessmentDto
{
    public string Type { get; set; } = string.Empty; // IMSAFE, PAVE
    public string PilotName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public DateTime AssessedAt { get; set; }
}
