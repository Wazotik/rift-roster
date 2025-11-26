import { useMutation } from '@tanstack/react-query';
import { useState } from 'react'
import { login } from '../api/authentication';
import { Loader } from '@mantine/core';

const Login = () => {
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");


    const { data: token, mutate: loginMutate, isPending: isLoginPending, isError: isLoginError, error: loginError } = useMutation({
        mutationKey: ["login"],
        mutationFn: () => login({ username, password })
    });


    const handleLogin = async (e) => {
        e.preventDefault();
        loginMutate();
    };

    if (!token && isLoginError) {
        return (
            <div>
                login error: {loginError.message}
            </div>
        )
    }

    return (
        <>
            <form onSubmit={handleLogin}>
                <input type="text" placeholder='enter username' onChange={(e) => setUsername(e.target.value)} />
                <input type="text" placeholder='enter password' onChange={(e) => setPassword(e.target.value)} />
                <button type='submit'>log in</button>
            </form>
            {
                isLoginPending ?
                    (
                        <Loader />
                    )
                    :
                    (
                        <div>
                            Token: {token}
                        </div>

                    )
            }
        </>
    )
}

export default Login