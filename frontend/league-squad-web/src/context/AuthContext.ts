import { Context, createContext, ReactNode } from "react";
import type { UserResponse } from "../types/UserDtos";
import type { LoginRequest, RegisterRequest } from "../types/AuthenticationDtos";

type AuthState = {
    isAuthenticated: boolean,
    loginUser: (req: LoginRequest) => void,
    logoutUser: () => void,
    registerUser: (req: RegisterRequest) => void,
    user?: UserResponse
};


export const AuthContext: Context<AuthState | null> = createContext<AuthState | null>(null);