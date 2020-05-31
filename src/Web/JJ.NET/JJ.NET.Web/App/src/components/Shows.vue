<template>
  <div class="container is-fluid">
    <div class="columns" v-for="(episodes, index) in rows" :key="index">
      <div class="column" v-for="episode in episodes" :key="episode.id">
        <div class="box is-paddingless">
          <article class="media">
            <div class="media-content">
              <div class="content has-text-centered">
                <p>
                  <img :src="episode.wallpaper" />
                  <strong> {{ episode.title }} </strong>
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

    <nav
      class="pagination is-centered"
      role="navigation"
      aria-label="pagination"
    >
      <ul class="pagination-list">
        <li><a class="pagination-link" aria-label="Goto page 1">1</a></li>
        <li><span class="pagination-ellipsis">&hellip;</span></li>
        <li><a class="pagination-link" aria-label="Goto page 45">45</a></li>
        <li>
          <a
            class="pagination-link is-current"
            aria-label="Page 46"
            aria-current="page"
            >46</a
          >
        </li>
        <li><a class="pagination-link" aria-label="Goto page 47">47</a></li>
        <li><span class="pagination-ellipsis">&hellip;</span></li>
        <li><a class="pagination-link" aria-label="Goto page 86">86</a></li>
      </ul>
    </nav>
  </div>
</template>

<script lang="ts">
const shows = Array(8).fill({
  id: 0,
  title: "this is a show title",
  wallpaper: "https://cdn.wallpapersafari.com/32/11/1B9LhF.jpg",
});

const rows = Array(2)
  .fill([])
  .map((_) => shows.splice(0, 4));

export default {
  name: "Shows",
  components: {},
  data: function() {
    return {
      rows,
    };
  },
  methods: {},
};
</script>

<style></style>
