import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Container, Paper, TextInput, PasswordInput, Button, Title, Text, Anchor } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { useAuth } from '../hooks/useAuth';

const Register = () => {
    const [name, setName] = useState<string>("");
    const [email, setEmail] = useState<string>("");
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");

    const { registerUser } = useAuth();

    const handleRegister = (e: React.FormEvent) => {
        e.preventDefault();

        // Validation
        if (!name || !email || !username || !password || !confirmPassword) {
            notifications.show({
                title: 'Validation Error',
                message: 'Please fill in all fields',
                color: 'yellow',
            });
            return;
        }

        if (password !== confirmPassword) {
            notifications.show({
                title: 'Validation Error',
                message: 'Passwords do not match',
                color: 'yellow',
            });
            return;
        }

        if (password.length < 6) {
            notifications.show({
                title: 'Validation Error',
                message: 'Password must be at least 6 characters',
                color: 'yellow',
            });
            return;
        }

        registerUser({ name, email, username, password });
    };

    return (
        <Container size={420} my={40}>
            <Title ta="center" style={{ fontWeight: 900 }}>
                Create an account
            </Title>
            <Text c="dimmed" size="sm" ta="center" mt={5}>
                Already have an account?{' '}
                <Anchor size="sm" component={Link} to="/login">
                    Sign in
                </Anchor>
            </Text>

            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                <form onSubmit={handleRegister}>
                    <TextInput
                        label="Name"
                        placeholder="Your full name"
                        required
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                    <TextInput
                        label="Email"
                        placeholder="your@email.com"
                        required
                        type="email"
                        mt="md"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <TextInput
                        label="Username"
                        placeholder="Choose a username"
                        required
                        mt="md"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                    <PasswordInput
                        label="Password"
                        placeholder="At least 6 characters"
                        required
                        mt="md"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <PasswordInput
                        label="Confirm Password"
                        placeholder="Re-enter your password"
                        required
                        mt="md"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        error={confirmPassword && password !== confirmPassword ? 'Passwords do not match' : undefined}
                    />
                    <Button 
                        fullWidth 
                        mt="xl" 
                        type="submit"
                    >
                        Create account
                    </Button>
                </form>
            </Paper>
        </Container>
    );
};

export default Register;
