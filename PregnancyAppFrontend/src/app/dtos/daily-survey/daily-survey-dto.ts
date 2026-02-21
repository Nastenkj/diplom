import {Temperature} from "../../enums/daily-survey/temperature";

export interface DailySurveyDto {
  abdomenHurts: boolean;
  bloodDischarge: boolean;
  nausea: boolean;
  urgeToPuke: number;
  temperature: Temperature;
  systolicPressure: number;
  diastolicPressure: number;
  heartRate: number;
  glucoseLevel?: number;
  hemoglobinLevel?: number;
  saturation?: number;
  uro?: number;
  bld?: number;
  bil?: number;
  ket?: number;
  leu?: number;
  glu?: number;
  pro?: number;
  ph?: number;
  nit?: number;
  sg?: number;
  vc?: number;
  pt?: number;
  aptt?: number;
  inr?: number;
  oxygenLevel?: number;
  additionalInformation?: string;
  creationDateUtc: string;
  id: string;
}
