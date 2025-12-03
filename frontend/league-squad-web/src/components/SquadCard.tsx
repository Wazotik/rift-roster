import { Card, Box, Text, Image, Group, Button, Modal, Divider, Stack, Badge, TextInput, Select, Collapse } from "@mantine/core"
import { notifications } from '@mantine/notifications';
import { IconChevronDown, IconChevronUp } from '@tabler/icons-react'
import { useDisclosure } from "@mantine/hooks";
import { useEffect, useState } from "react";
import type { SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { deleteSquad, getMatches, getSquad, updateSquad } from "../api/squads";
import { addSquadMember, getSquadMembers } from "../api/squadMembers";
import type { AddSquadMemberRequest, SquadMemberResponse } from "../types/SquadMemberDtos";
import { getRiotAccountUsingRiotId } from "../api/riotAccounts";
import type { RiotAccountResponse } from "../types/RiotAccountDtos";
import { Link } from "react-router-dom";
import { squadIconOptions } from "../assets/squadIconOptions";
import FindAndAddMemberModal from "./FindAndAddMemberModal";
import UpdateSquadModal from "./UpdateSquadModal";

type SquadCardTypes = {
    squadId: number
    name: string;
    iconUrl: string;
}

const SquadCard = ({ squadId, name, iconUrl }: SquadCardTypes) => {
    const [opened, { open, close }] = useDisclosure(false);
    const [finderOpened, finderHandlers] = useDisclosure(false);
    const [updaterOpened, updaterHandlers] = useDisclosure(false);
    const [showMembers, setShowMembers] = useState(false);
    const [updatedSquadName, setUpdatedSquadName] = useState(name);
    const [updatedSquadIcon, setUpdatedSquadIcon] = useState(iconUrl);
    const [findGameName, setFindGameName] = useState("");
    const [findTagline, setFindTagline] = useState("");
    const [riotAccountPuuid, setRiotAccountPuuid] = useState("");
    const [role, setRole] = useState("");
    const [alias, setAlias] = useState("");

    const [isSquadFull, setIsSquadFull] = useState(false);
    const [isSquadEmpty, setIsSquadEmpty] = useState(true);

    const queryClient = useQueryClient();

    const { data: squad, isLoading: isSquadLoading } = useQuery<SquadResponse, Error>({
        queryKey: ["squads", squadId],
        queryFn: () => getSquad(Number(squadId))
    });

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

    // Delete Squad
    const { data, mutate: deleteSquadMutate, isPending: isDeleteSquadPending, isSuccess: isDeleteSquadSuccess, isError: isDeleteSquadError, error: deleteSquadError } = useMutation<number, Error, number>({
        mutationFn: deleteSquad,
        onSuccess: (data, variables) => {
            queryClient.invalidateQueries({ queryKey: ["squads"] });
            notifications.show({
                title: 'Squad Deleted',
                message: 'Squad has been successfully deleted',
                color: 'blue',
            });
        },
        onError: (error) => {
            console.log("delete squad error", error.message);
            notifications.show({
                title: 'Error',
                message: `Failed to delete squad: ${error.message}`,
                color: 'red',
            });
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
        mutationFn: (req) => addSquadMember(squadId, req),
        onSuccess: async () => {
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "members"] });
            // await queryClient.fetchQuery({
            //     queryKey: ["squads", squadId, "matches"],
            //     queryFn: () => getMatches(Number(squadId), true)
            // })
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "matches"]});
            finderHandlers.close();
            notifications.show({
                title: 'Member Added!',
                message: 'Squad member has been successfully added',
                color: 'green',
            });
        },
        onError: (error) => {
            console.log(error.message);
            notifications.show({
                title: 'Error',
                message: `Failed to add member: ${error.message}`,
                color: 'red',
            });
        },
    });


    // delete a squad member



    const handleUpdateSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("updating squad ", squadId);
        updateSquadMutate({ id: squadId, req: { name: updatedSquadName, iconUrl: updatedSquadIcon } });
    }

    const handleDeleteSquad = (e: React.FormEvent) => {
        e.preventDefault();
        console.log("deleting squad ", squadId);
        deleteSquadMutate(squadId);
    }

    const handleFindPlayer = (e: React.FormEvent) => {
        e.preventDefault();
        refetchRiotAccount();
    };

    const handleAddMember = (e: React.FormEvent) => {
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

    useEffect(() => {
        if (squadMembers && squadMembers.length > 0 && squadMembers.length <= 5) setIsSquadEmpty(false);
    }, [squadMembers])

    if (!squadMembers || isSquadMembersError) {
        return (
            <div>squad member error</div>
        )
    }

    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Card.Section>
                <Image
                    fallbackSrc="https://res.cloudinary.com/dm6esbywi/image/upload/v1764755909/Ravenous_Poro_by_AxlRosie_on_DeviantArt_hkyad2.jp"
                    src={(iconUrl === "" ? null : iconUrl)}
                    height={250}
                    alt="squad-image"
                />
            </Card.Section>

            <Stack gap="md" mt="md">
                {/* Squad Name & Member Count */}
                <Group justify="space-between" align="center">
                    <Text fw={600} size="lg">{name}</Text>
                    <Badge color={isSquadFull ? "green" : "blue"} variant="light">
                        {squadMembers.length}/5
                    </Badge>
                </Group>

                {/* Show/Hide Members Toggle */}
                <Button
                    variant="subtle"
                    onClick={() => setShowMembers(!showMembers)}
                    rightSection={showMembers ? <IconChevronUp size={16} /> : <IconChevronDown size={16} />}
                    fullWidth
                >
                    {showMembers ? 'Hide Members' : 'Show Members'}
                </Button>

                {/* Collapsible Member List */}
                <Collapse in={showMembers}>
                    <Stack gap="xs">
                        {squadMembers.length !== 0 ? (
                            squadMembers.map((sm: SquadMemberResponse) => (
                                <Box key={sm.puuid}>
                                    <Text size="sm">
                                        {sm.gameName}
                                        {/* fix this: don't manually check for "string" and empty string */}
                                        {sm.alias !== "string" && sm.alias !== "" && (
                                            <Text span c="dimmed" size="xs" ml="xs">
                                                ({sm.alias})
                                            </Text>
                                        )}
                                    </Text>
                                </Box>
                            ))
                        ) : (
                            <Text size="sm" c="dimmed" ta="center">
                                No squad members yet
                            </Text>
                        )}
                    </Stack>
                </Collapse>

                <Divider />

                <Button variant="light" component={isSquadEmpty ? undefined : Link} to={`/squads/${squadId}`} disabled={isSquadEmpty} fullWidth>
                    View Details
                </Button>

                <Group grow>
                    <Button variant="filled" onClick={finderHandlers.open} disabled={isSquadFull}>
                        Add Member
                    </Button>
                    <Button variant="outline" onClick={updaterHandlers.open}>
                        Update
                    </Button>
                </Group>

                <Button onClick={handleDeleteSquad} variant="subtle" color="red" fullWidth>
                    Delete Squad
                </Button>
            </Stack>

            {squadId && <FindAndAddMemberModal squadId={squadId} opened={finderOpened} handlers={finderHandlers} />}
            {squad && squadId && <UpdateSquadModal squadId={squadId} opened={updaterOpened} handlers={updaterHandlers} squad={squad} />}
        </Card>
    )
}

export default SquadCard
