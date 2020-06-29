<template>
  <div>
    <mediaOverview
      v-bind:origin="title"
      v-bind:onClose="closeOverview"
      v-bind:show="show"
      v-if="show !== null"
    />
    <div class="container is-fluid" v-if="show === null">
      <transition name="fade">
        <div id="loaded_shows" v-if="isLoaded">
          <div class="columns" v-for="(row, index) in showRows" :key="index">
            <div class="column" v-for="show in row" :key="show.id">
              <div
                class="box is-paddingless"
                v-on:click="() => openOverview(show)"
              >
                <article class="media">
                  <div class="media-content">
                    <div class="content has-text-centered">
                      <p>
                        <img
                          src="https://i.pinimg.com/originals/3b/8a/d2/3b8ad2c7b1be2caf24321c852103598a.jpg"
                        />
                        <strong> {{ show.primaryTitle }} </strong>
                        <br />
                        28 Episodes
                      </p>

                      <p></p>
                    </div>
                  </div>
                </article>
              </div>
            </div>
          </div>
        </div>
      </transition>
      <section class="section">
        <nav
          class="pagination is-centered"
          role="navigation"
          aria-label="pagination"
        >
          <ul class="pagination-list">
            <li>
              <a
                class="pagination-link"
                aria-label="Goto first page"
                v-on:click="goPage(0)"
                v-if="pageIndex > 1"
                >1</a
              >
            </li>
            <li>
              <span class="pagination-ellipsis" v-if="pageIndex > 1"
                >&hellip;</span
              >
            </li>
            <li>
              <a
                class="pagination-link"
                aria-label="Goto page previous"
                v-on:click="goPage(pageIndex - 1)"
                v-if="pageIndex > 0"
                >{{ pageIndex }}</a
              >
            </li>
            <li>
              <a
                class="pagination-link is-current"
                :aria-label="pageIndex"
                aria-current="page"
                >{{ pageIndex + 1 }}</a
              >
            </li>
            <li>
              <a
                class="pagination-link"
                aria-label="Goto next page"
                v-on:click="goPage(pageIndex + 1)"
                v-if="pageIndex < totalPages - 1"
                >{{ pageIndex + 2 }}</a
              >
            </li>
            <li>
              <span
                class="pagination-ellipsis"
                v-if="pageIndex < totalPages - 2"
                >&hellip;</span
              >
            </li>
            <li>
              <a
                class="pagination-link"
                aria-label="Goto last page"
                v-if="pageIndex < totalPages - 2"
                v-on:click="goPage(totalPages - 1)"
                >{{ totalPages }}</a
              >
            </li>
          </ul>
        </nav>
      </section>
    </div>
  </div>
</template>

<script lang="ts">
import Pagination from "../models/Pagination";
import ShowModel from "@/models/showModel";
import { chunk } from "underscore";
import mediaOverview from "./MediaOverview.vue";

export default {
  name: "MediaCards",
  components: { mediaOverview },
  data: function() {
    return {
      showRows: [] as Array<Array<Pagination<ShowModel>>>,
      totalPages: 0,
      pageIndex: 0,
      itemsPerPage: 8,
      isLoaded: false,
      show: (null as unknown) as ShowModel | null,
    };
  },
  props: {
    getMedia: Function,
    title: String,
  },
  beforeMount() {
    this.goPage(this.pageIndex);
  },
  methods: {
    goPage: function(index: number) {
      this.isLoaded = false;
      this.getMedia(index, this.itemsPerPage).then(this.setShows);
    },
    setShows: function(page: Pagination<ShowModel>) {
      this.showRows = chunk(page.items, 4) as Array<
        Array<Pagination<ShowModel>>
      >;
      this.pageIndex = page.index;
      this.itemsPerPage = page.itemsPerPage;
      this.totalPages = Math.ceil(page.total / page.itemsPerPage);
      this.isLoaded = true;
    },
    openOverview: function(show: ShowModel) {
      console.log(`Opening Show: ${show.id}`);
      this.show = show;
    },
    closeOverview: function() {
      this.show = null;
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
