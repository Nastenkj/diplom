import {MaskitoOptions} from "@maskito/core";

export const numberMask: MaskitoOptions = {
  mask: /^\d*$/,
};

export const decimalMask: MaskitoOptions = {
  mask: /^\d+\.?\d*$/,
};
