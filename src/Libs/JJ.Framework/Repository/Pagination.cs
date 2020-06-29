namespace JJ.Framework.Repository {

    public class Pagination<TResult> {
        public int Total { get; set; }
        public int Index { get; set; }
        public TResult[] Items { get; set; }
        public int ItemsPerPage { get; set; }
    }
}