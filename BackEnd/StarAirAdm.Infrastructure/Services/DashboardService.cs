using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Dashboard;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Domain.Enums;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardService(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var allPilots = new List<ApplicationUser>();
        foreach (var u in allUsers)
        {
            var r = await _userManager.GetRolesAsync(u);
            if (!r.Contains("Admin")) allPilots.Add(u);
        }

        var totalPilots = allPilots.Count;
        var activePilots = allPilots.Count(p => p.Status == UserStatus.Active);
        var pendingPilots = allPilots.Count(p => p.Status == UserStatus.Pending);

        var totalAircraft = 0;
        var airworthyAircraft = 0;

        var totalImsafe = await _db.ImSafeAssessments.CountAsync();
        var totalPave = await _db.PaveAssessments.CountAsync();

        var imsafeGo = await _db.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.Go);
        var imSafeCaution = await _db.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.Caution);
        var imsafeNogo = await _db.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.NoGo);

        var paveGo = await _db.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.Go);
        var paveCaution = await _db.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.Caution);
        var paveNogo = await _db.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.NoGo);

        // Recent 10 assessments across both types
        var recentImsafe = await _db.ImSafeAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .Take(5).AsNoTracking().ToListAsync();

        var recentPave = await _db.PaveAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .Take(5).AsNoTracking().ToListAsync();

        var recentAssessments = recentImsafe
            .Select(a => new RecentAssessmentDto
            {
                Type = "IMSAFE", PilotName = a.Pilot?.FullName ?? "Unknown",
                Result = a.Result.ToString(), RiskScore = a.OverallRiskScore, AssessedAt = a.AssessedAt
            })
            .Concat(recentPave.Select(a => new RecentAssessmentDto
            {
                Type = "PAVE", PilotName = a.Pilot?.FullName ?? "Unknown",
                Result = a.Result.ToString(), RiskScore = a.OverallRiskScore, AssessedAt = a.AssessedAt
            }))
            .OrderByDescending(a => a.AssessedAt).Take(10).ToList();

        return new DashboardStatsDto
        {
            TotalPilots = totalPilots,
            ActivePilots = activePilots,
            PendingPilots = pendingPilots,
            TotalAircraft = totalAircraft,
            AirworthyAircraft = airworthyAircraft,
            TotalImSafeAssessments = totalImsafe,
            TotalPaveAssessments = totalPave,
            GoCount = imsafeGo + paveGo,
            CautionCount = imSafeCaution + paveCaution,
            NoGoCount = imsafeNogo + paveNogo,
            RecentAssessments = recentAssessments
        };
    }
}
