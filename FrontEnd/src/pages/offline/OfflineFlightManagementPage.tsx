// src/pages/offline/OfflineFlightManagementPage.tsx
// Create & manage local flights — identical UX to online mode (PilotDashboard + FlightManagement).
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useModeStore } from '../../stores/modeStore'
import { offlineFlightApi } from '../../lib/offlineBridge'
import type { FlightTripResponseDto } from '../../lib/types'
import { FLIGHT_CATEGORIES, getAircraftByCategory } from '../../lib/aircraftData'
import { Plane, Plus, Trash2, ChevronRight, RefreshCw, Loader2, Calendar, CheckCircle, Clock, X, Watch, ShieldCheck, ClipboardCheck, Brain } from 'lucide-react'
import clsx from 'clsx'
import { TripAssessmentBadge } from '../../components/shared/TripAssessmentBadge'
import { searchAerodromes, type Aerodrome } from '../../lib/aerodromeData'
import { useRef } from 'react'

function AerodromeDropdown({ value, onChange, label }: {
    value: string
    onChange: (icao: string) => void
    label: string
}) {
    const [query, setQuery] = useState(value)
    const [results, setResults] = useState<Aerodrome[]>([])
    const [open, setOpen] = useState(false)
    const ref = useRef<HTMLDivElement>(null)

    useEffect(() => {
        setResults(searchAerodromes(query, 15))
    }, [query])

    useEffect(() => {
        setQuery(value)
    }, [value])

    useEffect(() => {
        const handleClick = (e: MouseEvent) => {
            if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false)
        }
        document.addEventListener('mousedown', handleClick)
        return () => document.removeEventListener('mousedown', handleClick)
    }, [])

    const handleSelect = (a: Aerodrome) => {
        onChange(a.icao)
        setQuery(`${a.icao} — ${a.city}`)
        setOpen(false)
    }

    return (
        <div className="space-y-1.5" ref={ref}>
            <label className="text-xs font-bold text-black uppercase tracking-widest">{label}</label>
            <input
                className="w-full bg-white border-2 border-slate-300 rounded-xl px-3 py-2.5 text-black outline-none focus:border-primary-500 transition-all font-mono uppercase placeholder:text-slate-400 placeholder:normal-case text-sm"
                placeholder="Search ICAO, city, country..."
                value={query}
                onChange={e => { setQuery(e.target.value); setOpen(true); onChange('') }}
                onFocus={() => setOpen(true)}
                required
            />
            {open && results.length > 0 && (
                <div className="absolute z-50 w-full max-h-56 overflow-y-auto bg-white border-2 border-slate-300 rounded-xl shadow-xl mt-1">
                    {results.map(a => (
                        <button
                            key={a.icao}
                            type="button"
                            onClick={() => handleSelect(a)}
                            className="w-full text-left px-4 py-2.5 hover:bg-primary-50 border-b border-slate-100 last:border-0 transition-colors"
                        >
                            <div className="flex items-center justify-between">
                                <span className="font-mono font-bold text-black text-sm">{a.icao}</span>
                                <span className="text-xs text-slate-500 font-semibold">{a.iata}</span>
                            </div>
                            <div className="text-sm text-slate-700">{a.city}, {a.country}</div>
                            <div className="text-xs text-slate-400 truncate">{a.name}</div>
                        </button>
                    ))}
                </div>
            )}
        </div>
    )
}


export function OfflineFlightManagementPage() {
    const { offlinePilot } = useModeStore()
    const navigate = useNavigate()
    const [flights, setFlights] = useState<FlightTripResponseDto[]>([])
    const [loading, setLoading] = useState(true)
    const [showModal, setShowModal] = useState(false)
    const [saving, setSaving] = useState(false)
    
    const [form, setForm] = useState({
        flightNumber: '',
        flightCategory: '',
        aircraftType: '',
        departure: '',
        arrival: '',
        departureTime: '',
    })

    const load = async () => {
        setLoading(true)
        try {
            const data = await offlineFlightApi.getMy(offlinePilot?.id ?? '')
            setFlights(data)
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => { load() }, [offlinePilot?.id])

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault()
        if (!form.flightCategory || !form.aircraftType || !form.departure || !form.arrival || !form.departureTime) return
        setSaving(true)
        try {
            await offlineFlightApi.create({
                pilotId: offlinePilot!.id,
                pilotName: offlinePilot!.name,
                ...form,
            })
            setForm({ flightNumber: '', flightCategory: '', aircraftType: '', departure: '', arrival: '', departureTime: '' })
            setShowModal(false)
            load()
        } finally {
            setSaving(false)
        }
    }

    const handleDelete = async (id: number) => {
        if (!confirm('Delete this flight and all its linked assessments?')) return
        await offlineFlightApi.delete(id)
        load()
    }

    const availableAircraft = form.flightCategory ? getAircraftByCategory(form.flightCategory) : []

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3">
                <div>
                    <h1 className="text-xl sm:text-2xl font-bold text-black flex items-center gap-3">
                        <Plane size={24} className="text-primary-500" /> My Flights
                    </h1>
                    <p className="text-slate-600 text-sm mt-1">{flights.length} local flight{flights.length !== 1 ? 's' : ''}</p>
                </div>
                <div className="flex items-center gap-2">
                    <button onClick={load} className="p-2.5 rounded-xl border-2 border-slate-200 hover:bg-slate-50 transition-colors text-slate-500 hover:text-black font-bold">
                        <RefreshCw size={18} />
                    </button>
                    <button
                        onClick={() => setShowModal(true)}
                        className="btn-primary flex items-center gap-2 px-4 py-2.5"
                    >
                        <Plus size={18} /> New Trip
                    </button>
                </div>
            </div>

            {/* Flight list */}
            {loading ? (
                <div className="flex justify-center py-16"><Loader2 size={28} className="animate-spin text-primary-500" /></div>
            ) : flights.length === 0 ? (
                <div className="glass-card p-12 text-center border-2 border-dashed border-slate-300">
                    <Plane size={48} className="mx-auto text-slate-400 mb-4" />
                    <h3 className="text-lg font-bold text-black">No Flights Yet</h3>
                    <p className="text-slate-600 mt-2 text-sm">Create your first local flight to begin pre-flight preparation.</p>
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                    {flights.map(f => {
                        const swDone = !!f.smartWatchReadingId
                        const imDone = !!f.imSafeAssessmentId
                        const paveDone = !!f.paveAssessmentId
                        const decideDone = !!f.decideSessionId
                        const completedCount = [swDone, imDone, paveDone, decideDone].filter(Boolean).length

                        return (
                            <div key={f.id} className="bg-white rounded-2xl p-5 relative group overflow-hidden flex flex-col gap-4 shadow-xl shadow-slate-300/80 hover:shadow-2xl hover:shadow-slate-400 hover:-translate-y-1 transition-all border-2 border-blue-400/60 cursor-pointer" onClick={() => navigate(`/offline/flights/${f.id}`)}>
                                <div className="flex items-start justify-between">
                                    <div className="flex items-center gap-3">
                                        <div className="w-10 h-10 rounded-full bg-primary-100 flex items-center justify-center text-primary-600">
                                            <Plane size={20} />
                                        </div>
                                        <div>
                                            <div className="text-base font-bold text-black">{f.flightNumber || `Trip #${f.id}`}</div>
                                            <div className="text-sm text-slate-600">{f.aircraftType}</div>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        {f.status === 'Completed' ? (
                                            <span className="px-2 py-0.5 rounded-full text-xs font-bold bg-blue-100 text-blue-700">Completed</span>
                                        ) : completedCount === 4 ? (
                                            <span className="px-2 py-0.5 rounded-full text-xs font-bold bg-green-100 text-green-700">Cleared</span>
                                        ) : (
                                            <span className="px-2 py-0.5 rounded-full text-xs font-bold bg-amber-100 text-amber-700">Pending</span>
                                        )}
                                        <button onClick={(e) => { e.stopPropagation(); handleDelete(f.id); }} className="text-red-500 p-1.5 rounded-lg bg-red-50 hover:bg-red-100 transition-colors pointer-events-auto opacity-100" title="Delete Trip">
                                            <Trash2 size={16} />
                                        </button>
                                    </div>
                                </div>

                                {/* Route */}
                                <div className="flex items-center justify-between text-black bg-slate-50 px-3 py-3 rounded-xl border border-slate-200">
                                    <div className="text-center flex-1 font-black text-xl text-black">{f.departure}</div>
                                    <div className="px-2 text-primary-500 font-black text-xl">→</div>
                                    <div className="text-center flex-1 font-black text-xl text-black">{f.arrival}</div>
                                </div>

                                {/* Date */}
                                <div className="flex items-center gap-2 text-sm text-black">
                                    <Calendar size={14} className="text-primary-500" />
                                    {new Date(f.departureTime).toLocaleString()}
                                </div>

                                {/* Assessment progress */}
                                <div className="space-y-2">
                                    <div className="flex justify-between text-xs text-slate-600 uppercase tracking-widest font-bold">
                                        <span>Required Checks</span>
                                        <span>{completedCount}/4</span>
                                    </div>
                                    <div className="flex gap-1.5">
                                        {[
                                            { label: 'SW', done: swDone, icon: Watch },
                                            { label: 'IM', done: imDone, icon: ShieldCheck },
                                            { label: 'PV', done: paveDone, icon: ClipboardCheck },
                                            { label: 'DC', done: decideDone, icon: Brain },
                                        ].map(({ label, done, icon: Icon }) => (
                                            <div key={label} className={clsx('flex-1 flex flex-col items-center gap-1 py-2 rounded-lg border',
                                                done ? 'bg-green-100 text-green-700 border-green-200' : 'bg-slate-100 text-black border-slate-200')}>
                                                <Icon size={16} className={done ? "text-green-700" : "text-black"} />
                                                <span className="text-sm font-black text-black">{label}</span>
                                            </div>
                                        ))}
                                    </div>
                                    <div className="w-full h-1.5 bg-slate-200 rounded-full overflow-hidden">
                                        <div
                                            className="h-full bg-gradient-to-r from-primary-500 to-green-500 rounded-full transition-all duration-500"
                                            style={{ width: `${(completedCount / 4) * 100}%` }}
                                        />
                                    </div>
                                </div>

                                <button onClick={(e) => { e.stopPropagation(); navigate(`/offline/flights/${f.id}`); }}
                                    className="mt-auto w-full text-center py-2.5 bg-primary-600 hover:bg-primary-700 text-white text-sm font-bold rounded-xl transition-all flex items-center justify-center gap-2">
                                    {completedCount === 4 ? <><CheckCircle size={14} /> View Trip</> : <><Clock size={14} /> Continue Preparation</>}
                                </button>
                            </div>
                        )
                    })}
                </div>
            )}

            {/* Create Modal */}
            {showModal && (
                <div className="fixed inset-0 z-50 flex items-end sm:items-start justify-center bg-black/40 backdrop-blur-sm p-0 sm:p-4 sm:pt-10 overflow-y-auto">
                    <div className="glass-card w-full sm:max-w-4xl overflow-hidden border border-slate-300 shadow-2xl rounded-t-2xl sm:rounded-2xl max-h-[95vh] sm:max-h-none overflow-y-auto">
                        <div className="px-4 sm:px-6 py-3 sm:py-4 border-b border-slate-200 flex items-center justify-between bg-primary-50">
                            <h2 className="text-xl font-bold text-black flex items-center gap-2">
                                <Plane size={20} className="text-primary-500" /> New Local Flight
                            </h2>
                            <button onClick={() => setShowModal(false)} className="text-slate-500 hover:text-black transition-colors">
                                <X size={20} />
                            </button>
                        </div>
                        <form onSubmit={handleCreate} className="p-4 sm:p-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-5">
                                <div className="space-y-1.5 col-span-1 md:col-span-2">
                                    <label className="text-sm font-bold text-black uppercase tracking-widest">Flight Number</label>
                                    <input
                                        type="text"
                                        placeholder="STR-001"
                                        value={form.flightNumber}
                                        onChange={e => setForm(f => ({ ...f, flightNumber: e.target.value }))}
                                        className="w-full bg-white border-2 border-slate-300 rounded-xl px-4 py-3 text-black outline-none focus:border-primary-500 transition-all text-base"
                                    />
                                </div>
                                <div className="space-y-1.5">
                                    <label className="text-sm font-bold text-black uppercase tracking-widest">Trip Type *</label>
                                    <select
                                        value={form.flightCategory}
                                        onChange={e => setForm(f => ({ ...f, flightCategory: e.target.value, aircraftType: '' }))}
                                        required
                                        className="w-full bg-white border-2 border-slate-300 rounded-xl px-4 py-3 text-black outline-none focus:border-primary-500 transition-all text-base"
                                    >
                                        <option value="">Select trip type...</option>
                                        {FLIGHT_CATEGORIES.map(c => (
                                            <option key={c.key} value={c.key}>{c.emoji} {c.label}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="space-y-1.5">
                                    <label className="text-sm font-bold text-black uppercase tracking-widest">Aircraft Type *</label>
                                    <select
                                        value={form.aircraftType}
                                        onChange={e => setForm(f => ({ ...f, aircraftType: e.target.value }))}
                                        required
                                        disabled={!form.flightCategory}
                                        className="w-full bg-white border-2 border-slate-300 rounded-xl px-4 py-3 text-black outline-none focus:border-primary-500 transition-all text-base"
                                    >
                                        <option value="">{form.flightCategory ? 'Select aircraft type...' : 'Select trip type first...'}</option>
                                        {availableAircraft.map(a => (
                                            <option key={a} value={a}>{a}</option>
                                        ))}
                                    </select>
                                </div>
                                <div className="relative">
                                    <AerodromeDropdown
                                        value={form.departure}
                                        onChange={(v) => setForm(f => ({ ...f, departure: v }))}
                                        label="Origin (ICAO) *"
                                    />
                                </div>
                                <div className="relative">
                                    <AerodromeDropdown
                                        value={form.arrival}
                                        onChange={(v) => setForm(f => ({ ...f, arrival: v }))}
                                        label="Destination (ICAO) *"
                                    />
                                </div>
                                <div className="space-y-1.5">
                                    <label className="text-sm font-bold text-black uppercase tracking-widest">Departure Time *</label>
                                    <input
                                        type="datetime-local"
                                        required
                                        value={form.departureTime}
                                        onChange={e => setForm(f => ({ ...f, departureTime: e.target.value }))}
                                        className="w-full bg-white border-2 border-slate-300 rounded-xl px-4 py-3 text-black outline-none focus:border-primary-500 transition-all text-base"
                                    />
                                </div>
                            </div>
                            <div className="flex gap-4 pt-4 mt-2">
                                <button type="button" onClick={() => setShowModal(false)} className="flex-1 px-4 py-3 bg-slate-100 hover:bg-slate-200 text-black font-bold rounded-xl transition-all border border-slate-300 text-base">
                                    Cancel
                                </button>
                                <button type="submit" disabled={saving} className="flex-[2] px-4 py-3 bg-primary-600 hover:bg-primary-700 text-white font-bold rounded-xl transition-all flex items-center justify-center gap-2 text-base">
                                    {saving && <Loader2 size={18} className="animate-spin" />}
                                    Create Flight
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    )
}
