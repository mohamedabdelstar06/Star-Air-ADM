import { useEffect, useMemo, useState } from 'react'
import { useModeStore } from '../../stores/modeStore'
import { offlineFlightApi, offlineImSafeApi, offlinePaveApi, offlinePilotApi } from '../../lib/offlineBridge'
import type { FlightTripResponseDto, ImSafeResponseDto, PaveResponseDto } from '../../lib/types'
import { AdvancedTrendChart, SystemPieChart } from '../../components/shared/AdvancedCharts'

export function OfflineProfilePage() {
    const { offlinePilot, enterOfflineMode } = useModeStore()
    const [name, setName] = useState(offlinePilot?.name ?? '')
    const [saving, setSaving] = useState(false)
    const [message, setMessage] = useState<string | null>(null)
    const [assessments, setAssessments] = useState<(ImSafeResponseDto | PaveResponseDto)[]>([])
    const [flights, setFlights] = useState<FlightTripResponseDto[]>([])

    const load = async () => {
        if (!offlinePilot) return
        const [im, pv, fl] = await Promise.all([
            offlineImSafeApi.getAll(offlinePilot.id),
            offlinePaveApi.getAll(offlinePilot.id),
            offlineFlightApi.getMy(offlinePilot.id),
        ])
        setAssessments([...im, ...pv])
        setFlights(fl)
    }

    useEffect(() => {
        setName(offlinePilot?.name ?? '')
        load()
    }, [offlinePilot?.id])

    const stats = useMemo(() => {
        const go = assessments.filter((a) => a.result === 'Go').length
        const caution = assessments.filter((a) => a.result === 'Caution').length
        const nogo = assessments.filter((a) => a.result === 'NoGo').length
        return { go, caution, nogo, total: assessments.length }
    }, [assessments])

    const saveName = async () => {
        if (!offlinePilot) return
        const trimmed = name.trim()
        if (!trimmed) return
        setSaving(true)
        setMessage(null)
        try {
            const updated = await offlinePilotApi.rename(offlinePilot.id, trimmed)
            enterOfflineMode(updated)
            setMessage('Offline profile updated.')
            await load()
        } catch {
            setMessage('Could not update offline profile.')
        } finally {
            setSaving(false)
        }
    }

    if (!offlinePilot) return null

    return (
        <div className="space-y-6">
            <div className="glass-card p-6 border-2 border-slate-200">
                <h1 className="text-2xl font-black text-black mb-5">My Offline Profile</h1>
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
                    <div className="lg:col-span-2 space-y-2">
                        <label className="text-sm font-bold text-slate-800 uppercase tracking-wider">Pilot Name</label>
                        <input
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            className="w-full px-4 py-3.5 rounded-xl border-2 border-slate-300 text-slate-900 font-semibold text-[15px] focus:outline-none focus:border-primary-500"
                        />
                    </div>
                    <div className="flex items-end">
                        <button
                            onClick={saveName}
                            disabled={saving || !name.trim()}
                            className="w-full px-5 py-3.5 rounded-xl bg-primary-600 hover:bg-primary-700 text-white font-bold disabled:opacity-60"
                        >
                            {saving ? 'Saving...' : 'Save Name'}
                        </button>
                    </div>
                </div>
                {message && <p className="mt-3 text-sm font-semibold text-slate-700">{message}</p>}
                <div className="grid grid-cols-2 lg:grid-cols-4 gap-3 mt-6">
                    <div className="metric-card"><div className="metric-value text-slate-900">{flights.length}</div><div className="metric-label">Flights</div></div>
                    <div className="metric-card"><div className="metric-value text-green-600">{stats.go}</div><div className="metric-label">Go</div></div>
                    <div className="metric-card"><div className="metric-value text-amber-600">{stats.caution}</div><div className="metric-label">Caution</div></div>
                    <div className="metric-card"><div className="metric-value text-red-600">{stats.nogo}</div><div className="metric-label">No-Go</div></div>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
                <div className="lg:col-span-2 min-w-0">
                    <AdvancedTrendChart assessments={assessments} flights={flights} title="My Offline Trends" />
                </div>
                <div className="lg:col-span-1 min-w-0">
                    <SystemPieChart assessments={assessments} />
                </div>
            </div>
        </div>
    )
}
