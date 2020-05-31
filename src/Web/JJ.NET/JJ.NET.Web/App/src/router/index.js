import Vue from "vue";
import VueRouter from "vue-router";
import Login from "../views/Login.vue";
import Home from "../views/Home.vue";
import SessionManager from "../core/sessionManager";
Vue.use(VueRouter);
let routes = [];
if (!SessionManager.isAuthenticated) {
  routes = [
    {
      path: "/",
      name: "Login",
      component: Login,
    },
  ];
} else {
  routes = [
    {
      path: "/",
      name: "Home",
      component: Home,
    },
    {
      path: "/about",
      name: "About",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () =>
        import(/* webpackChunkName: "about" */ "../views/About.vue"),
    },
  ];
}
const router = new VueRouter({
  routes,
});
export default router;
//# sourceMappingURL=index.js.map
