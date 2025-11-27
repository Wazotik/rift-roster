import { Container, Paper, Title, Text, Stack, Group, Avatar, Divider } from '@mantine/core';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

const Profile = () => {
    const { user, isAuthenticated } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
        }
    }, [isAuthenticated, navigate]);

    if (!user) {
        return null;
    }

    return (
        <Container size="md" py={60}>
            <Paper shadow="md" p={40} radius="md" withBorder>
                <Stack gap="lg">
                    {/* Header */}
                    <Group gap="lg">
                        <Avatar size="xl" color="blue" radius="xl">
                            {user.name?.charAt(0).toUpperCase() || user.username.charAt(0).toUpperCase()}
                        </Avatar>
                        <div>
                            <Title order={2}>{user.name || user.username}</Title>
                            <Text c="dimmed" size="sm">@{user.username}</Text>
                        </div>
                    </Group>

                    <Divider />

                    {/* User Information */}
                    <Stack gap="md">
                        <Title order={3} size="h4">Account Information</Title>
                        
                        <Group>
                            <Text fw={600} w={120}>Name:</Text>
                            <Text>{user.name || 'Not set'}</Text>
                        </Group>

                        <Group>
                            <Text fw={600} w={120}>Username:</Text>
                            <Text>{user.username}</Text>
                        </Group>

                        <Group>
                            <Text fw={600} w={120}>Email:</Text>
                            <Text>{user.email}</Text>
                        </Group>

                        <Group>
                            <Text fw={600} w={120}>Role:</Text>
                            <Text tt="capitalize">{user.role}</Text>
                        </Group>

                        <Group>
                            <Text fw={600} w={120}>Member Since:</Text>
                            <Text>{new Date(user.createdAt).toLocaleDateString()}</Text>
                        </Group>
                    </Stack>
                </Stack>
            </Paper>
        </Container>
    );
};

export default Profile;
