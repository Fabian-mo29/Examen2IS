import axios from "axios";

export default {
  install(app) {
    const baseApiUrl = "https://localhost:7280/api";
    app.config.globalProperties.$apiBaseUrl = baseApiUrl;
    app.config.globalProperties.$api = {
      getSodasList() {
        return axios.get(`${baseApiUrl}/Soda`);
      },
      completePayment(payment) {
        return axios.post(`${baseApiUrl}/Payment`, payment);
      },
    };
  },
};
