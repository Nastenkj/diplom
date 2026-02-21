namespace PregnancyAppBackend.Infrastructure.Security;

public static class Policies
{
    #region TestController

    public const string Test = "TestGet";
    public const string UnusedPolicy = "UnusedPolicy";

    #endregion

    #region DailySurveysController

    public const string GetLatestDailySurveyCreationDateUtc = "GetLatestDailySurveyCreationDateUtc";
    public const string PostDailySurvey = "PostDailySurvey";
    public const string GetDailySurveysForUser = "GetDailySurveysForUser";
    public const string GetDailySurveyById = "GetDailySurveyById";

    #endregion
    
    #region MedicalHistoriesController

    public const string GetMedicalHistory = "GetMedicalHistory";
    public const string PostMedicalHistory = "PostMedicalHistory";
    public const string GetMedicalHistoryById = "GetMedicalHistoryById";

    #endregion
    
    #region WeeklySurveysController

    public const string PostWeeklySurvey = "PostWeeklySurvey";
    public const string GetLatestWeeklySurvey = "GetLatestWeeklySurvey";
    public const string GetWeeklySurveysForUser = "GetWeeklySurveysForUser";
    public const string GetWeeklySurveyById = "GetWeeklySurveyById";

    #endregion
    
    #region PatientsController

    public const string GetAllPatients = "GetAllPatients";
    public const string GetPatient = "GetPatient";
    public const string EditPatientInfo = "EditPatientInfo";

    #endregion

    #region CommunicationLinksController

    public const string CreateCommunicationLink = "CreateCommunicationLink";
    public const string GetMyCommunicationLinks = "GetMyCommunicationLinks";

    #endregion
    
    #region DoctorsController

    public const string GetAllDoctors = "GetAllDoctors";
    public const string GetDoctor = "GetDoctor";
    public const string EditDoctorInfo = "EditDoctorInfo";

    #endregion
}