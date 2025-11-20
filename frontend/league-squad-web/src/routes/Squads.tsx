import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createSquad, getAllSquads, getSquad, updateSquad } from "../api/squads";
import type { CreateSquadRequest, SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import { Container, Box, Button, Modal, TextInput, Title, Group, Stack, SimpleGrid, Text, Loader, Image, LoadingOverlay, Select, Flex } from "@mantine/core";
import { notifications } from '@mantine/notifications';
import SquadCard from "../components/SquadCard";
import { useDisclosure } from "@mantine/hooks";
import { useForm } from '@mantine/form';
import { useEffect, useState } from "react";

import { squadIconOptions } from "../assets/squadIconOptions";

const Squads = () => {
    const [visible, { toggle }] = useDisclosure(true);
    const [opened, { open, close }] = useDisclosure(false);
    const [newSquadName, setNewSquadName] = useState("");
    const [newSquadImage, setNewSquadImage] = useState("");

    const queryClient = useQueryClient();

    // Get all squads
    const { data: squads, isLoading: isSquadsLoading, isError: isSquadsError, error: squadsError } = useQuery<SquadResponse[], Error>({
        queryKey: ["squads"],
        queryFn: getAllSquads,
    });

    // Create a Squad
    const { data: createdSquad, mutate: createSquadMutate, isPending: isCreateSquadPending, isSuccess: isCreateSquadSuccess, isError: isCreateSquadError, error: createSquadError } = useMutation<SquadResponse, Error, CreateSquadRequest>({
        mutationFn: createSquad,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["squads"] });
            console.log("squad created", createdSquad);
            setNewSquadName("");
            close();
            notifications.show({
                title: 'Success!',
                message: 'Squad created successfully',
                color: 'green',
            });
        },
        onError: (error) => {
            console.log("create squad error!", error.message);
            notifications.show({
                title: 'Error',
                message: `Failed to create squad: ${error.message}`,
                color: 'red',
            });
        },
    });

    const handleCreateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        createSquadMutate({
            name: newSquadName,
            iconUrl: newSquadImage
        });
    }

    if (isSquadsError) {
        return (
            <Container size="xl" py="xl">
                <Text c="red">Failed to load squads: {squadsError.message}</Text>
            </Container>
        );
    }


    return (
        <Container size="xl" py="xl">
            <Stack gap="xl">
                {/* Page Header */}
                <Group justify="space-between" align="center">
                    <Title order={1}>Your Squads</Title>
                    <Button onClick={open} size="md">
                        New Squad
                    </Button>
                </Group>

                <Box>
                    <LoadingOverlay visible={visible && !isSquadsError && !squads} loaderProps={{ children: 'Loading...' }} />
                    {/* Squads Grid */}
                    {squads?.length === 0 ? (
                        <Box ta="center" py="xl">
                            <Text size="lg" c="dimmed" mb="md">
                                No squads created yet
                            </Text>
                            <Text size="sm" c="dimmed">
                                Click "New Squad" to create your first squad
                            </Text>
                        </Box>
                    ) : (
                        <SimpleGrid
                            cols={{ base: 1, sm: 2, lg: 3 }}
                            spacing="lg"
                        >
                            {squads?.map(squad => (
                                <SquadCard key={squad.id} squadId={squad.id} name={squad.name} iconUrl={squad.iconUrl} />
                            ))}
                        </SimpleGrid>
                    )
                    }
                </Box>


                {/* Create Squad Modal */}
                <Modal opened={opened} onClose={close} title="Create Squad" centered>
                    <form onSubmit={handleCreateSquad}>
                        <Stack gap="md">
                            <TextInput
                                label="Squad Name"
                                placeholder="Enter squad name"
                                value={newSquadName}
                                onChange={(e) => setNewSquadName(e.target.value)}
                                required
                            />

                            <Stack gap="sm">
                                <Text size="sm" fw={500}>
                                    Select Squad Icon
                                </Text>
                                <Group justify="center" gap="md">
                                    {squadIconOptions.map((img, index) => {
                                        const isSelected = newSquadImage === img;
                                        return (
                                            <Box
                                                key={index}
                                                onClick={() => setNewSquadImage(img)}
                                                style={{
                                                    cursor: 'pointer',
                                                    border: isSelected ? '3px solid var(--mantine-color-blue-6)' : '2px solid var(--mantine-color-gray-6)',
                                                    borderRadius: '8px',
                                                    padding: '4px',
                                                    transition: 'all 0.2s ease',
                                                    transform: isSelected ? 'scale(1.05)' : 'scale(1)',
                                                    boxShadow: isSelected ? '0 0 0 4px rgba(34, 139, 230, 0.2)' : 'none',
                                                    opacity: isSelected ? 1 : 0.7,
                                                }}
                                                onMouseEnter={(e) => {
                                                    if (!isSelected) {
                                                        e.currentTarget.style.opacity = '1';
                                                        e.currentTarget.style.transform = 'scale(1.02)';
                                                    }
                                                }}
                                                onMouseLeave={(e) => {
                                                    if (!isSelected) {
                                                        e.currentTarget.style.opacity = '0.7';
                                                        e.currentTarget.style.transform = 'scale(1)';
                                                    }
                                                }}
                                            >
                                                <Image
                                                    src={img}
                                                    w={100}
                                                    h={100}
                                                    radius="sm"
                                                    style={{ display: 'block' }}
                                                />
                                            </Box>
                                        );
                                    })}
                                </Group>
                            </Stack>
                            <Button type="submit" loading={isCreateSquadPending} fullWidth>
                                Create Squad
                            </Button>
                        </Stack>
                    </form>
                </Modal>
            </Stack>
        </Container>
    )
}


export default Squads;
