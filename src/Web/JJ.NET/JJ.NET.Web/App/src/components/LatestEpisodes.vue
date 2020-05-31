<template>
  <div class="container is-fluid">
    <div class="columns" v-for="(episodes, index) in rows" :key="index">
      <div class="column" v-for="episode in episodes" :key="episode.id">
        <div
          class="box is-paddingless"
          v-on:click="episode.displayVideo = true"
        >
          <article class="media">
            <div class="media-content">
              <div class="content has-text-centered">
                <p>
                  <img :src="episode.wallpaper" />
                  <strong
                    >{{ episode.seasonNo }}x{{ episode.episodeNo }} -
                    {{ episode.title }}</strong
                  >
                  <br />
                  {{ episode.showTitle }}
                  <small>
                    {{ formatAge(episode.age) }}
                  </small>
                </p>

                <p></p>
              </div>
            </div>
          </article>
        </div>
        <!-- MODAL START -->
        <div
          class="modal"
          v-if="episode.displayVideo"
          style="display:inline-flex"
        >
          <div
            class="modal-background"
            v-on:click="episode.displayVideo = false"
          ></div>
          <div class="modal-content" style="min-width: 60vw;">
            <p class="image">
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
            </p>
          </div>
          <button
            class="modal-close is-large"
            aria-label="close"
            v-on:click="episode.displayVideo = false"
          ></button>
        </div>
        <!-- MODAL END -->
      </div>
    </div>
  </div>
</template>

<script lang="ts">
const episodes = Array(8).fill({
  guid: "FFJAFADKA-NMAFASJFD-AFSNADAK-DAD",
  title: "this is an episode title",
  showTitle: "this is an show title",
  seasonNo: 1,
  episodeNo: 6,
  age: new Date(),
  wallpaper: "https://cdn.wallpapersafari.com/32/11/1B9LhF.jpg",
  displayVideo: false,
});

const rows = Array(2)
  .fill([])
  .map((_) => episodes.splice(0, 4));

export default {
  name: "LatestEpisodes",
  components: {},
  data: function() {
    return {
      rows,
    };
  },
  methods: {
    formatAge: (ageDate: Date) => {
      const hours = Math.abs(new Date().getTime() - ageDate.getTime()) / 36e5;

      if (hours < 0.01) {
        return " just now";
      } else if (hours < 1) {
        return Math.floor(hours * 100) + " minutes ago";
      } else if (hours < 24) {
        return Math.floor(hours) + " hours ago";
      } else {
        return Math.floor(hours / 24) + " days ago";
      }
    },
  },
};
</script>

<style>
.box.is-paddingless :hover {
  transform: scale(1.009) !important;
}
</style>
