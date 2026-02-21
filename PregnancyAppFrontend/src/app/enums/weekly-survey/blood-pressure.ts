export enum BloodPressure {
  Normal = 0,
  High = 1,
  Low = 2,
}

export function bloodPressureStringify(value: BloodPressure | null): string {
  switch (value) {
    case BloodPressure.Normal:
      return 'Не изменялось';
    case BloodPressure.High:
      return 'Повышалось';
    case BloodPressure.Low:
      return 'Понижалось';
    default:
      return '';
  }
}
