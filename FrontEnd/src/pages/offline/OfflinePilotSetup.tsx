import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useModeStore, isElectron } from '../../stores/modeStore'
import { offlinePilotApi } from '../../lib/offlineBridge'
import { Loader2, Radio, User, WifiOff, Monitor } from 'lucide-react'

export function OfflinePilotSetup() {
    const [name, setName] = useState('')
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const { enterOfflineMode } = useModeStore()
    const navigate = useNavigate()

    const handleStart = async (e: React.FormEvent) => {
        e.preventDefault()
        setError(null)
        const trimmed = name.trim()
        
        if (!trimmed) { setError('Name is required'); return }
        
        if (!isElectron()) {
            // Browser fallback
            enterOfflineMode({ id: `local-${Date.now()}`, name: trimmed })
            navigate('/offline/dashboard')
            return
        }
        
        setLoading(true)
        try {
            const pilot = await offlinePilotApi.create(trimmed)
            enterOfflineMode(pilot)
            navigate('/offline/dashboard')
        } catch (err: any) {
            setError('Failed to create local profile. Ensure the desktop app is running.')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="min-h-dvh flex bg-white">
            {/* Left — Image & Motivation */}
            <div className="hidden lg:flex lg:w-1/2 relative bg-slate-900 overflow-hidden items-end p-12 lg:p-16">
                <img
                    src="https://images.unsplash.com/photo-1542296332-2e4473faf563?w=1200&auto=format&fit=crop"
                    alt="Aviation Cockpit Dashboard at Night"
                    className="absolute inset-0 w-full h-full object-cover scale-105"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-slate-950 via-slate-900/50 to-transparent" />
                <div className="absolute inset-0 bg-primary-900/20 mix-blend-multiply" />
                <div className="relative z-10 text-white max-w-xl animate-slide-up">
                    <div className="inline-flex items-center justify-center p-2.5 rounded-xl bg-amber-500/20 backdrop-blur-md mb-6 border border-amber-500/30 shadow-xl">
                        <Monitor size={24} className="text-amber-400" />
                    </div>
                    <h2 className="text-3xl lg:text-4xl font-black mb-5 leading-tight tracking-tight">
                        Fly anytime, anywhere.<br />
                        <span className="text-amber-400">100% Offline Mode.</span>
                    </h2>
                    <p className="text-base text-slate-300 font-medium leading-relaxed">
                        STAR Air ADM Desktop lets you perform IMSAFE & PAVE assessments, use the DECIDE model, and track flights without an internet connection or account.
                    </p>
                </div>
            </div>

            {/* Right — Login Form */}
            <div className="w-full lg:w-1/2 flex flex-col justify-center items-center p-6 sm:p-12 bg-slate-50">
                {/* Mobile logo */}
                <div className="lg:hidden text-center mb-6 animate-slide-up">
                    <div className="inline-flex items-center justify-center w-12 h-12 rounded-xl bg-amber-600 shadow-lg mb-2 border border-amber-500/30">
                        <Monitor size={22} className="text-white" />
                    </div>
                    <h1 className="text-xl font-bold text-slate-900 tracking-tight">STAR Air ADM (Offline)</h1>
                </div>

                <div className="w-full max-w-md animate-slide-up" style={{ animationDelay: '100ms' }}>
                    <div className="bg-white rounded-2xl p-6 sm:p-8 shadow-sm border border-slate-200">
                        <div className="inline-flex items-center gap-2 px-3 py-1 bg-amber-500/10 border border-amber-500/20 rounded-full text-amber-600 text-xs font-bold uppercase tracking-widest mb-4">
                            <WifiOff size={12} />
                            <span>Pilot Free Mode</span>
                        </div>
                        <h2 className="text-xl font-black text-slate-900 mb-1">Enter your name</h2>
                        <p className="text-sm font-medium text-slate-500 mb-6">No account required. All data is saved locally.</p>

                        <form onSubmit={handleStart} className="space-y-5" noValidate>
                            {/* Name Input */}
                            <div>
                                <label className="text-xs font-bold text-slate-800 uppercase tracking-widest mb-2 block" htmlFor="name">
                                    Your Full Name
                                </label>
                                <div className="relative">
                                    <User size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none" />
                                    <input
                                        id="name"
                                        type="text"
                                        value={name}
                                        onChange={(e) => { setName(e.target.value); setError(null); }}
                                        placeholder="e.g. Capt. Ahmed Hassan"
                                        autoFocus
                                        className="w-full pl-10 pr-4 py-3 bg-white border-2 border-slate-200 rounded-xl text-slate-900 font-medium focus:outline-none focus:border-amber-500 transition-colors placeholder:text-slate-300 text-base"
                                    />
                                </div>
                            </div>

                            {/* Error */}
                            {error && (
                                <div className="flex items-start gap-2 px-4 py-3 bg-red-50 border border-red-200 rounded-xl text-red-700 text-sm font-medium animate-fade-in">
                                    <span className="w-1.5 h-1.5 rounded-full bg-red-500 flex-shrink-0 mt-1.5" />
                                    {error}
                                </div>
                            )}

                            <button
                                type="submit"
                                disabled={loading || !name.trim()}
                                className="w-full flex items-center justify-center gap-2 bg-slate-900 hover:bg-slate-800 text-white font-bold py-3.5 px-4 rounded-xl transition-all disabled:opacity-70 disabled:cursor-not-allowed shadow-md active:scale-[0.98] text-base mt-2"
                            >
                                {loading ? <Loader2 size={18} className="animate-spin" /> : null}
                                Start Flying Locally
                            </button>
                        </form>
                    </div>

                    <div className="mt-6 text-center">
                        <p className="text-xs font-semibold text-slate-400 uppercase tracking-widest mb-3">STAR Air ADM v1.0 (Desktop)</p>
                        <Link to="/" className="text-sm font-medium text-slate-500 hover:text-slate-800 transition-colors">
                            ← Back to Home Page
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    )
}

