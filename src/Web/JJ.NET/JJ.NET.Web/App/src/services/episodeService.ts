import { ServiceBase } from "../core/serviceBase";
import EpisodeModel from "../models/episodeModel";

export default class EpisodeService extends ServiceBase {
  public findEpisodes = (showId: number): Promise<EpisodeModel[]> => {
    return fetch(`/api/show/${showId}/episodes`).then((res) => res.json());
  };
}
