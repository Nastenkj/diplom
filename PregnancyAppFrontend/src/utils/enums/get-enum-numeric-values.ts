type AnyEnum = Record<string | number, string | number>;

export function getEnumNumericValues<T extends AnyEnum>(enumObj: T): (T[keyof T])[] {
  return Object.values(enumObj).filter(value => typeof value === 'number') as (T[keyof T])[];
}
