export enum Urination {
  Hurtful = 0,
  Unhurtful = 1,
}

export function urinationStringify(value: Urination | null): string {
  switch (value) {
    case Urination.Hurtful:
      return 'Болезненное';
    case Urination.Unhurtful:
      return 'Неболезненное';
    default:
      return '';
  }
}
