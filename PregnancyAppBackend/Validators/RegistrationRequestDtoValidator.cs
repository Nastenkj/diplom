using FluentValidation;
using PregnancyAppBackend.Dtos.Authentication;

namespace PregnancyAppBackend.Validators;

public class RegistrationRequestDtoValidator : AbstractValidator<PatientRegistrationRequestDto>
{
    public RegistrationRequestDtoValidator()
    {
        RuleFor(x => x.FullName)
           .NotEmpty().WithMessage("Поле ФИО обязательно для заполнения.");

        RuleFor(x => x.PhoneNumber)
           .NotEmpty().WithMessage("Поле номера телефона обязательно для заполнения.");

        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Поле электронной почты обязательно для заполнения.");

        RuleFor(x => x.TrustedPersonFullName)
           .NotEmpty().WithMessage("Поле ФИО доверенного лица обязательно для заполнения.");

        RuleFor(x => x.TrustedPersonPhoneNumber)
           .NotEmpty().WithMessage("Поле номера телефона доверенного лица обязательно для заполнения.");

        RuleFor(x => x.TrustedPersonEmail)
           .NotEmpty().WithMessage("Поле электронной почты доверенного лица обязательно для заполнения.");

        RuleFor(x => x.InsuranceNumber)
           .NotEmpty().WithMessage("Поле номера полиса обязательно для заполнения.");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Поле пароля обязательно для заполнения.");
    }
}