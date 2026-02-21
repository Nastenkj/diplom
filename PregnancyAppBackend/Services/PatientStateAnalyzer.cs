using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PregnancyAppBackend.Entities;  // Подставьте ваш namespace для Entities (например, Patient entity)
using PregnancyAppBackend.Persistence;  // Подставьте namespace для DbContext

namespace PregnancyAppBackend.Services
{
    public interface IPatientStateAnalyzer
    {
        Task<PatientStateResult> AnalyzePatientStateAsync(int patientId);
    }

    public class PatientStateAnalyzer : IPatientStateAnalyzer
    {
        private readonly ApplicationDbContext _dbContext;  // Замените на ваш DbContext

        public PatientStateAnalyzer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Метод для определения этапа беременности
        private string GetPregnancyStage(int gestationalWeek)
        {
            if (gestationalWeek <= 12)
                return "Ранний этап (1-12 недель)";
            else if (gestationalWeek <= 28)
                return "Средний этап (13-28 недель)";
            else
                return "Поздний этап (29+ недель)";
        }

        // Метод для проверки рисков
        private List<string> CheckRisks(Patient patient)
        {
            var risks = new List<string>();

            // Пример проверок; адаптируйте под ваши поля в Entity (Patient)
            if (patient.BloodPressureSystolic > 140 || patient.BloodPressureDiastolic > 90)
                risks.Add("Гипертензия (высокое давление)");
            if (patient.Hemoglobin < 110)
                risks.Add("Анемия (низкий гемоглобин)");
            if (patient.WeightGain > 20)  // Пример: чрезмерный набор веса
                risks.Add("Чрезмерный набор веса");

            // Добавьте больше проверок (например, сахар, симптомы)
            return risks;
        }

        // Основной метод анализа
        public async Task<PatientStateResult> AnalyzePatientStateAsync(int patientId)
        {
            // Запрос данных пациентки из БД
            var patient = await _dbContext.Patients  // Замените на вашу таблицу/сет
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                throw new Exception("Данные пациентки не найдены.");

            var stage = GetPregnancyStage(patient.GestationalWeek);
            var risks = CheckRisks(patient);

            string state;
            if (risks.Count == 0)
                state = "Нормальное";
            else if (risks.Count <= 2)
                state = "Требует мониторинга";
            else
                state = "Критическое";

            var lastUpdateDate = patient.LastUpdate.ToString("dd.MM.yyyy");

            return new PatientStateResult
            {
                Stage = stage,
                State = state,
                Risks = risks,
                LastUpdate = lastUpdateDate
            };
        }
    }

    // DTO для результата (можно разместить в Dtos)
    public class PatientStateResult
    {
        public string Stage { get; set; }
        public string State { get; set; }
        public List<string> Risks { get; set; }
        public string LastUpdate { get; set; }
    }
}