import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

export function bloodPressureValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const pattern = /^\d{2,3}\/\d{2,3}$/
    if (control.value && !pattern.test(control.value)) {
      return { bloodPressureInvalid: true }
    }
    return null
  }
}
