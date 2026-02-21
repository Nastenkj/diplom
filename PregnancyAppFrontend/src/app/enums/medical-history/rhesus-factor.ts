export enum RhesusFactor {
  Positive = 1,
  Negative = 2,
}

export function rhesusFactorStringify(value: RhesusFactor | null): string {
  switch (value) {
    case RhesusFactor.Positive:
      return 'Положительный';
    case RhesusFactor.Negative:
      return 'Отрицательный';
    default:
      return '';
  }
}
