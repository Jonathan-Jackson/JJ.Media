<template>
  <div id="overview">
    <section>
      <div class="container is-fluid">
        <div class="columns is-centered">
          <div class="column">
            <video
              id="player"
              playsinline
              controls
              data-poster="/path/to/poster.jpg"
            >
              <source
                src="http://jon-net.com/JJMedia2\Anime\Ascendance of a Bookworm\Season 1\Ascendance of a Bookworm - S01E23 (Harvest Festivals and Staying Home).webm"
                type="video/webm"
              />
            </video>
            <article class="panel is-link">
              <p class="panel-tabs">
                <a
                  v-bind:class="{ 'is-active': selectedSeason < 0 }"
                  v-on:click="selectedSeason = -1"
                  >All</a
                >
                <a
                  v-for="season in getSeasons()"
                  v-bind:key="season"
                  v-bind:class="{ 'is-active': selectedSeason === season }"
                  v-on:click="selectedSeason = season"
                >
                  {{ getSeasonTitle(season) }}
                </a>
              </p>
              <div class="panel-block">
                <p class="control has-icons-left">
                  <input
                    class="input is-link"
                    type="text"
                    placeholder="Search"
                  />
                  <span class="icon is-left">
                    <i class="fas fa-search" aria-hidden="true"></i>
                  </span>
                </p>
              </div>
              <a
                class="panel-block"
                v-for="episode in getEpisodesOfSeason()"
                :key="episode"
              >
                {{ episodeDisplayTitle(episode) }}
              </a>
            </article>
          </div>

          <div class="column"></div>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts">
import EpisodeService from "../services/episodeService";
import EpisodeModel from "@/models/episodeModel";
import ShowModel from "@/models/showModel";
import { uniq } from "underscore";

export default {
  name: "MediaOverview",
  components: {},
  data: function() {
    return {
      breadcrumbTitle: "...", // TODO: replace with show object
      episodes: (null as unknown) as EpisodeModel[] | null,
      episodeService: new EpisodeService(),
      selectedSeason: -1,
    };
  },
  props: {
    origin: String,
    show: { required: true },
    onClose: Function,
  },
  beforeMount() {
    this.getEpisodes();
  },
  methods: {
    getEpisodes: function() {
      this.episodeService
        .findEpisodes(this.show.id)
        .then((res) => (this.episodes = res.sort(this.compareEpisode)));
    },
    compareEpisode: function(a: EpisodeModel, b: EpisodeModel) {
      return (
        a.seasonNumber * 1000 +
        a.episodeNumber -
        (b.seasonNumber * 1000 + b.episodeNumber)
      );
    },
    episodeDisplayTitle: function(episode: EpisodeModel) {
      return `${episode.seasonNumber}x${episode.episodeNumber}. ${episode.title}`;
    },
    getSeasons: function(): number[] {
      if (this.episodes === null) return [];

      return uniq(this.episodes.map((ep) => ep.seasonNumber));
    },
    getSeasonTitle: function(seasonNo: number) {
      if (seasonNo === 0) return "Specials";
      else return "Season " + seasonNo;
    },
    getEpisodesOfSeason: function() {
      if (this.selectedSeason < 0 || this.episodes === null)
        return this.episodes;

      console.log("episodes of season..");
      return this.episodes.filter(
        (ep) => ep.seasonNumber === this.selectedSeason
      );
    },
  },
};
</script>
