// src/lib/offlineBridge.ts
// Local offline bridge (no Electron/Tauri required).
// Keeps the same exported API so offline pages continue to work.

import type {
    FlightTripResponseDto, CreateFlightTripDto, UpdateFlightTripDto, LinkAssessmentDto,
    ImSafeResponseDto, PaveResponseDto,
    DecideSessionResponseDto,
    SmartWatchReadingResponseDto, CreateSmartWatchReadingDto, SmartWatchAnalysisDto,
    DashboardStatsDto,
} from './types'

type Store = {
    pilots: { id: string; name: string }[]
    flights: FlightTripResponseDto[]
    imsafe: ImSafeResponseDto[]
    pave: PaveResponseDto[]
    decide: DecideSessionResponseDto[]
    smartwatch: SmartWatchReadingResponseDto[]
}

const STORAGE_KEY = 'star-air-offline-store-v1'
const SEED_PILOT_PREFIX = 'seed-pilot-'

const now = () => new Date().toISOString()

const emptyStore = (): Store => ({ pilots: [], flights: [], imsafe: [], pave: [], decide: [], smartwatch: [] })

const toIsoAt = (date: Date, hour: number, minute = 0): string => {
    const d = new Date(date)
    d.setHours(hour, minute, 0, 0)
    return d.toISOString()
}

const startOfCurrentWeek = (): Date => {
    const d = new Date()
    const day = d.getDay() // 0=Sun
    const diff = day === 0 ? -6 : 1 - day // Monday as start
    d.setDate(d.getDate() + diff)
    d.setHours(0, 0, 0, 0)
    return d
}

const buildSeedStore = (): Store => {
    const pilots = [
        { id: 'seed-pilot-1', name: 'Capt. Omar Nabil' },
        { id: 'seed-pilot-2', name: 'Capt. Sara Adel' },
        { id: 'seed-pilot-3', name: 'Capt. Youssef Tarek' },
    ]

    const routes = [
        ['HECA', 'HESH'], ['HECA', 'HEGN'], ['HEGN', 'HESH'], ['HESH', 'HEBA'], ['HEBA', 'HECA'],
        ['HECA', 'HESN'], ['HESN', 'HEGN'], ['HEGN', 'HECA'], ['HEBA', 'HESH'], ['HESH', 'HECA'],
    ]
    const aircraft = ['A320', 'B737-800', 'A321neo', 'E190', 'ATR 72-600']
    const categories = ['Domestic', 'Training', 'Scheduled', 'Reposition']
    const levels: Array<'None' | 'Low' | 'Medium' | 'High'> = ['None', 'Low', 'Medium', 'High']

    const weekStart = startOfCurrentWeek()
    const flights: FlightTripResponseDto[] = []
    const imsafe: ImSafeResponseDto[] = []
    const pave: PaveResponseDto[] = []
    const smartwatch: SmartWatchReadingResponseDto[] = []
    const decide: DecideSessionResponseDto[] = []

    let imId = 1
    let pvId = 1
    let swId = 1
    let dcId = 1

    // Fixed weekly pattern so all 4 trend lines (Go/Caution/NoGo/Pending) are visible.
    const pattern = [
        'Go', 'Caution', 'NoGo', 'Pending', 'Go', 'Caution', 'Pending',
        'Go', 'NoGo', 'Caution', 'Go', 'Pending', 'Go', 'Caution', 'NoGo',
    ] as const

    for (let i = 0; i < pattern.length; i++) {
        const pilot = pilots[i % pilots.length]
        const route = routes[i % routes.length]
        const day = new Date(weekStart)
        day.setDate(weekStart.getDate() + (i % 7))

        const createdAt = toIsoAt(day, 7 + (i % 10), (i * 7) % 60)
        const departureTime = toIsoAt(day, 9 + (i % 9), (i * 11) % 60)
        const statusPattern = pattern[i]
        const pendingFlight = statusPattern === 'Pending'
        const cautionFlight = statusPattern === 'Caution'
        const nogoFlight = statusPattern === 'NoGo'

        let imSafeAssessmentId: number | undefined
        let paveAssessmentId: number | undefined
        let smartWatchReadingId: number | undefined
        let decideSessionId: number | undefined
        let status: FlightTripResponseDto['status'] = 'Pending'

        if (!pendingFlight) {
            const baseLevel = nogoFlight ? 3 : cautionFlight ? 2 : 1
            const illnessLevel = levels[baseLevel]
            const stressLevel = levels[Math.min(3, baseLevel + (i % 2))]
            const fatigueLevel = levels[Math.min(3, baseLevel)]

            const imScore = levelScore(illnessLevel) + levelScore('Low') + levelScore(stressLevel) + levelScore('None') + levelScore(fatigueLevel) + levelScore('Low')
            const imResult = scoreToResult(imScore)
            imSafeAssessmentId = imId++
            imsafe.push({
                id: imSafeAssessmentId,
                pilotId: pilot.id,
                pilotName: pilot.name,
                illnessLevel,
                medicationLevel: 'Low',
                stressLevel,
                alcoholLevel: 'None',
                fatigueLevel,
                emotionLevel: 'Low',
                dataSource: 'Seeded',
                overallRiskScore: imScore,
                result: imResult,
                assessedAt: toIsoAt(day, 8 + (i % 8), 15),
                isSynced: false,
            })

            const pvPilot = levels[Math.min(3, baseLevel)]
            const pvAircraft = levels[nogoFlight ? 3 : cautionFlight ? 2 : 1]
            const pvEnv = levels[Math.min(3, baseLevel + 1)]
            const pvExternal = levels[cautionFlight ? 2 : 1]
            const pvScore = levelScore(pvPilot) + levelScore(pvAircraft) + levelScore(pvEnv) + levelScore(pvExternal)
            const pvResult = scoreToResult(pvScore)
            paveAssessmentId = pvId++
            pave.push({
                id: paveAssessmentId,
                pilotId: pilot.id,
                pilotName: pilot.name,
                pilotRiskLevel: pvPilot,
                aircraftRiskLevel: pvAircraft,
                environmentRiskLevel: pvEnv,
                externalRiskLevel: pvExternal,
                pilotReadiness: 'Seeded weekly readiness',
                aircraftCondition: 'Seeded weekly condition',
                externalPressures: 'Seeded operational pressure',
                overallRiskScore: pvScore,
                result: pvResult,
                assessedAt: toIsoAt(day, 8 + (i % 8), 35),
                isSynced: false,
            })

            smartWatchReadingId = swId++
            smartwatch.push({
                id: smartWatchReadingId,
                pilotId: pilot.id,
                heartRate: 64 + (i % 18),
                heartRateVariability: 35 + (i % 22),
                sleepHours: 5.8 + (i % 4) * 0.6,
                sleepQuality: 58 + (i % 35),
                stressIndex: 20 + (i % 50),
                spO2: 94 + (i % 5),
                deviceName: 'Seeded Garmin Device',
                recordedAt: toIsoAt(day, 6 + (i % 5), 50),
                isSynced: false,
                isManualEntry: false,
            })

            decideSessionId = dcId++
            decide.push({
                id: decideSessionId,
                pilotId: pilot.id,
                pilotName: pilot.name,
                scenario: `Seeded current-week scenario #${i + 1}`,
                status: 'Completed',
                finalRiskScore: pvScore,
                startedAt: toIsoAt(day, 9 + (i % 4), 10),
                completedAt: toIsoAt(day, 9 + (i % 4), 20),
                isSynced: false,
                steps: [
                    { id: 1, sessionId: decideSessionId, stepType: 'Detect', stepOrder: 1, notes: 'Seeded detect step' },
                    { id: 2, sessionId: decideSessionId, stepType: 'Evaluate', stepOrder: 2, notes: 'Seeded evaluate step' },
                ],
            })

            status = (imResult === 'NoGo' || pvResult === 'NoGo') ? 'Pending' : 'Completed'
        }

        flights.push({
            id: i + 1,
            pilotId: pilot.id,
            pilotName: pilot.name,
            flightCategory: categories[i % categories.length],
            aircraftType: aircraft[i % aircraft.length],
            departure: route[0],
            arrival: route[1],
            departureTime,
            flightNumber: `ST-${String(200 + i)}`,
            status,
            imSafeAssessmentId,
            paveAssessmentId,
            decideSessionId,
            smartWatchReadingId,
            createdAt,
        })
    }

    return { pilots, flights, imsafe, pave, decide, smartwatch }
}

const hasSeedData = (store: Store): boolean =>
    store.pilots.some((p) => p.id.startsWith(SEED_PILOT_PREFIX))

const mergeStore = (base: Store, incoming: Store): Store => {
    const pilots = [...base.pilots, ...incoming.pilots.filter((p) => !base.pilots.some((x) => x.id === p.id))]
    const remapNumeric = <T extends { id: number }>(existing: T[], items: T[]) => {
        let nextId = (existing.length ? Math.max(...existing.map((x) => x.id)) : 0) + 1
        const map = new Map<number, number>()
        const mapped = items.map((x) => {
            const newId = nextId++
            map.set(x.id, newId)
            return { ...x, id: newId }
        })
        return { mapped, map }
    }

    const im = remapNumeric(base.imsafe, incoming.imsafe)
    const pv = remapNumeric(base.pave, incoming.pave)
    const dc = remapNumeric(base.decide, incoming.decide)
    const sw = remapNumeric(base.smartwatch, incoming.smartwatch)

    const flights = [...base.flights]
    let nextFlightId = (flights.length ? Math.max(...flights.map((f) => f.id)) : 0) + 1
    for (const f of incoming.flights) {
        flights.push({
            ...f,
            id: nextFlightId++,
            flightNumber: `${f.flightNumber}-S`,
            imSafeAssessmentId: f.imSafeAssessmentId ? im.map.get(f.imSafeAssessmentId) : undefined,
            paveAssessmentId: f.paveAssessmentId ? pv.map.get(f.paveAssessmentId) : undefined,
            decideSessionId: f.decideSessionId ? dc.map.get(f.decideSessionId) : undefined,
            smartWatchReadingId: f.smartWatchReadingId ? sw.map.get(f.smartWatchReadingId) : undefined,
        })
    }

    const imsafe = [...base.imsafe, ...im.mapped]
    const pave = [...base.pave, ...pv.mapped]
    const decide = [...base.decide, ...dc.mapped]
    const smartwatch = [...base.smartwatch, ...sw.mapped]

    return { pilots, flights, imsafe, pave, decide, smartwatch }
}

const shouldAutoseed = (store: Store): boolean => {
    const tooFewFlights = store.flights.length < 10
    const tooFewAssessments = (store.imsafe.length + store.pave.length) < 12
    return !hasSeedData(store) && (tooFewFlights || tooFewAssessments)
}

const readStore = (): Store => {
    if (typeof window === 'undefined') {
        return emptyStore()
    }
    const raw = window.localStorage.getItem(STORAGE_KEY)
    if (!raw) {
        const seeded = buildSeedStore()
        writeStore(seeded)
        return seeded
    }
    try {
        const parsed = JSON.parse(raw) as Store
        if (shouldAutoseed(parsed)) {
            const merged = mergeStore(parsed, buildSeedStore())
            writeStore(merged)
            return merged
        }
        if (!parsed.flights?.length && !parsed.imsafe?.length && !parsed.pave?.length) {
            const seeded = buildSeedStore()
            writeStore(seeded)
            return seeded
        }
        return parsed
    } catch {
        const seeded = buildSeedStore()
        writeStore(seeded)
        return seeded
    }
}

const writeStore = (store: Store) => {
    if (typeof window === 'undefined') return
    window.localStorage.setItem(STORAGE_KEY, JSON.stringify(store))
}

const nextId = (items: { id: number }[]) =>
    (items.length ? Math.max(...items.map((x) => x.id)) : 0) + 1

const levelScore = (level?: string): number => {
    if (level === 'High') return 3
    if (level === 'Medium') return 2
    if (level === 'Low') return 1
    return 0
}

const scoreToResult = (score: number): 'Go' | 'Caution' | 'NoGo' => {
    if (score >= 6) return 'NoGo'
    if (score >= 3) return 'Caution'
    return 'Go'
}

// ─── Local Pilot ────────────────────────────────────────────────────────────
export const offlinePilotApi = {
    create: async (name: string): Promise<{ id: string; name: string }> => {
        const store = readStore()
        const pilot = { id: `local-${Date.now()}`, name }
        store.pilots.unshift(pilot)
        writeStore(store)
        return pilot
    },
    getAll: async (): Promise<{ id: string; name: string }[]> => readStore().pilots,
    rename: async (id: string, name: string): Promise<{ id: string; name: string }> => {
        const store = readStore()
        const idx = store.pilots.findIndex((p) => p.id === id)
        if (idx === -1) throw new Error('Pilot not found')
        store.pilots[idx] = { ...store.pilots[idx], name }
        store.flights = store.flights.map((f) => (f.pilotId === id ? { ...f, pilotName: name } : f))
        store.imsafe = store.imsafe.map((a) => (a.pilotId === id ? { ...a, pilotName: name } : a))
        store.pave = store.pave.map((a) => (a.pilotId === id ? { ...a, pilotName: name } : a))
        store.decide = store.decide.map((d) => (d.pilotId === id ? { ...d, pilotName: name } : d))
        writeStore(store)
        return store.pilots[idx]
    },
}

export const offlineMaintenanceApi = {
    resetAndReseed: async (): Promise<{ pilots: number; flights: number; assessments: number }> => {
        const seeded = buildSeedStore()
        writeStore(seeded)
        return {
            pilots: seeded.pilots.length,
            flights: seeded.flights.length,
            assessments: seeded.imsafe.length + seeded.pave.length,
        }
    },
}

// ─── Flights ────────────────────────────────────────────────────────────────
export const offlineFlightApi = {
    create: async (dto: CreateFlightTripDto & { pilotName: string }): Promise<FlightTripResponseDto> => {
        const store = readStore()
        const flight: FlightTripResponseDto = {
            id: nextId(store.flights),
            pilotId: dto.pilotId,
            pilotName: dto.pilotName,
            flightCategory: dto.flightCategory,
            aircraftType: dto.aircraftType,
            departure: dto.departure,
            arrival: dto.arrival,
            departureTime: dto.departureTime,
            flightNumber: dto.flightNumber,
            status: 'Pending',
            createdAt: now(),
        }
        store.flights.unshift(flight)
        writeStore(store)
        return flight
    },
    getAll: async (): Promise<FlightTripResponseDto[]> => readStore().flights,
    getMy: async (pilotId: string): Promise<FlightTripResponseDto[]> =>
        readStore().flights.filter((f) => f.pilotId === pilotId),
    getById: async (id: number): Promise<FlightTripResponseDto> =>
        readStore().flights.find((f) => f.id === id) as FlightTripResponseDto,
    update: async (id: number, dto: UpdateFlightTripDto): Promise<FlightTripResponseDto> => {
        const store = readStore()
        const idx = store.flights.findIndex((f) => f.id === id)
        if (idx === -1) throw new Error('Flight not found')
        store.flights[idx] = {
            ...store.flights[idx],
            ...dto,
            status: (dto.status as FlightTripResponseDto['status'] | undefined) ?? store.flights[idx].status,
        }
        writeStore(store)
        return store.flights[idx]
    },
    delete: async (id: number): Promise<boolean> => {
        const store = readStore()
        store.flights = store.flights.filter((f) => f.id !== id)
        writeStore(store)
        return true
    },
    link: async (id: number, dto: LinkAssessmentDto & { weatherRiskLevel?: string; weatherSummary?: string }): Promise<FlightTripResponseDto> => {
        const store = readStore()
        const idx = store.flights.findIndex((f) => f.id === id)
        if (idx === -1) throw new Error('Flight not found')
        const current = store.flights[idx]
        const next = {
            ...current,
            ...dto,
            status: (dto.imSafeAssessmentId ?? current.imSafeAssessmentId) &&
                (dto.paveAssessmentId ?? current.paveAssessmentId) &&
                (dto.decideSessionId ?? current.decideSessionId)
                ? 'Cleared'
                : current.status,
        }
        store.flights[idx] = next
        writeStore(store)
        return next
    },
    complete: async (id: number): Promise<FlightTripResponseDto> => {
        const store = readStore()
        const idx = store.flights.findIndex((f) => f.id === id)
        if (idx === -1) throw new Error('Flight not found')
        store.flights[idx].status = 'Completed'
        writeStore(store)
        return store.flights[idx]
    },
}

// ─── IMSAFE ─────────────────────────────────────────────────────────────────
export interface OfflineImSafeDto {
    pilotId: string
    pilotName: string
    illnessLevel: string
    illnessNotes?: string
    medicationLevel: string
    medicationNotes?: string
    stressLevel: string
    stressNotes?: string
    alcoholLevel: string
    hoursSinceLastDrink?: number
    fatigueLevel: string
    hoursSlept?: number
    emotionLevel: string
    emotionNotes?: string
}

export const offlineImSafeApi = {
    create: async (dto: OfflineImSafeDto): Promise<ImSafeResponseDto> => {
        const store = readStore()
        const score =
            levelScore(dto.illnessLevel) +
            levelScore(dto.medicationLevel) +
            levelScore(dto.stressLevel) +
            levelScore(dto.alcoholLevel) +
            levelScore(dto.fatigueLevel) +
            levelScore(dto.emotionLevel)
        const item: ImSafeResponseDto = {
            id: nextId(store.imsafe),
            pilotId: dto.pilotId,
            pilotName: dto.pilotName,
            illnessLevel: dto.illnessLevel as any,
            illnessNotes: dto.illnessNotes,
            medicationLevel: dto.medicationLevel as any,
            medicationNotes: dto.medicationNotes,
            stressLevel: dto.stressLevel as any,
            stressNotes: dto.stressNotes,
            alcoholLevel: dto.alcoholLevel as any,
            hoursSinceLastDrink: dto.hoursSinceLastDrink,
            fatigueLevel: dto.fatigueLevel as any,
            hoursSlept: dto.hoursSlept,
            emotionLevel: dto.emotionLevel as any,
            emotionNotes: dto.emotionNotes,
            dataSource: 'Manual',
            overallRiskScore: score,
            result: scoreToResult(score),
            assessedAt: now(),
            isSynced: false,
        }
        store.imsafe.unshift(item)
        writeStore(store)
        return item
    },
    getAll: async (pilotId?: string): Promise<ImSafeResponseDto[]> =>
        pilotId ? readStore().imsafe.filter((x) => x.pilotId === pilotId) : readStore().imsafe,
}

// ─── PAVE ───────────────────────────────────────────────────────────────────
export interface OfflinePaveDto {
    pilotId: string
    pilotName: string
    aircraftRegistration?: string
    pilotReadiness?: string
    pilotRiskLevel: string
    aircraftCondition?: string
    aircraftRiskLevel: string
    weatherSummary?: string
    metarData?: string
    tafData?: string
    environmentRiskLevel: string
    externalPressures?: string
    externalRiskLevel: string
}

export const offlinePaveApi = {
    create: async (dto: OfflinePaveDto): Promise<PaveResponseDto> => {
        const store = readStore()
        const score =
            levelScore(dto.pilotRiskLevel) +
            levelScore(dto.aircraftRiskLevel) +
            levelScore(dto.environmentRiskLevel) +
            levelScore(dto.externalRiskLevel)
        const item: PaveResponseDto = {
            id: nextId(store.pave),
            pilotId: dto.pilotId,
            pilotName: dto.pilotName,
            aircraftRegistration: dto.aircraftRegistration,
            pilotReadiness: dto.pilotReadiness,
            pilotRiskLevel: dto.pilotRiskLevel as any,
            aircraftCondition: dto.aircraftCondition,
            aircraftRiskLevel: dto.aircraftRiskLevel as any,
            weatherSummary: dto.weatherSummary,
            metarData: dto.metarData,
            tafData: dto.tafData,
            environmentRiskLevel: dto.environmentRiskLevel as any,
            externalPressures: dto.externalPressures,
            externalRiskLevel: dto.externalRiskLevel as any,
            overallRiskScore: score,
            result: scoreToResult(score),
            assessedAt: now(),
            isSynced: false,
        }
        store.pave.unshift(item)
        writeStore(store)
        return item
    },
    getAll: async (pilotId?: string): Promise<PaveResponseDto[]> =>
        pilotId ? readStore().pave.filter((x) => x.pilotId === pilotId) : readStore().pave,
}

// ─── DECIDE ─────────────────────────────────────────────────────────────────
export interface OfflineDecideDto {
    pilotId: string
    pilotName: string
    scenario?: string
    finalRiskScore?: number
    steps: { stepType: string; description: string }[]
}

export const offlineDecideApi = {
    create: async (dto: OfflineDecideDto): Promise<DecideSessionResponseDto> => {
        const store = readStore()
        const item: DecideSessionResponseDto = {
            id: nextId(store.decide),
            pilotId: dto.pilotId,
            pilotName: dto.pilotName,
            scenario: dto.scenario,
            status: 'Completed',
            finalRiskScore: dto.finalRiskScore ?? 0,
            startedAt: now(),
            completedAt: now(),
            isSynced: false,
            steps: dto.steps.map((s, idx) => ({
                id: idx + 1,
                sessionId: nextId(store.decide),
                stepType: s.stepType,
                stepOrder: idx + 1,
                notes: s.description,
            })),
        }
        store.decide.unshift(item)
        writeStore(store)
        return item
    },
    getAll: async (pilotId?: string): Promise<DecideSessionResponseDto[]> =>
        pilotId ? readStore().decide.filter((x) => x.pilotId === pilotId) : readStore().decide,
}

// ─── SmartWatch ─────────────────────────────────────────────────────────────
export const offlineSmartWatchApi = {
    addReading: async (dto: CreateSmartWatchReadingDto & { pilotId: string }): Promise<SmartWatchReadingResponseDto> => {
        const store = readStore()
        const item: SmartWatchReadingResponseDto = {
            id: nextId(store.smartwatch),
            pilotId: dto.pilotId,
            flightTripId: dto.flightTripId,
            heartRate: dto.heartRate,
            heartRateVariability: dto.heartRateVariability,
            sleepHours: dto.sleepHours,
            sleepQuality: dto.sleepQuality,
            stressIndex: dto.stressIndex,
            spO2: dto.spO2,
            skinTemperature: dto.skinTemperature,
            steps: dto.steps,
            deviceName: dto.deviceName,
            recordedAt: dto.recordedAt ?? now(),
            isSynced: false,
            isManualEntry: dto.isManualEntry,
        }
        store.smartwatch.unshift(item)
        writeStore(store)
        return item
    },
    getReadings: async (pilotId: string): Promise<SmartWatchReadingResponseDto[]> =>
        readStore().smartwatch.filter((x) => x.pilotId === pilotId),
    getAnalysis: async (pilotId: string): Promise<SmartWatchAnalysisDto | null> => {
        const readings = readStore().smartwatch.filter((x) => x.pilotId === pilotId).slice(0, 7)
        if (!readings.length) return null
        const latest = readings[0]
        const avg = (selector: (x: SmartWatchReadingResponseDto) => number | undefined): number | undefined => {
            const vals = readings.map(selector).filter((v): v is number => typeof v === 'number')
            return vals.length ? Math.round((vals.reduce((a, b) => a + b, 0) / vals.length) * 10) / 10 : undefined
        }
        const riskScore =
            (latest.heartRate && latest.heartRate > 100 ? 2 : 0) +
            (latest.stressIndex && latest.stressIndex > 75 ? 3 : 0) +
            (latest.spO2 && latest.spO2 < 94 ? 2 : 0) +
            (latest.sleepHours && latest.sleepHours < 6 ? 2 : 0)
        return {
            latestHeartRate: latest.heartRate,
            averageSleepHours: avg((x) => x.sleepHours),
            averageStressIndex: avg((x) => x.stressIndex),
            averageSpO2: avg((x) => x.spO2),
            fitnessStatus: riskScore >= 7 ? 'Not Fit' : riskScore >= 3 ? 'Caution' : 'Fit',
            recommendation: riskScore >= 3 ? 'Recheck readiness before departure.' : 'Biometrics acceptable for flight.',
            riskScore,
        }
    },
}

// ─── Dashboard ──────────────────────────────────────────────────────────────
export const offlineDashboardApi = {
    getStats: async (pilotId?: string): Promise<DashboardStatsDto> => {
        const store = readStore()
        const imsafe = pilotId ? store.imsafe.filter((x) => x.pilotId === pilotId) : store.imsafe
        const pave = pilotId ? store.pave.filter((x) => x.pilotId === pilotId) : store.pave
        const all = [...imsafe, ...pave]
        return {
            totalPilots: store.pilots.length,
            activePilots: store.pilots.length,
            pendingPilots: 0,
            totalAircraft: 0,
            airworthyAircraft: 0,
            totalImSafeAssessments: imsafe.length,
            totalPaveAssessments: pave.length,
            goCount: all.filter((x) => x.result === 'Go').length,
            cautionCount: all.filter((x) => x.result === 'Caution').length,
            noGoCount: all.filter((x) => x.result === 'NoGo').length,
            recentAssessments: all.slice(0, 10).map((x) => ({
                type: 'illnessLevel' in x ? 'IMSAFE' : 'PAVE',
                pilotName: x.pilotName,
                result: x.result,
                riskScore: x.overallRiskScore,
                assessedAt: x.assessedAt,
            })),
        }
    },
}
