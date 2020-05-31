import Axios, { AxiosRequestConfig } from "axios";
import queryString from "query-string";
import SessionManager from "./sessionManager";

export interface IRequestOptions {
  url: string;
  data?: any;
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
}

export interface ISendFormDataOptions {
  url: string;
  data: FormData;
  method: "POST" | "PUT" | "PATCH";
}

/**
 * Represents base class of the isomorphic service.
 */
export abstract class ServiceBase {
  /**
   * Make request with JSON data.
   * @param opts
   */
  public async requestJson<T>(opts: IRequestOptions): Promise<T> {
    let axiosResult = null;
    let result = null;

    const processQuery = (url: string, data: any): string => {
      if (data) {
        return `${url}?${queryString.stringify(data)}`;
      }
      return url;
    };

    // Make SSR requests 'authorized' from the NodeServices to the web server.
    let axiosRequestConfig: AxiosRequestConfig = {
      withCredentials: true,
    };

    try {
      switch (opts.method) {
        case "GET":
          axiosResult = await Axios.get(
            processQuery(opts.url, opts.data),
            axiosRequestConfig
          );
          break;
        case "POST":
          axiosResult = await Axios.post(
            opts.url,
            opts.data,
            axiosRequestConfig
          );
          break;
        case "PUT":
          axiosResult = await Axios.put(
            opts.url,
            opts.data,
            axiosRequestConfig
          );
          break;
        case "PATCH":
          axiosResult = await Axios.patch(
            opts.url,
            opts.data,
            axiosRequestConfig
          );
          break;
        case "DELETE":
          axiosResult = await Axios.delete(
            processQuery(opts.url, opts.data),
            axiosRequestConfig
          );
          break;
      }
      result = axiosResult.data.value;
    } catch (error) {
      console.log("Request Failed: " + error.message);
    }

    return result;
  }

  /**
   * Allows you to send files to the server.
   * @param opts
   */
  public async sendFormData<T>(opts: ISendFormDataOptions): Promise<T> {
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
    } catch (error) {
      console.log("Request Failed: " + error.message);
    }

    return result;
  }
}
