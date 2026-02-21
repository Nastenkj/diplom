import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClaimsConfigService {
  private readonly claimsMap: Record<string, string[]> = {
    // TODO: здесь и на бэке нужно с клеймами прибраться, сделать константами и для каждого эндпоинта свою policy
    'patient.daily_survey': ['post_daily_survey', 'get_latest_daily_survey_creation_date_utc'],
    'patient.weekly_survey': ['post_weekly_survey', 'get_latest_weekly_survey'],
    'patient.medical_history': ['get_medical_history', 'post_medical_history'],

    'doctor.patients': ['get_all_patients'],
    'doctor.patient_details': ['get_patient',
      'get_medical_history',
      'get_daily_surveys_for_user',
      'get_daily_survey_by_id',
    'get_weekly_surveys_for_user',
    'get_weekly_survey_by_id'],

    'communication_links_list': ['get_my_communication_links'],

    'patient.edit': ['edit_patient_info'],
    'doctor.edit': ['edit_doctor_info'],

    'admin.doctors': ['get_all_doctors'],
    'admin.doctor_details': ['get_doctor'],

    'doctor.create.communication.link': ['create_communication_link']
  };

  getRequiredClaims(key: string): string[] {
    return this.claimsMap[key] || [];
  }
}
