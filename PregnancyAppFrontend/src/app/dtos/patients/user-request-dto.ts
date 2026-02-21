export interface UserRequestDto {
  pageNumber: number;
  pageSize: number;
  phoneNumber?: string;
  name?: string;
  email?: string;
}
