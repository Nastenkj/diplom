export enum Stool {
  Daily = 0,
  OncePerTwoThreeDays = 1,
  MoreRarely = 2,
}

export function stoolStringify(value: Stool | null): string {
  switch (value) {
    case Stool.Daily:
      return 'Ежедневно';
    case Stool.OncePerTwoThreeDays:
      return 'Раз в 2-3 дня';
    case Stool.MoreRarely:
      return 'Реже';
    default:
      return '';
  }
}
