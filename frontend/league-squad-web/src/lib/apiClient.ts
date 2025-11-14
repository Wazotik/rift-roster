import axios from "axios";

const BASE_ADDRESS = import.meta.env.VITE_API_BASE as string;

export const apiGet = async <TResponse>(path: string): Promise<TResponse> => {
    try {
        const res = await axios.get<TResponse>(`${BASE_ADDRESS}${path}`);
        // return Promise.resolve(res.data); // ik wrapping with promise is not needed, but it helps my understanding 
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiPost = async <TRequest, TResponse>(path: string, payload: TRequest): Promise<TResponse> => {
    try {
        const res = await axios.post<TResponse>(`${BASE_ADDRESS}${path}`, payload);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiPut = async <TRequest, TResponse>(path: string, payload: TRequest): Promise<TResponse> => {
    try {
        console.log(payload);
        const res = await axios.put<TResponse>(`${BASE_ADDRESS}${path}`, payload);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};

export const apiDelete = async <TResponse>(path: string): Promise<TResponse> => {
    try {
        const res = await axios.delete<TResponse>(`${BASE_ADDRESS}${path}`);
        return res.data;
    }
    catch (e: unknown) {
        console.log(e);
        throw e;
    }
};




