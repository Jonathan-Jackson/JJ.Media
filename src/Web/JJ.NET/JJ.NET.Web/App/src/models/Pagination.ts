export default interface Pagination<TResult> {
  total: number;
  index: number;
  items: Array<TResult>;
  itemsPerPage: number;
}
