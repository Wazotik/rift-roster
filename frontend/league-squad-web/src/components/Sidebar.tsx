import { Drawer, Stack, Button, Text, Divider, Group, Avatar } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { IconHome, IconUsers, IconUser, IconLogin, IconUserPlus, IconLogout } from '@tabler/icons-react';

interface SidebarProps {
    opened: boolean;
    onClose: () => void;
}

const Sidebar = ({ opened, onClose }: SidebarProps) => {
    const navigate = useNavigate();
    const { isAuthenticated, user, logoutUser } = useAuth();

    const handleNavigation = (path: string) => {
        navigate(path);
        onClose();
    };

    const handleLogout = () => {
        logoutUser();
        onClose();
    };

    return (
        <Drawer
            opened={opened}
            onClose={onClose}
            position="left"
            size="280px"
            padding="md"
            title={
                <Text size="xl" fw={700} c="blue">
                    RiftRoster
                </Text>
            }
        >
            <Stack gap="md">
                {isAuthenticated && user ? (
                    <>
                        {/* User Info Section */}
                        <Group gap="sm" p="sm" style={{ background: '#f8f9fa', borderRadius: '8px' }}>
                            <Avatar color="blue" radius="xl">
                                {user.name?.charAt(0).toUpperCase() || user.username.charAt(0).toUpperCase()}
                            </Avatar>
                            <div style={{ flex: 1 }}>
                                <Text size="sm" fw={600}>
                                    {user.name || user.username}
                                </Text>
                                <Text size="xs" c="dimmed">
                                    @{user.username}
                                </Text>
                            </div>
                        </Group>

                        <Divider />

                        {/* Navigation Links */}
                        <Stack gap="xs">
                            <Button
                                variant="subtle"
                                leftSection={<IconHome size={18} />}
                                justify="flex-start"
                                onClick={() => handleNavigation('/')}
                            >
                                Home
                            </Button>
                            <Button
                                variant="subtle"
                                leftSection={<IconUsers size={18} />}
                                justify="flex-start"
                                onClick={() => handleNavigation('/squads')}
                            >
                                My Squads
                            </Button>
                            <Button
                                variant="subtle"
                                leftSection={<IconUser size={18} />}
                                justify="flex-start"
                                onClick={() => handleNavigation('/profile')}
                            >
                                Profile
                            </Button>
                        </Stack>

                        <Divider />

                        {/* Logout */}
                        <Button
                            variant="light"
                            color="red"
                            leftSection={<IconLogout size={18} />}
                            onClick={handleLogout}
                        >
                            Logout
                        </Button>
                    </>
                ) : (
                    <>
                        {/* Not authenticated */}
                        <Text size="sm" c="dimmed" ta="center" py="md">
                            Welcome! Please sign in to continue.
                        </Text>

                        <Stack gap="xs">
                            <Button
                                variant="filled"
                                leftSection={<IconLogin size={18} />}
                                onClick={() => handleNavigation('/login')}
                            >
                                Login
                            </Button>
                            <Button
                                variant="light"
                                leftSection={<IconUserPlus size={18} />}
                                onClick={() => handleNavigation('/register')}
                            >
                                Create Account
                            </Button>
                            <Divider />
                            <Button
                                variant="subtle"
                                leftSection={<IconHome size={18} />}
                                justify="flex-start"
                                onClick={() => handleNavigation('/')}
                            >
                                Home
                            </Button>
                        </Stack>
                    </>
                )}
            </Stack>
        </Drawer>
    );
};

export default Sidebar;
