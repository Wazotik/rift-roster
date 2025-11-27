import { apiGet, apiPost } from "../lib/apiClient";
import type { LoginRequest, RegisterRequest } from "../types/AuthenticationDtos";
import type { UserResponse } from "../types/UserDtos";

export const login = async (req: LoginRequest) => {
    await apiPost<LoginRequest, void>("/auth/login", req, { withCredentials: true });
};

export const logout = async () => {
    await apiPost("/auth/logout", { withCredentials: true });
};

export const register = async (req: RegisterRequest): Promise<UserResponse> => {
    const user = await apiPost<LoginRequest, UserResponse>("/auth/register", req, { withCredentials: true });
    return user;
};

export const me = async (): Promise<UserResponse> => {
    const user = await apiGet<UserResponse>("/auth/me");
    return user;
};