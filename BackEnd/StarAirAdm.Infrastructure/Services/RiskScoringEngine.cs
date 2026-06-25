using StarAirAdm.Domain.Enums;

namespace StarAirAdm.Infrastructure.Services;
public static class RiskScoringEngine
{
    private const int ImSafeCautionThreshold = 9;   // score ≥ 9 Caution
    private const int ImSafeNoGoThreshold    = 13;  // score ≥ 13 NoGo
   private const int PaveCautionThreshold = 3;    // score ≥ 3  Caution
    private const int PaveNoGoThreshold    = 6;    // score ≥ 6  → NoGo

    private const int SwCautionThreshold = 4;
    private const int SwNoGoThreshold    = 10;

    public static int ScoreRiskLevel(RiskLevel level) => level switch
    {
        RiskLevel.None   => 0,
        RiskLevel.Low    => 1,
        RiskLevel.Medium => 2,
        RiskLevel.High   => 3,
        _                => 0
    };

    public static (int score, AssessmentResult result) ScoreImSafe(
        RiskLevel illness, RiskLevel medication, RiskLevel stress,
        RiskLevel alcohol, RiskLevel fatigue, RiskLevel emotion,
        double? hoursSinceLastDrink, double? hoursSlept)
    {
        int score = ScoreRiskLevel(illness)
                  + ScoreRiskLevel(medication)
                  + ScoreRiskLevel(stress)
                  + ScoreRiskLevel(alcohol)
                  + ScoreRiskLevel(fatigue)
                  + ScoreRiskLevel(emotion);

        // Hard override 1: Alcohol High → automatic NoGo
        if (alcohol == RiskLevel.High)
            score = Math.Max(score, ImSafeNoGoThreshold);

        // Hard override 2: Illness High → automatic NoGo
        if (illness == RiskLevel.High)
            score = Math.Max(score, ImSafeNoGoThreshold);

        // Hard override 3: Fatigue High + very low sleep → automatic NoGo
        if (fatigue == RiskLevel.High && (hoursSlept ?? 99) < 4.0)
            score = Math.Max(score, ImSafeNoGoThreshold);

        // Hard override 4: Stress High → Caution minimum
        if (stress == RiskLevel.High)
            score = Math.Max(score, ImSafeCautionThreshold);

        // Hard override 5: Medication High (side effects) → Caution minimum
        if (medication == RiskLevel.High)
            score = Math.Max(score, ImSafeCautionThreshold);

        return (score, ImSafeScoreToResult(score));
    }

    private static AssessmentResult ImSafeScoreToResult(int score) =>
        score >= ImSafeNoGoThreshold    ? AssessmentResult.NoGo :
        score >= ImSafeCautionThreshold ? AssessmentResult.Caution :
        AssessmentResult.Go;
  public static (int score, AssessmentResult result) ScorePave(
        RiskLevel pilot, RiskLevel aircraft, RiskLevel environment, RiskLevel external)
    {
        int score = ScoreRiskLevel(pilot)
                  + ScoreRiskLevel(aircraft)
                  + ScoreRiskLevel(environment)
                  + ScoreRiskLevel(external);

        // Hard override: Aircraft flagged as No (High) → automatic NoGo.
        // Per PDF: "verify airworthiness" is a hard gate; any concern = NoGo.
        if (aircraft == RiskLevel.High)
            score = Math.Max(score, PaveNoGoThreshold);

        return (score, PaveScoreToResult(score));
    }

    private static AssessmentResult PaveScoreToResult(int score) =>
        score >= PaveNoGoThreshold    ? AssessmentResult.NoGo :
        score >= PaveCautionThreshold ? AssessmentResult.Caution :
        AssessmentResult.Go;

    
    public static (int riskScore, string fitnessStatus, string recommendation) AnalyzeSmartWatch(
        int? heartRate, int? hrv, int? stressIndex, int? spO2, double? sleepHours, int? sleepQuality)
    {
        int score = 0;
        var issues = new List<string>();

        // Heart rate (resting): normal resting HR for pilots is 50–90 bpm
        if (heartRate.HasValue)
        {
            if (heartRate > 100) { score += 2; issues.Add("Elevated resting heart rate (>100 bpm) — possible stress, illness, or fatigue"); }
            else if (heartRate > 90) { score += 1; }
        }

        // HRV: higher HRV = better recovery; lower = more stressed/fatigued
        if (hrv.HasValue)
        {
            if (hrv < 20) { score += 2; issues.Add("Very low heart rate variability (<20 ms) — high stress or fatigue likely"); }
            else if (hrv < 40) { score += 1; }
        }

        // Stress index (0–100 scale)
        if (stressIndex.HasValue)
        {
            if (stressIndex > 75) { score += 3; issues.Add("Critical stress level (>75/100) — cognitive performance significantly impaired"); }
            else if (stressIndex > 50) { score += 2; issues.Add("Elevated stress level (>50/100) — decision-making may be affected"); }
            else if (stressIndex > 30) { score += 1; }
        }

        // SpO2: critical for hypoxia awareness (esp. above 12,500 ft MSL)
        if (spO2.HasValue)
        {
            if (spO2 < 90) { score += 3; issues.Add("Dangerous SpO2 (<90%) — hypoxia risk; do NOT fly without supplemental oxygen"); }
            else if (spO2 < 94) { score += 2; issues.Add("Low blood oxygen saturation (<94%) — monitor closely; consider supplemental O2"); }
            else if (spO2 < 96) { score += 1; }
        }

        // Sleep: per PDF fatigue guidance (<4h = high severity; <6h = Medium; <7h = Low concern)
        if (sleepHours.HasValue)
        {
            if (sleepHours < 4) { score += 3; issues.Add("Critically insufficient sleep (<4 h) — assign High severity per IMSAFE fatigue guidance"); }
            else if (sleepHours < 6) { score += 2; issues.Add("Insufficient sleep (<6 h) — impairs every phase of flight; consider cancellation"); }
            else if (sleepHours < 7) { score += 1; }
        }

        // Sleep quality (<40/100 = poor restorative sleep)
        if (sleepQuality.HasValue && sleepQuality < 40) { score += 1; issues.Add("Poor sleep quality (<40/100) — even adequate hours may not be restorative"); }

        var fitnessStatus = score >= SwNoGoThreshold ? "Not Fit" : score >= SwCautionThreshold ? "Caution" : "Fit";
        var recommendation = issues.Count > 0
            ? string.Join("; ", issues)
            : "Biometrics within acceptable flight parameters — proceed with standard preflight checks";

        return (score, fitnessStatus, recommendation);
    }
}
