import {StatisticFields} from "../internal/statistic-fields-enum";

export interface ObservationParameterNormDto {
  parameterName: StatisticFields;
  lowerBound?: number;
  upperBound?: number;
  userId: string;
}

