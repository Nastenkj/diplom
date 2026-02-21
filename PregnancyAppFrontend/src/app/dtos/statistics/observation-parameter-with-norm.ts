import {ObservationParameterNormDto} from "../observation-parameter-norms/observation-parameter-norm-dto";
import {StatisticFields} from "../internal/statistic-fields-enum";

export interface ObservationParameterWithNormDto extends ObservationParameterNormDto {
  parameterName: StatisticFields;
  lowerBound?: number;
  upperBound?: number;
  value: number;
  userId: string;
}
