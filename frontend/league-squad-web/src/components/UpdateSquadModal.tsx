import { Modal, Stack, TextInput, Text, Image, Button, Group, Box } from '@mantine/core';
import { notifications } from '@mantine/notifications';
import React, { useState } from 'react'
import { queryClient } from '../lib/queryClient';
import { updateSquad } from '../api/squads';
import type { SquadResponse, UpdateSquadRequest } from '../types/SquadDtos';
import type { UseDisclosureHandlers } from '@mantine/hooks';
import { squadIconOptions } from '../assets/squadIconOptions';
import { useMutation } from '@tanstack/react-query';

type FullUpdateSquadRequest = {
    id: number;
    req: UpdateSquadRequest;
};

type UpdateSquadModalParamsTypes = {
    squadId: number,
    opened: boolean,
    handlers: UseDisclosureHandlers,
    squad: SquadResponse 
};

const UpdateSquadModal = ({ squadId, opened, handlers, squad }: UpdateSquadModalParamsTypes) => {
    const [updatedSquadName, setUpdatedSquadName] = useState(squad.name);
    const [updatedSquadIcon, setUpdatedSquadIcon] = useState(squad.iconUrl);

    // Update Squad
    const { data: updatedSquad, mutate: updateSquadMutate, isPending: isUpdateSquadPending, isSuccess: isUpdateSquadSuccess, isError: isUpdateSquadError, error: updateSquadError } = useMutation<SquadResponse, Error, FullUpdateSquadRequest>({
        mutationFn: ({ id, req }) => updateSquad(id, req),
        onSuccess: (data, variables) => {
            console.log("updated squad", data, "from vars", variables);
            // queryClient.invalidateQueries({ queryKey: ["squads"] });
            queryClient.invalidateQueries({ queryKey: ["squads"], exact: true });
            queryClient.invalidateQueries({ queryKey: ["squads", squadId], exact: true });
            console.log("squad updated");
            handlers.close();
            notifications.show({
                title: 'Success!',
                message: 'Squad updated successfully',
                color: 'green',
            });
        },
        onError: (error) => {
            console.log("update squad error", error.message);
            notifications.show({
                title: 'Error',
                message: `Failed to update squad: ${error.message}`,
                color: 'red',
            });
        }
    });

    const handleUpdateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("updating squad ", squadId);
        updateSquadMutate({ id: Number(squadId), req: { name: updatedSquadName, iconUrl: updatedSquadIcon } });
    }

    return (
        <Modal opened={opened} onClose={handlers.close} title="Update Squad" centered>
            <form onSubmit={handleUpdateSquad}>
                <Stack gap="md">
                    <TextInput
                        label="Squad Name"
                        placeholder="Enter squad name"
                        value={updatedSquadName}
                        onChange={(e) => setUpdatedSquadName(e.target.value)}
                        required
                    />
                    <Stack gap="sm">
                        <Text size="sm" fw={500}>
                            Select Squad Icon
                        </Text>
                        <Group justify="center" gap="md">
                            {squadIconOptions.map((img, index) => {
                                const isSelected = updatedSquadIcon === img;
                                return (
                                    <Box
                                        key={index}
                                        onClick={() => setUpdatedSquadIcon(img)}
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
                    <Button type="submit" fullWidth>
                        Update Squad
                    </Button>
                </Stack>
            </form>
        </Modal>
    )
}

export default UpdateSquadModal;