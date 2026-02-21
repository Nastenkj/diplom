import {TableUserDto} from "./table-user-dto";

export interface UserDto extends TableUserDto {
  trustedPersonFullName: string;
  trustedPersonPhoneNumber: string;
  trustedPersonEmail: string;
  insuranceNumber: string;
}
