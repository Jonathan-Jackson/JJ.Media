namespace JJ.Framework.Controller {

    public class PaginationRequest {
        public int Index { get; set; }
        public int ItemsPerPage { get; set; }

        public int GetSkip()
            => ItemsPerPage * Index;
    }
}