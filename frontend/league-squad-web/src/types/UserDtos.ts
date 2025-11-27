export interface UserResponse {
    username: string,
    name: string,
    email: string,
    role: string,
    createdAt: string
};

export type CreateUserRequest = Omit<UserResponse, "createdAt"> & {
    password: string,
};