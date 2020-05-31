<template>
  <nav class="navbar">
    <div class="container">
      <div class="navbar-brand">
        <a class="navbar-item">
          JJ.NET
        </a>
        <span class="navbar-burger burger" data-target="navbarMenuHeroB">
          <span></span>
          <span></span>
          <span></span>
        </span>
      </div>
      <div id="navbarMenuHeroB" class="navbar-menu">
        <div class="navbar-end">
          <a class="navbar-item is-active">
            Home
          </a>
          <a class="navbar-item">
            This Season
          </a>
          <a class="navbar-item">
            Logout
          </a>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
export default {
  name: "navbar",
  components: {},
};
</script>
