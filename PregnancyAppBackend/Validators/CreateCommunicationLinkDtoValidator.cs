using FluentValidation;
using PregnancyAppBackend.Dtos.Web.CommunicationLinks;

namespace PregnancyAppBackend.Validators;

public class CreateCommunicationLinkDtoValidator : AbstractValidator<CreateCommunicationLinkDto>
{
    public CreateCommunicationLinkDtoValidator()
    {
        RuleFor(x => x.PatientId)
           .NotEmpty().WithMessage("Поле 'PatientId' обязательно для заполнения.");
        
        RuleFor(x => x.MeetingScheduledAtUtc)
           .NotEmpty().WithMessage("Необходимо указать дату и время встречи.")
           .GreaterThan(DateTime.UtcNow).WithMessage("Дата и время встречи должны быть в будущем.");
        
        RuleFor(x => x.CustomLink)
           .MaximumLength(500).WithMessage("Ссылка не должна превышать 500 символов.");
    }
}