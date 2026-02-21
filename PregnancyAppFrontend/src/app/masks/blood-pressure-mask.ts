import {MaskitoOptions} from "@maskito/core";

export const bloodPressureMask: MaskitoOptions = {
  mask: [/\d/, /\d/, /\d?/, '/', /\d/, /\d/, /\d?/],
};
