export interface RegistrationRequestDto extends BaseRegistrationRequestDto {
  trustedPersonFullName: string;
  trustedPersonPhoneNumber: string;
  trustedPersonEmail: string;
  insuranceNumber: string;
}

export interface BaseRegistrationRequestDto {
  fullName: string;
  birthDate: string;
  phoneNumber: string;
  email: string;
  password: string;
}
