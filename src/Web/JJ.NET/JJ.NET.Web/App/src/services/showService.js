import { ServiceBase } from "../core/serviceBase";
export default class ShowService extends ServiceBase {
    constructor() {
        super(...arguments);
        this.findShowPaginated = (index, itemsPerPage) => {
            return fetch(`/api/show/paged?index=${index}&itemsPerPage=${itemsPerPage}`).then((x) => x.json());
            // return this.requestJson<IPagination<ShowModel>>({
            //   url: `/api/show/paged?index=${index}&itemsPerPage=${itemsPerPage}`,
            //   method: "GET",
            // });
        };
        this.findAimePaginated = (index, itemsPerPage) => {
            return fetch(`/api/anime/paged?index=${index}&itemsPerPage=${itemsPerPage}`).then((res) => res.json());
        };
        this.getShowOvewview = (showId) => {
            return fetch(`/api/show/overview/${showId}`).then((res) => res.json());
        };
    }
}
//# sourceMappingURL=showService.js.map