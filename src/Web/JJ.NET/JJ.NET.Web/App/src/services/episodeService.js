import { ServiceBase } from "../core/serviceBase";
export default class EpisodeService extends ServiceBase {
    constructor() {
        super(...arguments);
        this.findEpisodes = (showId) => {
            return fetch(`/api/show/${showId}/episodes`).then((res) => res.json());
        };
    }
}
//# sourceMappingURL=episodeService.js.map