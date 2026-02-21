namespace PregnancyAppBackend.Infrastructure.Security;

public static class Claims
{
    public const string Roles = "roles";
    public const string DateOfChangeOfAccessRights = "date_of_change_of_access_rights";

    #region TestRegion

    public const string TestClaim = "test";
    public const string UnusedPolicy = "unusedPolicy";

    #endregion
    
    #region DailySurveysController

    public const string GetLatestDailySurveyCreationDateUtc = "get_latest_daily_survey_creation_date_utc";
    public const string PostDailySurvey = "post_daily_survey";
    public const string GetDailySurveysForUser = "get_daily_surveys_for_user";
    public const string GetDailySurveyById = "get_daily_survey_by_id";

    #endregion

    #region MedicalHistoriesController

    public const string GetMedicalHistory = "get_medical_history";
    public const string PostMedicalHistory = "post_medical_history";
    public const string GetMedicalHistoryById = "get_medical_history_by_id";

    #endregion
    
    #region WeeklySurveysController

    public const string PostWeeklySurvey = "post_weekly_survey";
    public const string GetLatestWeeklySurvey = "get_latest_weekly_survey";
    public const string GetWeeklySurveysForUser = "get_weekly_surveys_for_user";
    public const string GetWeeklySurveyById = "get_weekly_survey_by_id";

    #endregion
    
    #region PatientsController

    public const string GetAllPatients = "get_all_patients";
    public const string GetPatient = "get_patient";
    public const string EditPatientInfo = "edit_patient_info";

    #endregion
    
    #region CommunicationLinksController

    public const string CreateCommunicationLink = "create_communication_link";
    public const string GetMyCommunicationLinks = "get_my_communication_links";

    #endregion
    
    #region DoctorsController

    public const string GetAllDoctors = "get_all_doctors";
    public const string GetDoctor = "get_doctor";
    public const string EditDoctorInfo = "edit_doctor_info";

    #endregion
}