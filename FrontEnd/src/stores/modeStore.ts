// src/stores/modeStore.ts — Online / Offline mode state
import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export interface OfflinePilot {
    id: string
    name: string
}

interface ModeState {
    isOffline: boolean
    offlinePilot: OfflinePilot | null
    enterOfflineMode: (pilot: OfflinePilot) => void
    exitOfflineMode: () => void
}

export const useModeStore = create<ModeState>()(
    persist(
        (set) => ({
            isOffline: false,
            offlinePilot: null,

            enterOfflineMode: (pilot) =>
                set({ isOffline: true, offlinePilot: pilot }),

            exitOfflineMode: () =>
                set({ isOffline: false, offlinePilot: null }),
        }),
        {
            name: 'star-air-mode',
            partialize: (state) => ({
                isOffline: state.isOffline,
                offlinePilot: state.offlinePilot,
            }),
        }
    )
)

/** Detect whether we're running inside a desktop WebView host */
export const isElectron = (): boolean =>
    typeof window !== 'undefined' &&
    (
        'electronAPI' in window ||
        !!(window as any).chrome?.webview
    )
