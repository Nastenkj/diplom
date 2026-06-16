import {MaskitoOptions} from "@maskito/core";

export const systolicPressureMask: MaskitoOptions = {
  // систолическое: 50..250, без дробей
  mask: /^[0-9]{0,3}$/,
};

export const diastolicPressureMask: MaskitoOptions = {
  // диастолическое: 30..150, без дробей
  mask: /^[0-9]{0,3}$/,
};
