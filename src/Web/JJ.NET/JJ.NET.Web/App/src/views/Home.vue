<template>
  <div class="home">
    <section class="hero is-info is-small">
      <div class="hero-head">
        <navbar />
      </div>
      <div class="hero-body">
        <div
          class="container has-text-centered"
          style="max-width: 550px;
    display: table;
    min-width: 500px;"
        >
          <p class="title">
            <showSearch />
          </p>
          <p class="subtitle"></p>
        </div>
      </div>
      <div class="hero-foot">
        <nav class="tabs is-boxed is-fullwidth">
          <div class="container">
            <ul>
              <li
                v-bind:class="{ 'is-active': selection == 0 }"
                v-on:click="selection = 0"
              >
                <a>Latest</a>
              </li>
              <li
                v-bind:class="{ 'is-active': selection == 1 }"
                v-on:click="selection = 1"
              >
                <a>Shows</a>
              </li>
              <li
                v-bind:class="{ 'is-active': selection == 2 }"
                v-on:click="selection = 2"
              >
                <a>Anime</a>
              </li>
              <li
                v-bind:class="{ 'is-active': selection == 3 }"
                v-on:click="selection = 3"
              >
                <a>Movies</a>
              </li>
              <li
                v-bind:class="{ 'is-active': selection == 4 }"
                v-on:click="selection = 4"
              >
                <a>Random</a>
              </li>
            </ul>
          </div>
        </nav>
      </div>
    </section>
    <!-- -->
    <!-- Latest -->
    <section class="section is-small" v-if="selection == 0">
      <latestEpisodes />
    </section>
    <!-- Shows -->
    <section class="section is-small" v-if="selection == 1">
      <shows />
    </section>
    <!-- Anime -->
    <section class="section is-small" v-if="selection == 2">
      anime
    </section>
    <!-- Movies -->
    <section class="section is-small" v-if="selection == 3">
      movies
    </section>
    <!-- Random -->
    <section class="section is-small" v-if="selection == 4">
      random
    </section>
    <!-- -->
    <footer class="footer is-paddingless" style="background-color: white;">
      <div class="content has-text-centered">
        <p>
          <strong>JJ.NET</strong>
          V0.131 / Last Update: 2020-05-29 /
          <a href="https://github.com/Jonathan-Jackson/JJ.Media"> @Github</a>.
        </p>
      </div>
    </footer>
  </div>
</template>

<script>
import { eHomeSection } from "../core/enums";
import navbar from "../components/NavBar";
import latestEpisodes from "../components/LatestEpisodes";
import showSearch from "../components/ShowSearchBar";
import shows from "../components/Shows";

export default {
  name: "home",
  components: {
    navbar,
    latestEpisodes,
    showSearch,
    shows,
  },
  data: function() {
    return { selection: eHomeSection.Latest };
  },
};
</script>
