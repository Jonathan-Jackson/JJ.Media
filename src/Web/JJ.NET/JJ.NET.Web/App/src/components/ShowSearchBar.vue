<template>
  <section>
    <b-autocomplete
      :data="data"
      placeholder="Show Search"
      field="title"
      :loading="isFetching"
      @typing="getAsyncData"
      @select="(option) => (selected = option)"
    >
      <template slot-scope="props">
        <div class="media">
          <div class="media-left">
            <img
              width="32"
              :src="
                `https://image.tmdb.org/t/p/w500/${props.option.poster_path}`
              "
            />
          </div>
          <div class="media-content">
            {{ props.option.title }}
            <br />
            <small>
              Released at {{ props.option.release_date }}, rated
              <b>{{ props.option.vote_average }}</b>
            </small>
          </div>
        </div>
      </template>
    </b-autocomplete>
  </section>
</template>

<script>
import debounce from "lodash/debounce";
import Vue from "vue";
import { Autocomplete } from "buefy";

Vue.use(Autocomplete);

export default {
  data() {
    return {
      data: [],
      selected: null,
      isFetching: false,
    };
  },
  methods: {
    // You have to install and import debounce to use it,
    // it's not mandatory though.
    getAsyncData: debounce(function(name) {
      if (!name.length) {
        this.data = [];
        return;
      }
      this.isFetching = true;
      fetch(
        `https://api.themoviedb.org/3/search/movie?api_key=bb6f51bef07465653c3e553d6ab161a8&query=${name}`
      )
        .then((response) => response.json())
        .then((data) => {
          this.data = [];
          data.results.forEach((item) => this.data.push(item));
        })
        .catch((error) => {
          this.data = [];
          throw error;
        })
        .finally(() => {
          this.isFetching = false;
        });
    }, 100),
  },
};
</script>
