using FluentValidation;
using PregnancyAppBackend.Dtos.Web.DailySurvey;

namespace PregnancyAppBackend.Validators;

public class DailySurveyDtoValidator : AbstractValidator<DailySurveyDto>
{
    public DailySurveyDtoValidator()
    {
        RuleFor(x => x.Temperature)
           .IsInEnum().WithMessage("Поле 'Температура' имеет некорректное значение.");

        RuleFor(x => x.HeartRate)
           .NotEmpty().WithMessage("Поле 'Частота сердечных сокращений' обязательно для заполнения.")
           .GreaterThan(0).WithMessage("Частота сердечных сокращений должна быть больше 0.");

        RuleFor(x => x.AdditionalInformation)
           .MaximumLength(2000).WithMessage("Дополнительная информация не должна превышать 2000 символов.");
    }
}