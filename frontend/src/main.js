import { createApp } from "vue";
import App from "./App.vue";
import "bootstrap/dist/css/bootstrap.min.css";
import api from "./Api/api";

const app = createApp(App);
app.use(api);
app.mount("#app");
