export function convertUtcToLocal(dateUtc: Date): Date {
  const utcDate = dateUtc instanceof Date ? dateUtc : new Date(dateUtc);
  return new Date(utcDate.getTime() - utcDate.getTimezoneOffset() * 60000);
}
