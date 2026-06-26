namespace StarAirAdm.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery() : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var allUsers = await _userManager.Users.ToListAsync(cancellationToken);
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

        var totalImsafe = await _context.ImSafeAssessments.CountAsync(cancellationToken);
        var totalPave = await _context.PaveAssessments.CountAsync(cancellationToken);

        var imsafeGo = await _context.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.Go, cancellationToken);
        var imSafeCaution = await _context.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.Caution, cancellationToken);
        var imsafeNogo = await _context.ImSafeAssessments.CountAsync(a => a.Result == AssessmentResult.NoGo, cancellationToken);

        var paveGo = await _context.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.Go, cancellationToken);
        var paveCaution = await _context.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.Caution, cancellationToken);
        var paveNogo = await _context.PaveAssessments.CountAsync(a => a.Result == AssessmentResult.NoGo, cancellationToken);

        // Recent 10 assessments across both types
        var recentImsafe = await _context.ImSafeAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .Take(5).AsNoTracking().ToListAsync(cancellationToken);

        var recentPave = await _context.PaveAssessments
            .Include(a => a.Pilot)
            .OrderByDescending(a => a.AssessedAt)
            .Take(5).AsNoTracking().ToListAsync(cancellationToken);

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
