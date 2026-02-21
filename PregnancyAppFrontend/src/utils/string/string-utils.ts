export class StringUtils {
  public static isNullOrWhiteSpace(str: string): boolean {
    return str === null || str === undefined || str.match('/^ *$/') !== null || str === '';
  }
}
