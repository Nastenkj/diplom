using PregnancyAppBackend.Dtos.Web.CommunicationLinks;
using PregnancyAppBackend.Dtos.Web.DailySurvey;
using PregnancyAppBackend.Dtos.Web.MedicalHistory;
using PregnancyAppBackend.Dtos.Web.ObservationParameterNorm;
using PregnancyAppBackend.Dtos.Web.Patients;
using PregnancyAppBackend.Dtos.Web.WeeklySurvey;
using PregnancyAppBackend.Entities;
using PregnancyAppBackend.Exceptions;

namespace PregnancyAppBackend.Converters;

public static class EntityToDtoConverters
{
    public static MedicalHistoryDto ConvertToDto(this MedicalHistory entity)
    {
        return new MedicalHistoryDto
        {
            Weight = entity.Weight,
            Height = entity.Height,
            BloodGroup = entity.BloodGroup,
            RhesusFactor = entity.RhesusFactor,
            BloodPressure = entity.BloodPressure,
            Thermometer = entity.Thermometer,
            PregnancyAmount = entity.PregnancyAmount,
            AbortionAmount = entity.AbortionAmount,
            MiscarriageAmount = entity.MiscarriageAmount,
            PrematureBirthAmount = entity.PrematureBirthAmount,
            PreviousBirthType = entity.PreviousBirthType,
            GynecologicalDiseases = entity.GynecologicalDiseases,
            SomaticDiseases = entity.SomaticDiseases,
            UndergoneOperations = entity.UndergoneOperations,
            AllergicReactions = entity.AllergicReactions,
            HereditaryDiseases = entity.HereditaryDiseases,
            IsSmoking = entity.IsSmoking,
            IsConsumingAlcohol = entity.IsConsumingAlcohol,
            EnduredCovid = entity.EnduredCovid
        };
    }

    public static DailySurveyDto ConvertToDto(this DailySurvey entity)
    {
        return new DailySurveyDto
        {
            AbdomenHurts = entity.AbdomenHurts,
            BloodDischarge = entity.BloodDischarge,
            Nausea = entity.Nausea,
            UrgeToPuke = entity.UrgeToPuke,
            Temperature = entity.Temperature,
            DiastolicPressure = entity.DiastolicPressure,
            SystolicPressure = entity.SystolicPressure,
            HeartRate = entity.HeartRate,
            GlucoseLevel = entity.GlucoseLevel,
            HemoglobinLevel = entity.HemoglobinLevel,
            Saturation = entity.Saturation,
            Uro = entity.Uro,
            Bld = entity.Bld,
            Bil = entity.Bil,
            Ket = entity.Ket,
            Leu = entity.Leu,
            Glu = entity.Glu,
            Pro = entity.Pro,
            Ph = entity.Ph,
            Nit = entity.Nit,
            Sg = entity.Sg,
            OxygenLevel = entity.OxygenLevel,
            AdditionalInformation = entity.AdditionalInformation,
            CreationDateUtc = entity.CreationDateUtc,
            Id = entity.Id,
            Vc = entity.Vc,
            Pt = entity.Ph,
            Aptt = entity.Aptt,
            Inr = entity.Inr,
        };
    }

    public static WeeklySurveyDto ConvertToDto(this WeeklySurvey entity)
    {
        return new WeeklySurveyDto
        {
            Id = entity.Id,
            PregnancyWeek = entity.PregnancyWeek,
            HasOrvi = entity.HasOrvi,
            OrviSymptoms = entity.OrviSymptoms,
            HasUnordinaryTemp = entity.HasUnordinaryTemp,
            UnordinaryTempOccurrences = entity.UnordinaryTempOccurrences,
            UnordinaryBloodPressure = entity.UnordinaryBloodPressure,
            HasGynecologicalSymptoms = entity.HasGynecologicalSymptoms,
            GynecologicalSymptoms = entity.GynecologicalSymptoms,
            HasUnordinaryUrine = entity.HasUnordinaryUrine,
            HasSwelling = entity.HasSwelling,
            SwellingDescription = entity.SwellingDescription,
            WaterConsumed = entity.WaterConsumed,
            Stool = entity.Stool,
            Urination = entity.Urination,
            WeightAdded = entity.WeightAdded,
            CreationDateUtc = entity.CreationDateUtc,
        };
    }

    public static TableUserDto ConvertToTableUserDto(this UserCommonInfo userCommonInfo)
    {
        if (userCommonInfo?.User is null)
        {
            throw new ApiException($"User not found in userCommonInfoId={userCommonInfo?.Id}", "User not found.");
        }

        return new TableUserDto
        {
            Id = userCommonInfo.UserId,
            Email = userCommonInfo.User.Email,
            FullName = userCommonInfo.FullName,
            PhoneNumber = userCommonInfo.PhoneNumber,
            BirthDate = userCommonInfo.BirthDate
        };
    }

    public static TableUsersDto ConvertToTableUsersDto(this List<TableUserDto> tableUsers, int total)
    {
        return new TableUsersDto()
        {
            Total = total,
            TableUsers = tableUsers
        };
    }
    
    public static UserDto ConvertToUserDto(this UserCommonInfo userCommonInfo)
    {
        if (userCommonInfo?.User is null)
        {
            throw new ApiException($"User not found in userCommonInfoId={userCommonInfo?.Id}", "User not found.");
        }

        return new UserDto
        {
            Id = userCommonInfo.UserId,
            Email = userCommonInfo.User.Email,
            FullName = userCommonInfo.FullName,
            PhoneNumber = userCommonInfo.PhoneNumber,
            BirthDate = userCommonInfo.BirthDate,
            TrustedPersonFullName = userCommonInfo.TrustedPersonFullName,
            TrustedPersonPhoneNumber = userCommonInfo.TrustedPersonPhoneNumber,
            TrustedPersonEmail = userCommonInfo.TrustedPersonEmail,
            InsuranceNumber = userCommonInfo.InsuranceNumber
        };
    }
    
    public static PatientDoctorCommunicationLinkDto ConvertToDto(this PatientDoctorCommunicationLink entity, string userName, string doctorName)
    {
        return new PatientDoctorCommunicationLinkDto
        {
            UserId = entity.UserId,
            UserName = userName,
            DoctorId = entity.DoctorId,
            DoctorName = doctorName,
            CommunicationLink = entity.CommunicationLink,
            MeetingScheduledAtUtc = entity.MeetingScheduledAtUtc,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }
    
    public static ObservationParameterNormDto ConvertToDto(this ObservationParameterNorm entity)
    {
        return new ObservationParameterNormDto
        {
            UserId = entity.UserId,
            
            LowerBound = entity.LowerBound,
            UpperBound = entity.UpperBound,
            ParameterName = entity.Parameter.ParameterName,
            ParameterId = entity.ParameterId
        };
    }
}