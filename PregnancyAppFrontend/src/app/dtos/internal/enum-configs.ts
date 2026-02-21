import {StatisticFields} from "./statistic-fields-enum";
import {BloodPressure, bloodPressureStringify} from "../../enums/weekly-survey/blood-pressure";
import {WaterConsumed, waterConsumedStringify} from "../../enums/weekly-survey/water-consumed";
import {Stool, stoolStringify} from "../../enums/weekly-survey/stool";
import {Urination, urinationStringify} from "../../enums/weekly-survey/urination";
import {Temperature, temperatureStringify} from "../../enums/daily-survey/temperature";

export interface EnumTypeConfig {
  enumType: string;
  stringifyFn: (value: number) => string;
  yAxisTicks: number[];
  yScaleMin?: number;
  yScaleMax?: number;
}

// Словарь соответствий между полями статистики и их конфигурациями
export const enumConfigs = new Map<StatisticFields, EnumTypeConfig>([
  [StatisticFields.UnordinaryBloodPressure, {
    enumType: 'BloodPressure',
    stringifyFn: (value: number) => bloodPressureStringify(value as BloodPressure),
    yAxisTicks: [0, 1, 2],
    yScaleMin: -0.5,
    yScaleMax: 2.5
  }],
  [StatisticFields.WaterConsumed, {
    enumType: 'WaterConsumed',
    stringifyFn: (value: number) => waterConsumedStringify(value as WaterConsumed),
    yAxisTicks: [0, 1, 2]
  }],
  [StatisticFields.Stool, {
    enumType: 'Stool',
    stringifyFn: (value: number) => stoolStringify(value as Stool),
    yAxisTicks: [0, 1, 2]
  }],
  [StatisticFields.Urination, {
    enumType: 'Urination',
    stringifyFn: (value: number) => urinationStringify(value as Urination),
    yAxisTicks: [0, 1]
  }],
  [StatisticFields.Temperature, {
    enumType: 'Temperature',
    stringifyFn: (value: number) => temperatureStringify(value as Temperature),
    yAxisTicks: [0, 1, 2]
  }]
]);
