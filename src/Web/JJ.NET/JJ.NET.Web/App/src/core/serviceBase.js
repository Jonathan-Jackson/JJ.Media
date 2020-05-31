import Axios from "axios";
import queryString from "query-string";
/**
 * Represents base class of the isomorphic service.
 */
export class ServiceBase {
    /**
     * Make request with JSON data.
     * @param opts
     */
    async requestJson(opts) {
        let axiosResult = null;
        let result = null;
        const processQuery = (url, data) => {
            if (data) {
                return `${url}?${queryString.stringify(data)}`;
            }
            return url;
        };
        // Make SSR requests 'authorized' from the NodeServices to the web server.
        let axiosRequestConfig = {
            withCredentials: true,
        };
        try {
            switch (opts.method) {
                case "GET":
                    axiosResult = await Axios.get(processQuery(opts.url, opts.data), axiosRequestConfig);
                    break;
                case "POST":
                    axiosResult = await Axios.post(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "PUT":
                    axiosResult = await Axios.put(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "PATCH":
                    axiosResult = await Axios.patch(opts.url, opts.data, axiosRequestConfig);
                    break;
                case "DELETE":
                    axiosResult = await Axios.delete(processQuery(opts.url, opts.data), axiosRequestConfig);
                    break;
            }
            result = axiosResult.data.value;
        }
        catch (error) {
            console.log("Request Failed: " + error.message);
        }
        return result;
    }
    /**
     * Allows you to send files to the server.
     * @param opts
     */
    async sendFormData(opts) {
        let axiosResult = null;
        let result = null;
        const axiosOpts = {
            headers: {
                "Content-Type": "multipart/form-data",
            },
        };
        try {
            switch (opts.method) {
                case "POST":
                    axiosResult = await Axios.post(opts.url, opts.data, axiosOpts);
                    break;
                case "PUT":
                    axiosResult = await Axios.put(opts.url, opts.data, axiosOpts);
                    break;
                case "PATCH":
                    axiosResult = await Axios.patch(opts.url, opts.data, axiosOpts);
                    break;
            }
            result = axiosResult.data.value;
        }
        catch (error) {
            console.log("Request Failed: " + error.message);
        }
        return result;
    }
}
//# sourceMappingURL=serviceBase.js.map