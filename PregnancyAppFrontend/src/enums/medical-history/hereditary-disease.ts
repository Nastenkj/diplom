export enum HereditaryDisease {
  None = 0,
  Diabetes = 1,
  Oncology = 2,
  StrokeHeartAttack = 4,
}

export function hereditaryDiseaseStringify(value: HereditaryDisease | null): string {
  switch (value) {
    case HereditaryDisease.None:
      return 'Не отягощена';
    case HereditaryDisease.Diabetes:
      return 'Сахарный диабет';
    case HereditaryDisease.Oncology:
      return 'Онкология';
    case HereditaryDisease.StrokeHeartAttack:
      return 'Инсульт/инфаркт';
    default:
      return '';
  }
}

