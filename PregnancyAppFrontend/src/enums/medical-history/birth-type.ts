export enum BirthType {
  Independent = 1,
  CSection = 2,
  FirstPregnancy = 3,
}

export function birthTypeStringify(value: BirthType | null): string {
  switch (value) {
    case BirthType.Independent:
      return 'Самостоятельные';
    case BirthType.CSection:
      return 'Кесерово сечение';
    case BirthType.FirstPregnancy:
      return 'Первая беременность';
    default:
      return '';
  }
}

