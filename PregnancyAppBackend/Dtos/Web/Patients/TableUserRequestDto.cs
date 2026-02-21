namespace PregnancyAppBackend.Dtos.Web.Patients;

public class TableUserRequestDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}