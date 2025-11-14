import { Card, Box, Flex, Text, Image, Group, Button, Modal } from "@mantine/core"
import { useDisclosure } from "@mantine/hooks";
import { useState } from "react";
import type { SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteSquad, updateSquad } from "../api/squads";

type SquadCardTypes = {
    squadId: number
    name: string;
}

const SquadCard = ({ squadId, name }: SquadCardTypes) => {
    const [opened, { open, close }] = useDisclosure(false);
    const [updatedSquadName, setUpdatedSquadName] = useState(name);

    const queryClient = useQueryClient();

    type FullUpdateSquadRequest = {
        id: number;
        req: UpdateSquadRequest;
    };

    // Update Squad
    const { data: updatedSquad, mutate: updateSquadMutate, isPending: isUpdateSquadPending, isSuccess: isUpdateSquadSuccess, isError: isUpdateSquadError, error: updateSquadError } = useMutation<SquadResponse, Error, FullUpdateSquadRequest>({
        mutationFn: ({ id, req }) => updateSquad(id, req),
        onSuccess: (data, variables) => {
            console.log("updated squad", data, "from vars", variables);
            queryClient.invalidateQueries({ queryKey: ["squads"] });
            console.log("squad updated");
            close();
        },
        onError: () => {
            console.log("update squad error", (isUpdateSquadError ? updateSquadError.message : ""));
        }
    });

    // Delete Squad
    const { data, mutate: deleteSquadMutate, isPending: isDeleteSquadPending, isSuccess: isDeleteSquadSuccess, isError: isDeleteSquadError, error: deleteSquadError } = useMutation<number, Error, number>({
        mutationFn: deleteSquad,
        onSuccess: (data, variables) => {
            queryClient.invalidateQueries({ queryKey: ["squads"] });
        },
        onError: () => {
            console.log("delete squad error", (isDeleteSquadError ? deleteSquadError.message : ""));
        }
    });

    const handleUpdateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("updating squad ", squadId);
        updateSquadMutate({ id: squadId, req: { name: updatedSquadName } });
    }

    const handleDeleteSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("deleting squad ", squadId);
        deleteSquadMutate( squadId );
    }


    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Card.Section>
                <Image
                    src="https://i.pinimg.com/736x/bd/4b/b9/bd4bb91d737b74f25c7ff5ab46001b9b.jpg"
                    height={350}
                    alt="squad-image"
                />
            </Card.Section>

            <Box mt="md" mb="xs">
                <Text fw={600}>{name}</Text>
            </Box>

            <Group mt="md">
                <Group justify="flex-start">
                    <Button variant="filled" color="blue" onClick={open}>
                        Update
                    </Button>
                </Group>
                <Group justify="flex-end">
                    <Button onClick={handleDeleteSquad} variant="filled" color="red">
                        Delete
                    </Button>
                </Group>
            </Group>

            <Modal opened={opened} onClose={close} title="update squad" centered>
                <Box>
                    <form
                        onSubmit={handleUpdateSquad}
                    >
                        <Flex direction="column">
                            <label htmlFor="updateSquadName">Name</label>
                            <input type="text" onChange={(e) => setUpdatedSquadName(e.target.value)} value={updatedSquadName} id="updateSquadName" required />
                            <Button mt="lg" type="submit">
                                update
                            </Button>
                        </Flex>
                    </form>
                </Box>
            </Modal>

        </Card>
    )
}

export default SquadCard