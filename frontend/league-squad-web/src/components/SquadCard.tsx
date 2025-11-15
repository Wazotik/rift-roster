import { Card, Box, Flex, Text, Image, Group, Button, Modal, Divider } from "@mantine/core"
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import type { SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import { useMutation, usePrefetchQuery, useQuery, useQueryClient } from "@tanstack/react-query";
import { deleteSquad, updateSquad } from "../api/squads";
import { addSquadMember, getSquadMembers } from "../api/squadMembers";
import type { AddSquadMemberRequest, SquadMemberResponse } from "../types/SquadMemberDtos";
import { apiGet } from "../lib/apiClient";
import { getRiotAccountUsingRiotId } from "../api/riotAccounts";
import type { RiotAccountResponse } from "../types/RiotAccountDtos";

type SquadCardTypes = {
    squadId: number
    name: string;
}

const SquadCard = ({ squadId, name }: SquadCardTypes) => {
    const [opened, { open, close }] = useDisclosure(false);
    const [finderOpened, finderHandlers] = useDisclosure(false);
    const [updatedSquadName, setUpdatedSquadName] = useState(name);
    const [findGameName, setFindGameName] = useState("");
    const [findTagline, setFindTagline] = useState("");
    const [riotAccountPuuid, setRiotAccountPuuid] = useState("");
    const [riotAccountGamename, setRiotAccountGamename] = useState("");
    const [riotAccountTagline, setRiotAccountTagline] = useState("");
    const [riotAccoutRegion, setRiotAccountRegion] = useState("");
    const [role, setRole] = useState("");
    const [alias, setAlias] = useState("");

    const [isSquadFull, setIsSquadFull] = useState(false);

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

    // get all squad members

    const { data: squadMembers, isLoading: isSquadMembersLoading, isError: isSquadMembersError, error: squadMembersError } = useQuery<SquadMemberResponse[], Error>({
        queryKey: ["squads", squadId, "members"],
        queryFn: () => getSquadMembers(squadId)
    });

    // add a squad member (find then add)

    // find a riot account
    const { data: riotAccount, refetch: refetchRiotAccount, isLoading: isRiotAccountLoading, isError: isRiotAccountError, error: riotAccountError } = useQuery<RiotAccountResponse, Error>({
        queryKey: ["riot-account", findGameName, findTagline],
        queryFn: () => getRiotAccountUsingRiotId(findGameName, findTagline),
        enabled: false,
    });

    // add a squad member 
    const { data: squadMember, mutate: addSquadMemberMutate, isPending: isAddSquadMemberPending, isError: isAddSquadMemberError, error: addSquadMemberError } = useMutation<SquadMemberResponse, Error, AddSquadMemberRequest>({
        // mutationFn: ({ id, req }) => addSquadMember(squadId, req),
        mutationFn: (req) => addSquadMember(squadId, req),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "members"] });
            finderHandlers.close();
        },
        onError: (e) => {
            console.log(e.message);
        },
    });


    // delete a squad member



    const handleUpdateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("updating squad ", squadId);
        updateSquadMutate({ id: squadId, req: { name: updatedSquadName } });
    }

    const handleDeleteSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("deleting squad ", squadId);
        deleteSquadMutate(squadId);
    }

    const handleFindPlayer = (e) => {
        e.preventDefault();
        refetchRiotAccount();
    };

    const handleAddMember = (e) => {
        e.preventDefault();
        console.log(role);
        console.log(alias);
        addSquadMemberMutate({ puuid: riotAccountPuuid, role: role, alias: alias });
    };

    useEffect(() => {
        if (riotAccount) setRiotAccountPuuid(riotAccount.puuid);
    }, [riotAccount])

    useEffect(() => {
        if (squadMembers && squadMembers.length === 5) setIsSquadFull(true);
    }, [squadMembers])

    // useEffect(() => {
    //     if (squadMembers && squadMembers.length === 5) setIsSquadFull(true);
    // }, [])

    if (!squadMembers || isSquadMembersError) {
        return (
            <div>squad member error</div>
        )
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

            <Box mt="md" mb="xs">
                {
                    squadMembers.length !== 0 ?
                        (
                            squadMembers.map((sm: SquadMemberResponse) =>
                            (
                                <div key={sm.puuid}>
                                    <Text>{sm.gameName}</Text>
                                </div>
                            )
                            )
                        )
                        :
                        (
                            <div>no squad members</div>
                        )
                }
            </Box>

            <Group mt="md">
                <Group justify="flex-start">
                    <Button variant="filled" color="blue" onClick={open}>
                        update
                    </Button>
                </Group>
                <Group justify="flex-end">
                    <Button onClick={handleDeleteSquad} variant="filled" color="red">
                        delete
                    </Button>
                </Group>

                <Group justify="flex-start">
                    <Button variant="filled" color="blue" onClick={finderHandlers.open} disabled={isSquadFull}>
                        add member
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

            <Modal opened={finderOpened} onClose={finderHandlers.close} title="find + add member" centered>
                <Box>
                    <form
                        onSubmit={handleFindPlayer}
                    >
                        <Flex direction="column">
                            <label htmlFor="gamename">gamename</label>
                            <input type="text" onChange={(e) => setFindGameName(e.target.value)} value={findGameName} id="findGameName" required />
                            <label htmlFor="tagline">tagline</label>
                            <input type="text" onChange={(e) => setFindTagline(e.target.value)} value={findTagline} id="findTagline" required />
                            <Button mt="lg" type="submit" loading={isRiotAccountLoading}>
                                find
                            </Button>
                        </Flex>
                    </form>
                </Box>
                {
                    riotAccount &&
                    (
                        <Box mt={"lg"} mb={"lg"}>
                            <Box mt={"lg"} mb={"lg"}>
                                <Text><b>found player:</b></Text>
                                <Text>gamename: {riotAccount?.gameName}</Text>
                                <Text>tagline: {riotAccount?.tagLine}</Text>
                                <Text>region: {riotAccount?.region}</Text>
                            </Box>
                            <Box>
                                <form
                                    onSubmit={handleAddMember}
                                >
                                    <Flex direction="column">
                                        <label htmlFor="role">role</label>
                                        <select name="role" id="role" value={role} onChange={(e) => setRole(e.target.value)}>
                                            <option hidden disabled value=""></option>
                                            <option value="top">Top</option>
                                            <option value="jungle">Jungle</option>
                                            <option value="middle">Middle</option>
                                            <option value="bottom">Bottom</option>
                                            <option value="support">Support</option>
                                        </select>
                                        <label htmlFor="alias">alias</label>
                                        <input type="text" onChange={(e) => setAlias(e.target.value)} value={alias} id="alias" />
                                        <Button mt="lg" type="submit" loading={isAddSquadMemberPending}>
                                            add
                                        </Button>
                                    </Flex>
                                </form>
                            </Box>
                        </Box>
                    )
                }
                {
                    (!riotAccount && isRiotAccountError) &&
                    (
                        <div>
                            <Text>{riotAccountError.message}</Text>
                        </div>
                    )
                }

            </Modal>



        </Card>
    )
}

export default SquadCard