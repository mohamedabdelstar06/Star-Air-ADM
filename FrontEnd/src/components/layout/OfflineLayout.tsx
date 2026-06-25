// src/components/layout/OfflineLayout.tsx
import { Outlet, NavLink, useNavigate, useLocation } from 'react-router-dom'
import { useModeStore } from '../../stores/modeStore'
import { useUIStore } from '../../stores/uiStore'
import {
    LayoutDashboard, Users, Plane, ShieldCheck, ClipboardCheck,
    Brain, Watch, LogOut, WifiOff, Menu, X, ChevronLeft, ChevronRight, Bird,
    Bell
} from 'lucide-react'
import { useState, useEffect, useRef } from 'react'
import clsx from 'clsx'

// 1. Same route structure as the online sidebar but for offline
interface NavGroup {
    label: string
    items: { to: string; icon: React.ReactNode; label: string }[]
}

const navigation: NavGroup[] = [
    {
        label: 'Overview',
        items: [
            { to: '/offline/dashboard', icon: <LayoutDashboard size={18} />, label: 'Dashboard' },
        ],
    },
    {
        label: 'Management',
        items: [
            { to: '/offline/flights', icon: <Plane size={18} />, label: 'Flight Missions' },
        ],
    },
    {
        label: 'Pilot Tools',
        items: [
            { to: '/offline/smartwatch', icon: <Watch size={18} />, label: 'SmartWatch' },
            { to: '/offline/profile', icon: <Users size={18} />, label: 'Profile' },
        ],
    },
]

const routeTitles: Record<string, string> = {
    '/offline/dashboard': 'Dashboard',
    '/offline/flights': 'Flight Missions',
    '/offline/smartwatch': 'SmartWatch',
    '/offline/profile': 'Profile',
    '/offline/imsafe': 'IMSAFE Assessment',
    '/offline/pave': 'PAVE Assessment',
    '/offline/decide': 'DECIDE Model',
}

// 2. Clone of the online Sidebar but offline data
function OfflineSidebar() {
    const { offlinePilot, exitOfflineMode } = useModeStore()
    const { sidebarCollapsed, toggleCollapse, setSidebarOpen } = useUIStore()
    const navigate = useNavigate()

    const handleExit = () => {
        exitOfflineMode()
        navigate('/')
    }

    return (
        <aside
            className={clsx(
                'flex flex-col h-full bg-slate-50 border-r border-slate-200 transition-all duration-300',
                sidebarCollapsed ? 'w-16' : 'w-56 lg:w-60'
            )}
        >
            {/* Logo */}
            <div className="flex items-center gap-2.5 px-3 py-4 border-b border-slate-200">
                <button
                    onClick={toggleCollapse}
                    className="flex-shrink-0 w-8 h-8 rounded-lg bg-primary-600 hover:bg-primary-700 flex items-center justify-center shadow-sm transition-colors"
                >
                    <Bird size={16} className="text-white" />
                </button>
                {!sidebarCollapsed && (
                    <div className="animate-fade-in flex-1 cursor-pointer" onClick={toggleCollapse}>
                        <div className="text-sm font-bold text-slate-900 leading-tight">EgyptAir</div>
                        <div className="text-xs text-slate-600">STAR ADM</div>
                    </div>
                )}
                <button
                    onClick={toggleCollapse}
                    className="ml-auto btn-icon hidden lg:flex"
                    aria-label="Toggle sidebar"
                >
                    {sidebarCollapsed ? <ChevronRight size={16} /> : <ChevronLeft size={16} />}
                </button>
            </div>

            {/* Nav */}
            <nav className="flex-1 overflow-y-auto py-3 px-1.5 space-y-3">
                {navigation.map((group) => (
                    <div key={group.label}>
                        {!sidebarCollapsed && (
                            <p className="px-3 mb-1 text-[10px] font-semibold uppercase tracking-widest text-slate-600">
                                {group.label}
                            </p>
                        )}
                        <div className="space-y-0.5">
                            {group.items.map((item) => (
                                <NavLink
                                    key={item.to}
                                    to={item.to}
                                    onClick={() => setSidebarOpen(false)}
                                    className={({ isActive }) =>
                                        clsx(isActive ? 'nav-item-active' : 'nav-item', sidebarCollapsed && 'justify-center')
                                    }
                                    title={sidebarCollapsed ? item.label : undefined}
                                >
                                    <span className="flex-shrink-0">{item.icon}</span>
                                    {!sidebarCollapsed && <span className="animate-fade-in">{item.label}</span>}
                                </NavLink>
                            ))}
                        </div>
                    </div>
                ))}
            </nav>

            {/* User info + logout */}
            <div className="border-t border-slate-200 px-2 py-3">
                {!sidebarCollapsed && (
                    <div className="px-2.5 py-2 mb-2 rounded-xl bg-white border border-slate-200 shadow-sm animate-fade-in flex items-center gap-2">
                        <div className="w-7 h-7 rounded-full bg-primary-600 flex items-center justify-center text-white font-bold text-xs shadow-sm">
                            {offlinePilot?.name?.[0]?.toUpperCase() ?? '?'}
                        </div>
                        <div className="flex-1 min-w-0">
                            <p className="text-xs font-semibold text-slate-900 truncate">{offlinePilot?.name ?? 'Pilot'}</p>
                            <p className="text-[10px] text-amber-600 font-bold">Offline Session</p>
                        </div>
                    </div>
                )}
                <button
                    onClick={handleExit}
                    className={clsx('nav-item w-full text-red-500 hover:text-red-600 hover:bg-red-50', sidebarCollapsed && 'justify-center')}
                    title={sidebarCollapsed ? 'Exit Offline Mode' : undefined}
                >
                    <LogOut size={18} />
                    {!sidebarCollapsed && <span>Exit Offline Mode</span>}
                </button>
            </div>
        </aside>
    )
}

// 3. Clone of the online Header
function OfflineHeader() {
    const { toggleSidebar } = useUIStore()
    const { offlinePilot } = useModeStore()
    const location = useLocation()
    const title = routeTitles[location.pathname] ?? 'STAR Air ADM'
    const now = new Date().toLocaleString('en-US', { weekday: 'short', month: 'short', day: '2-digit', hour: '2-digit', minute: '2-digit', hour12: false })

    return (
        <header className="flex items-center gap-3 px-3 lg:px-5 py-2.5 bg-white border-b border-slate-200">
            {/* Mobile menu toggle */}
            <button onClick={toggleSidebar} className="btn-icon lg:hidden">
                <Menu size={20} />
            </button>

            {/* Page title */}
            <div>
                <h1 className="text-base font-bold text-black">{title}</h1>
                <p className="text-xs text-slate-600 font-mono hidden sm:block">{now}</p>
            </div>

            <div className="ml-auto flex items-center gap-3">
                {/* Offline indicator */}
                <div className="flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-amber-100 text-amber-700">
                    <WifiOff size={12} />
                    <span className="hidden sm:inline">Offline Mode</span>
                </div>

                {/* Notification bell (dummy for offline) */}
                <button className="btn-icon relative">
                    <Bell size={18} className="text-black opacity-50" />
                </button>

                {/* Avatar */}
                <div className="w-7 h-7 rounded-full bg-primary-600 border border-primary-500/40 flex items-center justify-center text-xs font-bold text-white" title={offlinePilot?.name}>
                    {offlinePilot?.name?.[0]?.toUpperCase() ?? 'U'}
                </div>
            </div>
        </header>
    )
}

export function OfflineLayout() {
    const { sidebarOpen, setSidebarOpen } = useUIStore()

    return (
        <div className="flex h-dvh overflow-hidden bg-cockpit-gradient">
            {/* Overlay for mobile sidebar */}
            {sidebarOpen && (
                <div
                    className="fixed inset-0 bg-black/50 z-20 lg:hidden"
                    onClick={() => setSidebarOpen(false)}
                />
            )}

            {/* Sidebar */}
            <div
                className={clsx(
                    'fixed lg:relative inset-y-0 left-0 z-30 lg:z-auto transition-transform duration-300',
                    sidebarOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'
                )}
            >
                <OfflineSidebar />
            </div>

            {/* Main content */}
            <div className="flex-1 flex flex-col overflow-hidden">
                <OfflineHeader />
                <main className="flex-1 overflow-y-auto overflow-x-hidden p-3 sm:p-4 lg:p-5">
                    {/* Ambient background glow (same as AppLayout) */}
                    <div className="fixed inset-0 pointer-events-none overflow-hidden -z-10">
                        <div className="absolute top-0 left-1/3 w-72 h-72 bg-primary-600/5 rounded-full blur-3xl" />
                        <div className="absolute bottom-0 right-1/4 w-72 h-72 bg-accent-cyan/5 rounded-full blur-3xl" />
                    </div>
                    <Outlet />
                </main>
            </div>
        </div>
    )
}
