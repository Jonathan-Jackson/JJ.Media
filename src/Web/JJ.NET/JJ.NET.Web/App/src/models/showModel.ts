import ShowTitleModel from "./showTitleModel";

export default class ShowModel {
  id: number = 0;
  airDate: Date = new Date();
  overview: string = "";
  showTitles: Array<ShowTitleModel> = [];
  tvDbId: number = 0;

  public getPrimaryTitle = () =>
    this.showTitles.find((title) => title.isPrimary) ?? this.showTitles[0];
}
