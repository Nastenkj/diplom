import {BloodPressure} from "../../enums/weekly-survey/blood-pressure";
import {WaterConsumed} from "../../enums/weekly-survey/water-consumed";
import {Stool} from "../../enums/weekly-survey/stool";
import {Urination} from "../../enums/weekly-survey/urination";

export interface WeeklySurveyDto {
  hasOrvi: boolean;
  orviSymptoms?: string;
  hasUnordinaryTemp: boolean;
  unordinaryTempOccurrences?: number;
  unordinaryBloodPressure: BloodPressure;
  hasGynecologicalSymptoms: boolean;
  gynecologicalSymptoms?: string;
  hasUnordinaryUrine: boolean;
  hasSwelling: boolean;
  swellingDescription?: string;
  waterConsumed: WaterConsumed;
  stool: Stool;
  urination: Urination;
  weightAdded: number;
  pregnancyWeek: number;
  creationDateUtc: string;
  id: string;
}
