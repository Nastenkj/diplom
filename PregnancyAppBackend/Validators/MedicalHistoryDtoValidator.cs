using FluentValidation;
using PregnancyAppBackend.Dtos.Web.MedicalHistory;

namespace PregnancyAppBackend.Validators;

public class MedicalHistoryDtoValidator : AbstractValidator<MedicalHistoryDto>
{
    public MedicalHistoryDtoValidator()
    {
        RuleFor(x => x.Weight)
           .GreaterThan(0).WithMessage("Вес должен быть больше 0.");

        RuleFor(x => x.Height)
           .GreaterThan(0).WithMessage("Рост должен быть больше 0.");

        RuleFor(x => x.BloodGroup)
           .IsInEnum().WithMessage("Неверная группа крови.");

        RuleFor(x => x.RhesusFactor)
           .IsInEnum().WithMessage("Неверный резус-фактор.");

        RuleFor(x => x.BloodPressure)
           .NotEmpty().WithMessage("Артериальное давление обязательно для заполнения.")
           .Matches(@"^\d{2,3}\/\d{2,3}$").WithMessage("Артериальное давление должно иметь формат '120/80'.");

        RuleFor(x => x.Thermometer)
           .IsInEnum().WithMessage("Неверный тип термометра.");

        RuleFor(x => x.PregnancyAmount)
           .GreaterThanOrEqualTo(0).WithMessage("Количество беременностей не может быть отрицательным.");

        When(x => x.PregnancyAmount >= 1, () =>
        {
            RuleFor(x => x.AbortionAmount)
               .NotNull().WithMessage("Количество абортов обязательно, если количество беременностей не менее 1.");

            RuleFor(x => x.MiscarriageAmount)
               .NotNull().WithMessage("Количество выкидышей обязательно, если количество беременностей не менее 1.");

            RuleFor(x => x.PrematureBirthAmount)
               .NotNull().WithMessage("Количество преждевременных родов обязательно, если количество беременностей не менее 1.");
        });

        RuleFor(x => x.PreviousBirthType)
           .IsInEnum().WithMessage("Неверный тип предыдущих родов.");

        RuleFor(x => x.HereditaryDiseases).ForEach(hd => hd.IsInEnum().WithMessage("Неверный тип наследственного заболевания."));

        RuleFor(x => x.IsSmoking)
           .NotNull().WithMessage("Укажите, курите ли вы.");

        RuleFor(x => x.IsConsumingAlcohol)
           .NotNull().WithMessage("Укажите, употребляете ли вы алкоголь.");

        RuleFor(x => x.EnduredCovid)
           .IsInEnum().WithMessage("Неверный статус Covid.");
    }
}