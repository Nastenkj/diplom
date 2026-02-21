import {convertUtcToLocal} from "./convert-utc-to-local";

export function calculateSurveyTime(dateUtc: Date | null, hours: number): Date {
  if (dateUtc === null) {
    return new Date();
  }
  const localDate = convertUtcToLocal(dateUtc);
  const now = new Date();
  const diff = now.getTime() - localDate.getTime();
  const msDiff = hours * 60 * 60 * 1000;
  return diff > msDiff ? now : new Date(localDate.getTime() + msDiff);
}
