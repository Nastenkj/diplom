using FluentValidation;
using PregnancyAppBackend.Dtos.Web.CommunicationLinks;

namespace PregnancyAppBackend.Validators;

public class CommunicationLinkDtoValidator : AbstractValidator<PatientDoctorCommunicationLinkDto>
{
   public CommunicationLinkDtoValidator()
   {
      RuleFor(x => x.UserId)
        .NotEmpty().WithMessage("Поле 'UserId' обязательно для заполнения.");

      RuleFor(x => x.UserName)
        .NotEmpty().WithMessage("Поле 'UserName' обязательно для заполнения.")
        .MaximumLength(100).WithMessage("Имя пользователя не должно превышать 100 символов.");

      RuleFor(x => x.DoctorId)
        .NotEmpty().WithMessage("Поле 'DoctorId' обязательно для заполнения.");

      RuleFor(x => x.DoctorName)
        .NotEmpty().WithMessage("Поле 'DoctorName' обязательно для заполнения.")
        .MaximumLength(100).WithMessage("Имя доктора не должно превышать 100 символов.");

      RuleFor(x => x.CommunicationLink)
        .NotEmpty().WithMessage("Поле 'CommunicationLink' обязательно для заполнения.")
        .MaximumLength(100).WithMessage("Ссылка не должна превышать 100 символов.");
      
      RuleFor(x => x.MeetingScheduledAtUtc)
       .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Время встречи должно быть в будущем.");
   }
}