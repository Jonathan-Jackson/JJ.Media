<template>
  <cards v-bind:getMedia="findShowPaginated" title="Shows" />
</template>

<script>
import ShowService from "../services/showService";
import cards from "../components/MediaCards";
const { findShowPaginated } = new ShowService();

export default {
  name: "Shows",
  components: { cards },
  data: function() {
    return {
      findShowPaginated,
    };
  },
};
</script>
