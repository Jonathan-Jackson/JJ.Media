export default class ShowModel {
    constructor() {
        this.id = 0;
        this.airDate = new Date();
        this.overview = "";
        this.showTitles = [];
        this.tvDbId = 0;
        this.getPrimaryTitle = () => this.showTitles.find((title) => title.isPrimary) ?? this.showTitles[0];
    }
}
//# sourceMappingURL=showModel.js.map