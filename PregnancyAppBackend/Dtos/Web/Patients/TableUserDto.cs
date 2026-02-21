namespace PregnancyAppBackend.Dtos.Web.Patients;

public class TableUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
}

public class TableUsersDto
{
    public int Total { get; set; }
    public List<TableUserDto> TableUsers { get; set; }
}