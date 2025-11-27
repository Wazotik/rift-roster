import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Container, Paper, TextInput, PasswordInput, Button, Title, Text, Anchor } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { useAuth } from '../hooks/useAuth';

const Login = () => {
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    const { loginUser } = useAuth();

    const handleLogin = (e: React.FormEvent) => {
        e.preventDefault();

        // Validation
        if (!username || !password) {
            notifications.show({
                title: 'Validation Error',
                message: 'Please fill in all fields',
                color: 'yellow',
            });
            return;
        }

        loginUser({ username, password });
    };

    return (
        <Container size={420} my={40}>
            <Title ta="center" style={{ fontWeight: 900 }}>
                Welcome back!
            </Title>
            <Text c="dimmed" size="sm" ta="center" mt={5}>
                Don't have an account yet?{' '}
                <Anchor size="sm" component={Link} to="/register">
                    Create account
                </Anchor>
            </Text>

            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                <form onSubmit={handleLogin}>
                    <TextInput
                        label="Username"
                        placeholder="Your username"
                        required
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                    <PasswordInput
                        label="Password"
                        placeholder="Your password"
                        required
                        mt="md"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <Button 
                        fullWidth 
                        mt="xl" 
                        type="submit"
                    >
                        Sign in
                    </Button>
                </form>
            </Paper>
        </Container>
    );
};

export default Login;
