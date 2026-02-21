export enum BloodGroup {
  I = 1,
  II = 2,
  III = 3,
  IV = 4,
}

export function bloodGroupStringify(value: BloodGroup | null): string {
  switch (value) {
    case BloodGroup.I:
      return 'I';
    case BloodGroup.II:
      return 'II';
    case BloodGroup.III:
      return 'III';
    case BloodGroup.IV:
      return 'IV';
    default:
      return '';
  }
}
