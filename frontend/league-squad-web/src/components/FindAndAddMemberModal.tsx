import { useDisclosure, type UseDisclosureHandlers } from '@mantine/hooks';
import React, { useEffect, useState } from 'react'
import { queryClient } from '../lib/queryClient';
import { notifications } from '@mantine/notifications';
import { Box, Button, Divider, Modal, Stack, TextInput, Text, Select } from '@mantine/core';
import { addSquadMember } from '../api/squadMembers';
import type { AddSquadMemberRequest, SquadMemberResponse } from '../types/SquadMemberDtos';
import { getRiotAccountUsingRiotId } from '../api/riotAccounts';
import type { RiotAccountResponse } from '../types/RiotAccountDtos';
import { useMutation, useQuery } from '@tanstack/react-query';

type FindAndAddMemberModalParamsTypes = {
    squadId: number,
    opened: boolean,
    handlers: UseDisclosureHandlers
};

const FindAndAddMemberModal = ({ squadId, opened, handlers }: FindAndAddMemberModalParamsTypes ) => {
    const [findGameName, setFindGameName] = useState("");
    const [findTagline, setFindTagline] = useState("");
    const [riotAccountPuuid, setRiotAccountPuuid] = useState("");
    const [role, setRole] = useState("");
    const [alias, setAlias] = useState("");

    // find a riot account
    const { data: riotAccount, refetch: refetchRiotAccount, isLoading: isRiotAccountLoading, isError: isRiotAccountError, error: riotAccountError } = useQuery<RiotAccountResponse, Error>({
        queryKey: ["riot-account", findGameName, findTagline],
        queryFn: () => getRiotAccountUsingRiotId(findGameName, findTagline),
        enabled: false,
    });

    // add a squad member 
    const { data: squadMember, mutate: addSquadMemberMutate, isPending: isAddSquadMemberPending, isError: isAddSquadMemberError, error: addSquadMemberError } = useMutation<SquadMemberResponse, Error, AddSquadMemberRequest>({
        mutationFn: (req: AddSquadMemberRequest) => addSquadMember(Number(squadId), req),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["squads"], exact: true});
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "members"]});
            // await queryClient.fetchQuery({
            //     queryKey: ["squads", squadId, "matches"],
            //     queryFn: () => getMatches(Number(squadId), true)
            // })
            queryClient.invalidateQueries({ queryKey: ["squads", squadId, "matches"]});
            handlers.close();
            notifications.show({
                title: 'Member Added!',
                message: 'Squad member has been successfully added',
                color: 'green',
            });
        },
        onError: (error: Error) => {
            console.log(error.message);
            notifications.show({
                title: 'Error',
                message: `Failed to add member: ${error.message}`,
                color: 'red',
            });
        },
    });


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


    return (
        <Modal opened={opened} onClose={handlers.close} title="Find & Add Member" centered size="md">
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
    )
}

export default FindAndAddMemberModal;