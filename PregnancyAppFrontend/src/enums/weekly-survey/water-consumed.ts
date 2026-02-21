export enum WaterConsumed {
  LessThanOneLitre = 0,
  OneToThreeLitres = 1,
  MoreThanThreeLitres = 2
}

export function waterConsumedStringify(value: WaterConsumed | null): string {
  switch (value) {
    case WaterConsumed.LessThanOneLitre:
      return 'Менее 1л';
    case WaterConsumed.OneToThreeLitres:
      return 'От 1 до 3л';
    case WaterConsumed.MoreThanThreeLitres:
      return 'Более 3л';
    default:
      return '';
  }
}
