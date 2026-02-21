using ClosedXML.Excel;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Enums.DailySurvey;
using PregnancyAppBackend.Enums.MedicalHistory;
using PregnancyAppBackend.Enums.WeeklySurvey;
using PregnancyAppBackend.Exceptions;
using PregnancyAppBackend.Services.DailySurveysService;
using PregnancyAppBackend.Services.MedicalHistoriesService;
using PregnancyAppBackend.Services.StatisticsService;
using PregnancyAppBackend.Services.WeeklySurveyService;

namespace PregnancyAppBackend.Services.Excel;

public class ExcelExportService : IExcelExportService
{
    private readonly IDailySurveysService _dailySurveysService;
    private readonly IWeeklySurveysService _weeklySurveysService;
    private readonly IMedicalHistoriesService _medicalHistoriesService;
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<ExcelExportService> _logger;

    // Словари для русских текстов
    private static readonly Dictionary<Temperature, string> TemperatureText = new()
    {
        { Temperature.Lower32And2, "Ниже 37.2" },
        { Temperature.Between37And2And37And5, "Между 37.2 и 37.5" },
        { Temperature.Higher37And5, "Выше 37.5" }
    };

    private static readonly Dictionary<BirthType, string> BirthTypeText = new()
    {
        { BirthType.Independent, "Самостоятельные" },
        { BirthType.CSection, "Кесарево сечение" },
        { BirthType.FirstPregnancy, "Первая беременность" }
    };

    private static readonly Dictionary<BloodGroup, string> BloodGroupText = new()
    {
        { BloodGroup.I, "I" },
        { BloodGroup.II, "II" },
        { BloodGroup.III, "III" },
        { BloodGroup.IV, "IV" }
    };

    private static readonly Dictionary<CovidStatus, string> CovidStatusText = new()
    {
        { CovidStatus.No, "Нет" },
        { CovidStatus.YesBeforePregnancy, "Да, до беременности" },
        { CovidStatus.YesDuringPregnancy, "Да, во время беременности" }
    };

    private static readonly Dictionary<HereditaryDisease, string> HereditaryDiseaseText = new()
    {
        { HereditaryDisease.None, "Нет" },
        { HereditaryDisease.Diabetes, "Диабет" },
        { HereditaryDisease.Oncology, "Онкология" },
        { HereditaryDisease.StrokeHeartAttack, "Инсульт/Инфаркт" }
    };

    private static readonly Dictionary<RhesusFactor, string> RhesusFactorText = new()
    {
        { RhesusFactor.Positive, "Положительный" },
        { RhesusFactor.Negative, "Отрицательный" }
    };

    private static readonly Dictionary<Thermometer, string> ThermometerText = new()
    {
        { Thermometer.Mercury, "Ртутный" },
        { Thermometer.Electronic, "Электронный" }
    };

    private static readonly Dictionary<BloodPressure, string> BloodPressureText = new()
    {
        { BloodPressure.Normal, "Нормальное" },
        { BloodPressure.High, "Высокое" },
        { BloodPressure.Low, "Низкое" }
    };

    private static readonly Dictionary<Stool, string> StoolText = new()
    {
        { Stool.Daily, "Ежедневно" },
        { Stool.OncePerTwoThreeDays, "Раз в 2-3 дня" },
        { Stool.MoreRarely, "Реже" }
    };

    private static readonly Dictionary<Urination, string> UrinationText = new()
    {
        { Urination.Hurtful, "Болезненное" },
        { Urination.Unhurtful, "Безболезненное" }
    };

    private static readonly Dictionary<WaterConsumed, string> WaterConsumedText = new()
    {
        { WaterConsumed.LessThanOneLitre, "Менее 1 литра" },
        { WaterConsumed.OneToThreeLitres, "1-3 литра" },
        { WaterConsumed.MoreThanThreeLitres, "Более 3 литров" }
    };

    public ExcelExportService(
        IDailySurveysService dailySurveysService,
        IWeeklySurveysService weeklySurveysService,
        IMedicalHistoriesService medicalHistoriesService,
        IStatisticsService statisticsService,
        ILogger<ExcelExportService> logger)
    {
        _dailySurveysService = dailySurveysService;
        _weeklySurveysService = weeklySurveysService;
        _medicalHistoriesService = medicalHistoriesService;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    public async Task<byte[]> ExportDailySurveyToExcelAsync(Guid surveyId)
    {
        var survey = await _dailySurveysService.GetDailySurveyById(surveyId);
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ежедневный опрос");
        
        AddDailySurveyHeaders(worksheet);
        AddDailySurveyData(worksheet, survey, 2);
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportWeeklySurveyToExcelAsync(Guid surveyId)
    {
        var survey = await _weeklySurveysService.GetWeeklySurveyById(surveyId);
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Еженедельный опрос");
        
        AddWeeklySurveyHeaders(worksheet);
        AddWeeklySurveyData(worksheet, survey, 2);
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportAllDailySurveysToExcelAsync(Guid userId)
    {
        var surveys = await _dailySurveysService.GetDailySurveysForUser(userId);
        
        _logger.LogInformation("Exporting daily excel info for userId={userId}", userId);
        
        if (!surveys.Any())
        {
            throw new ApiException($"Surveys not found for userId={userId}", "Ежедневные опросы не найдены для пользователя");
        }
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ежедневные опросы");
        
        var sortedSurveys = surveys.OrderByDescending(s => s.CreationDateUtc).ToList();
        
        AddDailySurveyHeaders(worksheet);
        
        int row = 2;
        foreach (var survey in sortedSurveys)
        {
            AddDailySurveyData(worksheet, survey, row);
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportAllWeeklySurveysToExcelAsync(Guid userId)
    {
        var surveys = await _weeklySurveysService.GetWeeklySurveysForUser(userId);
        
        _logger.LogInformation("Exporting weekly excel info for userId={userId}", userId);
        
        if (!surveys.Any())
        {
            throw new ApiException($"Surveys not found for userId={userId}", "Еженедельные опросы не найдены для пользователя");
        }
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Еженедельные опросы");
        
        var sortedSurveys = surveys.OrderByDescending(s => s.CreationDateUtc).ToList();
        
        AddWeeklySurveyHeaders(worksheet);
        
        int row = 2;
        foreach (var survey in sortedSurveys)
        {
            AddWeeklySurveyData(worksheet, survey, row);
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportMedicalHistoryToExcelAsync(Guid userId)
    {
        var medicalHistory = await _medicalHistoriesService.GetMedicalHistoryAsync(userId);
        
        if (medicalHistory == null)
        {
            throw new ApiException($"Medical histories not found for userId={userId}", "Анамнез не найден для пользователя");
        }
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Медицинская история");
        
        worksheet.Cell(1, 1).Value = "Показатель";
        worksheet.Cell(1, 2).Value = "Значение";
        
        int row = 2;
        
        worksheet.Cell(row, 1).Value = "Вес (кг)";
        worksheet.Cell(row++, 2).Value = medicalHistory.Weight;
        
        worksheet.Cell(row, 1).Value = "Рост (см)";
        worksheet.Cell(row++, 2).Value = medicalHistory.Height;
        
        worksheet.Cell(row, 1).Value = "Группа крови";
        worksheet.Cell(row++, 2).Value = BloodGroupText[medicalHistory.BloodGroup];
        
        worksheet.Cell(row, 1).Value = "Резус-фактор";
        worksheet.Cell(row++, 2).Value = RhesusFactorText[medicalHistory.RhesusFactor];
        
        worksheet.Cell(row, 1).Value = "Кровяное давление";
        worksheet.Cell(row++, 2).Value = medicalHistory.BloodPressure;
        
        worksheet.Cell(row, 1).Value = "Термометр";
        worksheet.Cell(row++, 2).Value = ThermometerText[medicalHistory.Thermometer];
        
        worksheet.Cell(row, 1).Value = "Количество беременностей";
        worksheet.Cell(row++, 2).Value = medicalHistory.PregnancyAmount;
        
        worksheet.Cell(row, 1).Value = "Количество абортов";
        worksheet.Cell(row++, 2).Value = medicalHistory.AbortionAmount;
        
        worksheet.Cell(row, 1).Value = "Количество выкидышей";
        worksheet.Cell(row++, 2).Value = medicalHistory.MiscarriageAmount;
        
        worksheet.Cell(row, 1).Value = "Количество преждевременных родов";
        worksheet.Cell(row++, 2).Value = medicalHistory.PrematureBirthAmount;
        
        worksheet.Cell(row, 1).Value = "Тип предыдущих родов";
        worksheet.Cell(row++, 2).Value = BirthTypeText[medicalHistory.PreviousBirthType];
        
        worksheet.Cell(row, 1).Value = "Гинекологические заболевания";
        worksheet.Cell(row++, 2).Value = medicalHistory.GynecologicalDiseases ?? "Нет";
        
        worksheet.Cell(row, 1).Value = "Соматические заболевания";
        worksheet.Cell(row++, 2).Value = medicalHistory.SomaticDiseases ?? "Нет";
        
        worksheet.Cell(row, 1).Value = "Перенесенные операции";
        worksheet.Cell(row++, 2).Value = medicalHistory.UndergoneOperations ?? "Нет";
        
        worksheet.Cell(row, 1).Value = "Аллергические реакции";
        worksheet.Cell(row++, 2).Value = medicalHistory.AllergicReactions ?? "Нет";
        
        worksheet.Cell(row, 1).Value = "Наследственные заболевания";
        if (medicalHistory.HereditaryDiseases.Count == 0 || medicalHistory.HereditaryDiseases.Contains(HereditaryDisease.None))
        {
            worksheet.Cell(row++, 2).Value = "Нет";
        }
        else
        {
            var diseases = medicalHistory.HereditaryDiseases
                .Where(d => d != HereditaryDisease.None)
                .Select(d => HereditaryDiseaseText[d]);
            worksheet.Cell(row++, 2).Value = string.Join(", ", diseases);
        }
        
        worksheet.Cell(row, 1).Value = "Курение";
        worksheet.Cell(row++, 2).Value = medicalHistory.IsSmoking ? "Да" : "Нет";
        
        worksheet.Cell(row, 1).Value = "Употребление алкоголя";
        worksheet.Cell(row++, 2).Value = medicalHistory.IsConsumingAlcohol ? "Да" : "Нет";
        
        worksheet.Cell(row, 1).Value = "Перенесенный COVID";
        worksheet.Cell(row++, 2).Value = CovidStatusText[medicalHistory.EnduredCovid];
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // TODO: эти строки должны быть в ресурсах и на фронт тоже приходить с бека (для других страниц)
    private void AddDailySurveyHeaders(IXLWorksheet worksheet)
    {
        int col = 1;
        worksheet.Cell(1, col++).Value = "Дата";
        worksheet.Cell(1, col++).Value = "Боли в животе";
        worksheet.Cell(1, col++).Value = "Кровянистые выделения";
        worksheet.Cell(1, col++).Value = "Тошнота";
        worksheet.Cell(1, col++).Value = "Позывы к рвоте";
        worksheet.Cell(1, col++).Value = "Температура";
        worksheet.Cell(1, col++).Value = "Систолическое давление";
        worksheet.Cell(1, col++).Value = "Диастолическое давление";
        worksheet.Cell(1, col++).Value = "Пульс";
        worksheet.Cell(1, col++).Value = "Уровень глюкозы";
        worksheet.Cell(1, col++).Value = "Уровень гемоглобина";
        worksheet.Cell(1, col++).Value = "Сатурация";
        worksheet.Cell(1, col++).Value = "Уро";
        worksheet.Cell(1, col++).Value = "Кровь";
        worksheet.Cell(1, col++).Value = "Билирубин";
        worksheet.Cell(1, col++).Value = "Кетоны";
        worksheet.Cell(1, col++).Value = "Лейкоциты";
        worksheet.Cell(1, col++).Value = "Глюкоза";
        worksheet.Cell(1, col++).Value = "Белок";
        worksheet.Cell(1, col++).Value = "pH";
        worksheet.Cell(1, col++).Value = "Нитриты";
        worksheet.Cell(1, col++).Value = "Удельный вес";
        worksheet.Cell(1, col++).Value = "Витамин C";
        worksheet.Cell(1, col++).Value = "ПТ";
        worksheet.Cell(1, col++).Value = "АЧТВ";
        worksheet.Cell(1, col++).Value = "МНО";
        worksheet.Cell(1, col++).Value = "Свертываемость крови";
        worksheet.Cell(1, col++).Value = "Уровень кислорода";
        worksheet.Cell(1, col++).Value = "Дополнительная информация";
    }

    private void AddDailySurveyData(IXLWorksheet worksheet, DailySurveyDto survey, int row)
    {
        int col = 1;
        var date = survey.CreationDateUtc;
        worksheet.Cell(row, col++).Value = string.Format("{0:dd.MM.yyyy HH:mm}", date);
        worksheet.Cell(row, col++).Value = survey.AbdomenHurts ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.BloodDischarge ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.Nausea ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.UrgeToPuke;
        worksheet.Cell(row, col++).Value = TemperatureText[survey.Temperature];
        worksheet.Cell(row, col++).Value = survey.SystolicPressure;
        worksheet.Cell(row, col++).Value = survey.DiastolicPressure;
        worksheet.Cell(row, col++).Value = survey.HeartRate;
        worksheet.Cell(row, col++).Value = survey.GlucoseLevel;
        worksheet.Cell(row, col++).Value = survey.HemoglobinLevel;
        worksheet.Cell(row, col++).Value = survey.Saturation;
        worksheet.Cell(row, col++).Value = survey.Uro;
        worksheet.Cell(row, col++).Value = survey.Bld;
        worksheet.Cell(row, col++).Value = survey.Bil;
        worksheet.Cell(row, col++).Value = survey.Ket;
        worksheet.Cell(row, col++).Value = survey.Leu;
        worksheet.Cell(row, col++).Value = survey.Glu;
        worksheet.Cell(row, col++).Value = survey.Pro;
        worksheet.Cell(row, col++).Value = survey.Ph;
        worksheet.Cell(row, col++).Value = survey.Nit;
        worksheet.Cell(row, col++).Value = survey.Sg;
        worksheet.Cell(row, col++).Value = survey.Vc;
        worksheet.Cell(row, col++).Value = survey.Pt;
        worksheet.Cell(row, col++).Value = survey.Aptt;
        worksheet.Cell(row, col++).Value = survey.Inr;
        worksheet.Cell(row, col++).Value = survey.OxygenLevel;
        worksheet.Cell(row, col++).Value = survey.AdditionalInformation ?? "Нет";
    }

    private void AddWeeklySurveyHeaders(IXLWorksheet worksheet)
    {
        int col = 1;
        worksheet.Cell(1, col++).Value = "Дата";
        worksheet.Cell(1, col++).Value = "Неделя беременности";
        worksheet.Cell(1, col++).Value = "Прибавка в весе";
        worksheet.Cell(1, col++).Value = "Наличие ОРВИ";
        worksheet.Cell(1, col++).Value = "Симптомы ОРВИ";
        worksheet.Cell(1, col++).Value = "Необычная температура";
        worksheet.Cell(1, col++).Value = "Случаи необычной температуры";
        worksheet.Cell(1, col++).Value = "Необычное кровяное давление";
        worksheet.Cell(1, col++).Value = "Гинекологические симптомы";
        worksheet.Cell(1, col++).Value = "Описание гинекологических симптомов";
        worksheet.Cell(1, col++).Value = "Необычная моча";
        worksheet.Cell(1, col++).Value = "Наличие отеков";
        worksheet.Cell(1, col++).Value = "Описание отеков";
        worksheet.Cell(1, col++).Value = "Потребление воды";
        worksheet.Cell(1, col++).Value = "Стул";
        worksheet.Cell(1, col++).Value = "Мочеиспускание";
    }

    private void AddWeeklySurveyData(IXLWorksheet worksheet, WeeklySurveyDto survey, int row)
    {
        int col = 1;
        var date = survey.CreationDateUtc;
        worksheet.Cell(row, col++).Value = string.Format("{0:dd.MM.yyyy HH:mm}", date);
        worksheet.Cell(row, col++).Value = survey.PregnancyWeek;
        worksheet.Cell(row, col++).Value = survey.WeightAdded;
        worksheet.Cell(row, col++).Value = survey.HasOrvi ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.OrviSymptoms ?? "Нет";
        worksheet.Cell(row, col++).Value = survey.HasUnordinaryTemp ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.UnordinaryTempOccurrences;
        worksheet.Cell(row, col++).Value = BloodPressureText[survey.UnordinaryBloodPressure];
        worksheet.Cell(row, col++).Value = survey.HasGynecologicalSymptoms ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.GynecologicalSymptoms ?? "Нет";
        worksheet.Cell(row, col++).Value = survey.HasUnordinaryUrine ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.HasSwelling ? "Да" : "Нет";
        worksheet.Cell(row, col++).Value = survey.SwellingDescription ?? "Нет";
        worksheet.Cell(row, col++).Value = WaterConsumedText[survey.WaterConsumed];
        worksheet.Cell(row, col++).Value = StoolText[survey.Stool];
        worksheet.Cell(row, col++).Value = UrinationText[survey.Urination];
    }
    
    public async Task<byte[]> ExportObservationParametersStatisticsAsync(Guid userId)
    {
        var parameters = await _statisticsService.GetObservationParametersStatistics(userId);
        
        if (!parameters.Any())
        {
            throw new ApiException($"Observation parameters not found for userId={userId}", "Ежедневные опросы не найдены для пользователя");
        }
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Параметры наблюдения");
        
        int col = 1;
        worksheet.Cell(1, col++).Value = "Параметр";
        worksheet.Cell(1, col++).Value = "Значение";
        worksheet.Cell(1, col++).Value = "Нижняя граница";
        worksheet.Cell(1, col++).Value = "Верхняя граница";
        worksheet.Cell(1, col++).Value = "В пределах нормы";
        
        int row = 2;
        foreach (var parameter in parameters)
        {
            col = 1;
            worksheet.Cell(row, col++).Value = parameter.ParameterName;
            worksheet.Cell(row, col++).Value = parameter.Value;
            worksheet.Cell(row, col++).Value = parameter.LowerBound;
            worksheet.Cell(row, col++).Value = parameter.UpperBound;
            
            bool isWithinNorm = true;
            if (parameter.LowerBound.HasValue && parameter.Value < parameter.LowerBound.Value)
            {
                isWithinNorm = false;
            }
            if (parameter.UpperBound.HasValue && parameter.Value > parameter.UpperBound.Value)
            {
                isWithinNorm = false;
            }
            
            worksheet.Cell(row, col++).Value = isWithinNorm ? "Да" : "Нет";
            
            if (!isWithinNorm)
            {
                worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Red;
            }
            
            row++;
        }
        
        worksheet.Columns().AdjustToContents();
        
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}