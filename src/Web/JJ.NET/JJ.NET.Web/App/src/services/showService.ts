import { ServiceBase } from "../core/serviceBase";
import ShowModel from "../models/showModel";
import IPagination from "../models/Pagination";

export default class ShowService extends ServiceBase {
  public findShowPaginated = (
    index: number,
    itemsPerPage: number
  ): Promise<IPagination<ShowModel>> => {
    return fetch(
      `/api/show/paged?index=${index}&itemsPerPage=${itemsPerPage}`
    ).then((x) => x.json());

    // return this.requestJson<IPagination<ShowModel>>({
    //   url: `/api/show/paged?index=${index}&itemsPerPage=${itemsPerPage}`,
    //   method: "GET",
    // });
  };

  public findAimePaginated = (
    index: number,
    itemsPerPage: number
  ): Promise<IPagination<ShowModel>> => {
    return fetch(
      `/api/anime/paged?index=${index}&itemsPerPage=${itemsPerPage}`
    ).then((res) => res.json());
  };

  public getShowOvewview = (showId: number) => {
    return fetch(`/api/show/overview/${showId}`).then((res) => res.json());
  };
}
