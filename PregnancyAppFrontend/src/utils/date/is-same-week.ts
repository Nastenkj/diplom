export function isSameWeek(date1: Date, date2: Date): boolean {
  const diffInMs = Math.abs(date1.getTime() - date2.getTime());
  const diffInDays = diffInMs / (1000 * 60 * 60 * 24);
  return diffInDays <= 7;
}
