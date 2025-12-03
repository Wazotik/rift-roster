import axios  from "axios";

const BASE_ADDRESS = import.meta.env.VITE_API_BASE as string;

const authAxios = axios.create({
    withCredentials: true,
    baseURL: BASE_ADDRESS
});

export const apiGet = async <TResponse>(path: string, overrideConfig: object = {}, forceRefresh: boolean = false): Promise<TResponse> => {
    try {
        const res = await authAxios.get<TResponse>(path, overrideConfig);
        // return Promise.resolve(res.data); // ik wrapping with promise is not needed, but it helps my understanding 
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiPost = async <TRequest, TResponse>(path: string, payload: TRequest, overrideConfig: object = {}): Promise<TResponse> => {
    try {
        const res = await authAxios.post<TResponse>(path, payload, overrideConfig);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiPut = async <TRequest, TResponse>(path: string, payload: TRequest, overrideConfig: object = {}): Promise<TResponse> => {
    try {
        console.log(payload);
        const res = await authAxios.put<TResponse>(path, payload, overrideConfig);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiDelete = async <TResponse>(path: string, overrideConfig: object = {}): Promise<TResponse> => {
    try {
        const res = await authAxios.delete<TResponse>(path, overrideConfig);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};




