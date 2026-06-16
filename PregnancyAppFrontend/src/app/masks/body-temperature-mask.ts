import {MaskitoOptions} from "@maskito/core";

export const bodyTemperatureMask: MaskitoOptions = {
  // Разрешаем целую и дробную часть 1..2 знака: 36.6 / 36 / 36.12
  // На случай ввода запятой: 36,6
  mask: /^\d{0,2}(?:[.,]\d{0,2})?$/,
};
