const allDates = ['2026-04-20'];
let baseStartDate = new Date(allDates[0]);
baseStartDate.setHours(0,0,0,0);
let baseEndDate = new Date('2026-04-23');
baseEndDate.setHours(0,0,0,0);
const minEndDate = new Date(baseStartDate);
minEndDate.setDate(baseStartDate.getDate() + 6);
if(baseEndDate < minEndDate) baseEndDate = minEndDate;

const diffTime = baseEndDate.getTime() - baseStartDate.getTime();
const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
const remainder = diffDays % 7;
if (remainder !== 0) baseEndDate.setDate(baseEndDate.getDate() + (7 - remainder));

console.log("baseStartDate:", baseStartDate);
console.log("baseEndDate:", baseEndDate);

const days = [];
let curr = new Date(baseStartDate);
while (curr <= baseEndDate) {
    days.push(curr.toString());
    curr.setDate(curr.getDate() + 1);
}
console.log("days:\n", days.join('\n'));

console.log("days.length:", days.length);

const pageOffset = 0
const DAYS_TO_SHOW = 7
const maxOffset = Math.max(0, Math.floor(days.length / DAYS_TO_SHOW) - 1)
const currentOffset = Math.min(pageOffset, maxOffset)

const visibleStartIdx = days.length - (currentOffset + 1) * DAYS_TO_SHOW
const visibleEndIdx = visibleStartIdx + DAYS_TO_SHOW - 1
const visibleDays = days.slice(visibleStartIdx, visibleEndIdx + 1)

console.log("visibleStartIdx:", visibleStartIdx, "visibleEndIdx:", visibleEndIdx);
