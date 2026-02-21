export function isNowSameDayOrLater(compareWith: Date): boolean {
  const now = new Date();
  return now.getFullYear() === compareWith.getFullYear() &&
    now.getMonth() === compareWith.getMonth() &&
    now.getDate() >= compareWith.getDate();
}
