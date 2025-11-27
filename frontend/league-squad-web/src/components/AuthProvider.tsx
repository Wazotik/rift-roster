import { useState, type ReactNode } from 'react'
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import type { LoginRequest, RegisterRequest } from '../types/AuthenticationDtos';
import { login, logout, me, register } from '../api/authentication';
import { useMutation, useQuery } from '@tanstack/react-query';
import type { UserResponse } from '../types/UserDtos';
import { queryClient } from '../lib/queryClient';
import { notifications } from '@mantine/notifications';

type AuthContextPropsType = {
    children: ReactNode
};

const AuthProvider = ({ children }: AuthContextPropsType) => {
    // const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

    const navigate = useNavigate();

    const { data: user } = useQuery<UserResponse | undefined, Error>({
        queryKey: ["user"],
        queryFn: me,
        retry: false,
    });

    const isAuthenticated = !!user;

    const { mutate: loginMutate } = useMutation({
        mutationFn: (credentials: LoginRequest) => login(credentials),
        onSuccess: () => {
            notifications.show({
                title: 'Success!',
                message: 'Login successful',
                color: 'green',
            });
            queryClient.invalidateQueries({ queryKey: ["user"] });
            navigate("/squads");
        },
        onError: (error: Error) => {
            notifications.show({
                title: 'Login Failed',
                message: error.message || 'Invalid username or password',
                color: 'red',
            });
        },
    });

    const { mutate: logoutMutate } = useMutation({
        mutationFn: logout,
        onSuccess: () => {
            notifications.show({
                title: 'Success!',
                message: 'Logged out successfully',
                color: 'green',
            });
            queryClient.invalidateQueries({ queryKey: ["user"] });
            navigate("/");
        },
        onError: (error: Error) => {
            notifications.show({
                title: 'Logout Failed',
                message: error.message || 'Failed to logout',
                color: 'red',
            });
        },
    });

    const { mutate: registerMutate } = useMutation({
        mutationFn: (newUserInfo: RegisterRequest) => register(newUserInfo),
        onSuccess: () => {
            notifications.show({
                title: 'Success!',
                message: 'Account created! Please log in.',
                color: 'green',
            });
            navigate("/login");
        },
        onError: (error: Error) => {
            notifications.show({
                title: 'Registration Failed',
                message: error.message || 'Failed to create account',
                color: 'red',
            });
        },
    });

    const loginUser = (req: LoginRequest) => {
        loginMutate(req);
    };

    const logoutUser = () => {
        logoutMutate();
    };

    const registerUser = (req: RegisterRequest) => {
        registerMutate(req);
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, loginUser, logoutUser, registerUser, user }}>
            {children}
        </AuthContext.Provider>
    )
}

export default AuthProvider
