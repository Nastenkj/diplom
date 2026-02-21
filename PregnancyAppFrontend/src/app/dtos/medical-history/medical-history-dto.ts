import {BloodGroup} from "../../enums/medical-history/blood-group";
import {RhesusFactor} from "../../enums/medical-history/rhesus-factor";
import {Thermometer} from "../../enums/medical-history/thermometer";
import {BirthType} from "../../enums/medical-history/birth-type";
import {CovidStatus} from "../../enums/medical-history/covid-status";
import {HereditaryDisease} from "../../enums/medical-history/hereditary-disease";

export interface MedicalHistoryDto {
  weight: number;
  height: number;
  bloodGroup: BloodGroup;
  rhesusFactor: RhesusFactor;
  bloodPressure: string;
  thermometer: Thermometer;
  pregnancyAmount: number;
  abortionAmount?: number;
  miscarriageAmount?: number;
  prematureBirthAmount?: number;
  previousBirthType: BirthType;
  gynecologicalDiseases?: string;
  somaticDiseases?: string;
  undergoneOperations?: string;
  allergicReactions?: string;
  hereditaryDiseases: Array<HereditaryDisease>;
  isSmoking: boolean;
  isConsumingAlcohol: boolean;
  enduredCovid: CovidStatus;
}
