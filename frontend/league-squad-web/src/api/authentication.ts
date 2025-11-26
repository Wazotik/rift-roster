import { apiPost } from "../lib/apiClient";
import type { LoginRequest, RegisterRequest } from "../types/AuthenticationDtos";
import type { UserResponse } from "../types/UserDtos";

export const login = async (req: LoginRequest): Promise<string> => {
    const token = await apiPost<LoginRequest, string>("/auth/login", req);
    return token;
};

export const register = async (req: RegisterRequest): Promise<UserResponse> => {
    const user = await apiPost<LoginRequest, UserResponse>("/auth/register", req);
    return user;
};