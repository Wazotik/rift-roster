import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createSquad, getAllSquads, getSquad, updateSquad } from "../api/squads";
import type { CreateSquadRequest, SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import { Flex, Loader, Container, Box, Button, Modal, TextInput } from "@mantine/core";
import SquadCard from "../components/SquadCard";
import { useDisclosure } from "@mantine/hooks";
import { useForm } from '@mantine/form';
import { useEffect, useState } from "react";


const Squads = () => {
    const [opened, { open, close }] = useDisclosure(false);
    const [newSquadName, setNewSquadName] = useState("");

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
        },
        onError: () => {
            console.log("create squad error!", (isCreateSquadError ? createSquadError.message : ""));
        },
    });

    const handleCreateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        createSquadMutate({
            name: newSquadName
        });
    }

    if (isSquadsError) {
        return (<div>Failed: {squadsError.message}</div>)
    };

    if (!squads || squads.length === 0) {
        return <div>No squads created</div>
    };


    return (
        <>
            <Container size="xl" p="lg">
                <Box>
                    <Flex gap="md" wrap="wrap" direction="row" justify="flex-start" align="flex-start">
                        {squads.map(squad => (
                            <SquadCard key={squad.id} squadId={squad.id} name={squad.name} />
                        ))}
                    </Flex>
                </Box>



                <Modal opened={opened} onClose={close} title="create squad" centered>
                    <Box>
                        <form
                            // onSubmit={createSquadForm.onSubmit(handleCreateSquad)}
                            onSubmit={handleCreateSquad}
                        >
                            <Flex direction="column">
                                <label htmlFor="newSquadName">Name</label>
                                <input type="text" onChange={(e) => setNewSquadName(e.target.value)} value={newSquadName} id="newSquadName" required />
                                <Button mt="lg" type="submit" loading={isCreateSquadPending}>
                                    create
                                </Button>
                            </Flex>
                        </form>
                    </Box>
                </Modal>

                <Button variant="default" onClick={open}>
                    new squad
                </Button>
            </Container>
        </>
    )
}


export default Squads;