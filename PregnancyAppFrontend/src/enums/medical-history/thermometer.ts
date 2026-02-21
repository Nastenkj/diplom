export enum Thermometer {
  Mercury = 1,
  Electronic = 2,
}

export function thermometerStringify(value: Thermometer | null): string {
  switch (value) {
    case Thermometer.Mercury:
      return 'Ртутный';
    case Thermometer.Electronic:
      return 'Электронный';
    default:
      return '';
  }
}

