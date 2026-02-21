export function formatLocalDateTime(utcDateInput: Date | string): string {
  if (!utcDateInput) return '';

  try {
    const utcDate = typeof utcDateInput === 'string' ? new Date(utcDateInput) : new Date(utcDateInput);

    if (isNaN(utcDate.getTime())) {
      console.error('Invalid date:', utcDateInput);
      return '';
    }

    const timezoneOffsetMs = -utcDate.getTimezoneOffset() * 60 * 1000;

    const localDate = new Date(utcDate.getTime() + timezoneOffsetMs);

    return localDate.toLocaleString('ru-RU', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false
    });
  } catch (error) {
    console.error('Error formatting date:', error);
    return String(utcDateInput);
  }
}
