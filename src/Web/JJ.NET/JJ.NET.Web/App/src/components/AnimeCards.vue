<template>
  <cards v-bind:getMedia="findAimePaginated" title="Anime" />
</template>

<script>
import cards from "../components/MediaCards";
import ShowService from "../services/showService";
const { findAimePaginated } = new ShowService();

export default {
  name: "Anime",
  components: { cards },
  data: function() {
    return {
      findAimePaginated,
    };
  },
};
</script>
