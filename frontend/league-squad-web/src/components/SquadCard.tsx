import { Card, Box, Flex, Text, Image, Group, Button, Modal, Divider, Stack, Badge, TextInput, Select, Collapse, ActionIcon } from "@mantine/core"
import { notifications } from '@mantine/notifications';
import { IconChevronDown, IconChevronUp, IconMug } from '@tabler/icons-react'
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
import { Link } from "react-router-dom";
import { squadIconOptions } from "../assets/squadIconOptions";

type SquadCardTypes = {
    squadId: number
    name: string;
    iconUrl: string;
}

const SquadCard = ({ squadId, name, iconUrl }: SquadCardTypes) => {
    const [opened, { open, close }] = useDisclosure(false);
    const [finderOpened, finderHandlers] = useDisclosure(false);
    const [showMembers, setShowMembers] = useState(false);
    const [updatedSquadName, setUpdatedSquadName] = useState(name);
    const [updatedSquadIcon, setUpdatedSquadIcon] = useState(iconUrl);
    const [findGameName, setFindGameName] = useState("");
    const [findTagline, setFindTagline] = useState("");
    const [riotAccountPuuid, setRiotAccountPuuid] = useState("");
    const [riotAccountGamename, setRiotAccountGamename] = useState("");
    const [riotAccountTagline, setRiotAccountTagline] = useState("");
    const [riotAccoutRegion, setRiotAccountRegion] = useState("");
    const [role, setRole] = useState("");
    const [alias, setAlias] = useState("");

    const [isSquadFull, setIsSquadFull] = useState(false);
    const [isSquadEmpty, setIsSquadEmpty] = useState(true);

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
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "members"] });
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "matches"], refetchType: "active" });
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
                    fallbackSrc="https://i.pinimg.com/736x/bd/4b/b9/bd4bb91d737b74f25c7ff5ab46001b9b.jpg"
                    src={(iconUrl === "" ? null : iconUrl)}
                    height={180}
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
                    <Button variant="outline" onClick={open}>
                        Update
                    </Button>
                </Group>

                <Button onClick={handleDeleteSquad} variant="subtle" color="red" fullWidth>
                    Delete Squad
                </Button>
            </Stack>

            <Modal opened={opened} onClose={close} title="Update Squad" centered>
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

            <Modal opened={finderOpened} onClose={finderHandlers.close} title="Find & Add Member" centered size="md">
                <Stack gap="lg">
                    {/* Find Player Form */}
                    <form onSubmit={handleFindPlayer}>
                        <Stack gap="md">
                            <TextInput
                                label="Game Name"
                                placeholder="Enter Riot ID game name"
                                value={findGameName}
                                onChange={(e) => setFindGameName(e.target.value)}
                                required
                            />
                            <TextInput
                                label="Tagline"
                                placeholder="Enter tagline (e.g., NA1)"
                                value={findTagline}
                                onChange={(e) => setFindTagline(e.target.value)}
                                required
                            />
                            <Button type="submit" loading={isRiotAccountLoading} fullWidth>
                                Find Player
                            </Button>
                        </Stack>
                    </form>
                    {/* Found Player & Add Member Form */}
                    {riotAccount && (
                        <Box>
                            <Divider mb="md" />
                            <Stack gap="md">
                                <Box>
                                    <Text fw={600} mb="xs">Found Player:</Text>
                                    <Text size="sm">Game Name: {riotAccount?.gameName}</Text>
                                    <Text size="sm">Tagline: {riotAccount?.tagLine}</Text>
                                    <Text size="sm">Region: {riotAccount?.region}</Text>
                                </Box>

                                <form onSubmit={handleAddMember}>
                                    <Stack gap="md">
                                        <Select
                                            label="Role (Optional)"
                                            placeholder="Select role"
                                            value={role}
                                            onChange={(value) => setRole(value || "")}
                                            data={[
                                                { value: 'top', label: 'Top' },
                                                { value: 'jungle', label: 'Jungle' },
                                                { value: 'middle', label: 'Middle' },
                                                { value: 'bottom', label: 'Bottom' },
                                                { value: 'support', label: 'Support' }
                                            ]}
                                        />
                                        <TextInput
                                            label="Alias (Optional)"
                                            placeholder="Enter player alias/nickname"
                                            value={alias}
                                            onChange={(e) => setAlias(e.target.value)}
                                        />
                                        <Button type="submit" loading={isAddSquadMemberPending} fullWidth>
                                            Add to Squad
                                        </Button>
                                    </Stack>
                                </form>
                            </Stack>
                        </Box>
                    )}

                    {/* Error Message */}
                    {!riotAccount && isRiotAccountError && (
                        <Text c="red" size="sm">
                            {riotAccountError.message}
                        </Text>
                    )}
                </Stack>
            </Modal>



        </Card>
    )
}

export default SquadCard
