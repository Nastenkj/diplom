export enum Temperature {
  Lower37And2 = 0,
  Between37And2And37And5 = 1,
  Higher37And5 = 2,
}

export const mapTemperature = (temp: number): Temperature =>
  temp < 37.2 ? Temperature.Lower37And2 : temp <= 37.5 ? Temperature.Between37And2And37And5 : Temperature.Higher37And5;

export function temperatureStringify(value: Temperature | null): string {
  switch (value) {
    case Temperature.Lower37And2:
      return 'до 37.2';
    case Temperature.Between37And2And37And5:
      return '37.2 - 37.5';
    case Temperature.Higher37And5:
      return 'более 37.5';
    default:
      return '';
  }
}
