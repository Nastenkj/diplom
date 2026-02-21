using PregnancyAppBackend.Dtos.Web.CommunicationLinks;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Dtos.Web.MedicalHistory;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Entities;

namespace PregnancyAppBackend.Converters;

public static class DtoToEntityConverters
{
    public static MedicalHistory ConvertToEntity(this MedicalHistoryDto dto)
    {
        return new MedicalHistory
        {
            Weight = dto.Weight,
            Height = dto.Height,
            BloodGroup = dto.BloodGroup,
            RhesusFactor = dto.RhesusFactor,
            BloodPressure = dto.BloodPressure,
            Thermometer = dto.Thermometer,
            PregnancyAmount = dto.PregnancyAmount,
            AbortionAmount = dto.AbortionAmount,
            MiscarriageAmount = dto.MiscarriageAmount,
            PrematureBirthAmount = dto.PrematureBirthAmount,
            PreviousBirthType = dto.PreviousBirthType,
            GynecologicalDiseases = dto.GynecologicalDiseases,
            SomaticDiseases = dto.SomaticDiseases,
            UndergoneOperations = dto.UndergoneOperations,
            AllergicReactions = dto.AllergicReactions,
            HereditaryDiseases = dto.HereditaryDiseases,
            IsSmoking = dto.IsSmoking,
            IsConsumingAlcohol = dto.IsConsumingAlcohol,
            EnduredCovid = dto.EnduredCovid
        };
    }

    public static DailySurvey ConvertToEntity(this DailySurveyDto dto)
    {
        return new DailySurvey
        {
            AbdomenHurts = dto.AbdomenHurts,
            BloodDischarge = dto.BloodDischarge,
            Nausea = dto.Nausea,
            UrgeToPuke = dto.UrgeToPuke,
            Temperature = dto.Temperature,
            SystolicPressure = dto.SystolicPressure,
            DiastolicPressure = dto.DiastolicPressure,
            HeartRate = dto.HeartRate,
            GlucoseLevel = dto.GlucoseLevel,
            HemoglobinLevel = dto.HemoglobinLevel,
            Saturation = dto.Saturation,
            Uro = dto.Uro,
            Bld = dto.Bld,
            Bil = dto.Bil,
            Ket = dto.Ket,
            Leu = dto.Leu,
            Glu = dto.Glu,
            Pro = dto.Pro,
            Ph = dto.Ph,
            Nit = dto.Nit,
            Sg = dto.Sg,
            OxygenLevel = dto.OxygenLevel,
            AdditionalInformation = dto.AdditionalInformation,
            Vc = dto.Vc,
            Pt = dto.Ph,
            Aptt = dto.Aptt,
            Inr = dto.Inr,
        };
    }

    public static WeeklySurvey ConvertToEntity(this WeeklySurveyDto dto)
    {
        return new WeeklySurvey
        {
            HasOrvi = dto.HasOrvi,
            OrviSymptoms = dto.OrviSymptoms,
            HasUnordinaryTemp = dto.HasUnordinaryTemp,
            UnordinaryTempOccurrences = dto.UnordinaryTempOccurrences,
            UnordinaryBloodPressure = dto.UnordinaryBloodPressure,
            HasGynecologicalSymptoms = dto.HasGynecologicalSymptoms,
            PregnancyWeek = dto.PregnancyWeek,
            GynecologicalSymptoms = dto.GynecologicalSymptoms,
            HasUnordinaryUrine = dto.HasUnordinaryUrine,
            HasSwelling = dto.HasSwelling,
            SwellingDescription = dto.SwellingDescription,
            WaterConsumed = dto.WaterConsumed,
            Stool = dto.Stool,
            Urination = dto.Urination,
            WeightAdded = dto.WeightAdded
        };
    }
    
    public static PatientDoctorCommunicationLink ConvertToEntity(this CreateCommunicationLinkDto dto, Guid doctorId)
    {
        return new PatientDoctorCommunicationLink
        {
            UserId = dto.PatientId,
            DoctorId = doctorId,
            CommunicationLink = dto.CustomLink,
            MeetingScheduledAtUtc = dto.MeetingScheduledAtUtc
        };
    }
}