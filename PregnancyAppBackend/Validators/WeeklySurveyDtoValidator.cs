using FluentValidation;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;

namespace PregnancyAppBackend.Validators;

public class WeeklySurveyDtoValidator : AbstractValidator<WeeklySurveyDto>
{
    public WeeklySurveyDtoValidator()
    {
        RuleFor(x => x.UnordinaryBloodPressure)
           .IsInEnum().WithMessage("Неверный выбор артериального давления.");

        When(x => x.HasOrvi, () =>
        {
            RuleFor(x => x.OrviSymptoms)
               .NotEmpty().WithMessage("Укажите симптомы ОРВИ.");
        });

        When(x => x.HasUnordinaryTemp, () =>
        {
            RuleFor(x => x.UnordinaryTempOccurrences)
               .NotNull().WithMessage("Укажите количество случаев повышения температуры.")
               .GreaterThan(0).WithMessage("Количество должно быть больше 0.");
        });

        When(x => x.HasGynecologicalSymptoms, () =>
        {
            RuleFor(x => x.GynecologicalSymptoms)
               .NotEmpty().WithMessage("Укажите гинекологические симптомы.");
        });

        When(x => x.HasSwelling, () =>
        {
            RuleFor(x => x.SwellingDescription)
               .NotEmpty().WithMessage("Укажите места отёков.");
        });

        RuleFor(x => x.WaterConsumed)
           .IsInEnum().WithMessage("Неверный выбор количества жидкости.");

        RuleFor(x => x.Stool)
           .IsInEnum().WithMessage("Неверный выбор регулярности стула.");

        RuleFor(x => x.Urination)
           .IsInEnum().WithMessage("Неверный выбор мочеиспускания.");

        RuleFor(x => x.WeightAdded)
           .GreaterThanOrEqualTo(0).WithMessage("Прибавка веса не может быть отрицательной.");
    }
}