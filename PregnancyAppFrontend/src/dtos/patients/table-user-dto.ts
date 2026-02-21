export interface TableUserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  birthDate: string;  // DateOnly is represented as string in ISO format (YYYY-MM-DD)
}

export interface TableUsersDto {
  total: number;
  tableUsers: TableUserDto[];
}
