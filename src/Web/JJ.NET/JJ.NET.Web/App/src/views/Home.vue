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
                v-for="tab in tabs"
                v-bind:key="tab"
                v-bind:class="{ 'is-active': currentTab === tab }"
                v-on:click="currentTab = tab"
              >
                <a>{{ tab }}</a>
              </li>
            </ul>
          </div>
        </nav>
      </div>
    </section>

    <!-- -->
    <section class="section is-small">
      <transition name="fade">
        <keep-alive>
          <component v-bind:is="currentTabComponent"></component>
        </keep-alive>
      </transition>
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
import cards from "../components/MediaCards";
import anime from "../components/AnimeCards";
import shows from "../components/ShowCards";

export default {
  name: "home",
  components: {
    navbar,
    "tab-latest": latestEpisodes,
    "tab-shows": shows,
    "tab-anime": anime,
    "tab-movies": cards, //TODO
    showSearch,
    cards,
  },
  data: function() {
    return {
      selection: eHomeSection.Latest,
      currentTab: "Latest",
      tabs: ["Latest", "Shows", "Anime", "Movies"],
    };
  },
  computed: {
    currentTabComponent: function() {
      return "tab-" + this.currentTab.toLowerCase();
    },
  },
};
</script>

<style>
.fade-enter-active {
  transition: opacity 1.2s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
</style>
