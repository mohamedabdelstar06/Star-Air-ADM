const getLocalDate = (dateString) => {
    const d = new Date(dateString)
    if (isNaN(d.getTime())) return ''
    const year = d.getFullYear()
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const day = String(d.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
}

const allDates = ['2026-04-20', '2026-04-21', '2026-04-22', '2026-04-23', '2026-04-24', '2026-04-25', '2026-04-26']

let baseStartDate = new Date()
if (allDates.length > 0) {
    baseStartDate = new Date(allDates[0])
}
baseStartDate.setHours(0, 0, 0, 0)

let baseEndDate = new Date()
baseEndDate.setHours(0, 0, 0, 0)

const minEndDate = new Date(baseStartDate)
minEndDate.setDate(baseStartDate.getDate() + 6)

if (baseEndDate < minEndDate) {
    baseEndDate = minEndDate
}

const diffTime = Math.abs(baseEndDate.getTime() - baseStartDate.getTime())
const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1
const remainder = diffDays % 7
if (remainder !== 0) {
    baseEndDate.setDate(baseEndDate.getDate() + (7 - remainder))
}

const days = []
let curr = new Date(baseStartDate)
while (curr <= baseEndDate) {
    const dateStr = getLocalDate(curr)
    if (dateStr) {
        days.push(dateStr)
    }
    curr.setDate(curr.getDate() + 1)
}

const pageOffset = 0
const DAYS_TO_SHOW = 7
const maxOffset = Math.max(0, Math.floor(days.length / DAYS_TO_SHOW) - 1)
const currentOffset = Math.min(pageOffset, maxOffset)

const visibleStartIdx = days.length - (currentOffset + 1) * DAYS_TO_SHOW
const visibleEndIdx = visibleStartIdx + DAYS_TO_SHOW - 1
const visibleDays = days.slice(visibleStartIdx, visibleEndIdx + 1)

console.log("days.length:", days.length)
console.log("visibleDays:", visibleDays)
