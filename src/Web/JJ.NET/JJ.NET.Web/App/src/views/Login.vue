<template>
  <div class="login">
    <section class="hero is-success is-fullheight" style="">
      <div class="hero-body">
        <div class="columns">
          <div class="column is-four-fifths"><br /></div>
          <div class="column">
            <div class="container">
              <h1 class="title">
                JJ-NET
              </h1>
              <h2 class="subtitle">
                <a
                  class="button is-large is-link"
                  href="https://discord.com/api/oauth2/authorize?client_id=709055703654072341&redirect_uri=http%3A%2F%2F192.168.1.36%3A8080%2F&response_type=code&scope=identify%20guilds"
                >
                  <span class="icon is-medium">
                    <i class="fab fa-discord" style="color:white"></i>
                  </span>
                  <span>Login</span>
                </a>
              </h2>
            </div>
          </div>

          <div class="column "></div>
        </div>
      </div>
    </section>
  </div>
</template>

<script lang="ts">
export default {
  name: "login",
  components: {},
};
</script>
