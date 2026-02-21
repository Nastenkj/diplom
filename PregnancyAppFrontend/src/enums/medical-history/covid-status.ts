export enum CovidStatus {
  No = 1,
  YesBeforePregnancy = 2,
  YesDuringPregnancy = 3,
}

export function covidStatusStringify(value: CovidStatus | null): string {
  switch (value) {
    case CovidStatus.No:
      return 'Нет';
    case CovidStatus.YesBeforePregnancy:
      return 'Да, до беременности';
    case CovidStatus.YesDuringPregnancy:
      return 'Да, во время беременности';
    default:
      return '';
  }
}
