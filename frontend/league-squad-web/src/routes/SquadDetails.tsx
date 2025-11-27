import { useQuery } from '@tanstack/react-query';
import { useParams, useNavigate } from 'react-router-dom';
import { getMatchDetails, getMatches, getSquad } from '../api/squads';
import type { SquadMatchResponse } from '../types/SquadMatchDtos';
import { useState, useEffect } from 'react';
import { Container, Grid, Stack, Card, Text, Badge, ScrollArea, Box, Title, Group, Loader, Avatar, Button, Divider } from '@mantine/core';
import { IconPlus, IconEdit, IconTrash, IconArrowLeft } from '@tabler/icons-react';
import type { SquadMatchDetailsResponse } from '../types/SquadMatchDetailsResponse';
import ParticipantCard from '../components/ParticipantCard';
import SquadMemberCard from '../components/SquadMemberCard';
import type { ParticipantStats } from '../types/ParticipantDtos';
import { getSquadMembers } from '../api/squadMembers';
import type { SquadMemberResponse } from '../types/SquadMemberDtos';
import type { SquadResponse } from '../types/SquadDtos';

const SquadDetails = () => {
    const { squadId } = useParams();
    const navigate = useNavigate();
    const [selectedMatch, setSelectedMatch] = useState<SquadMatchResponse | null>(null);
    const [selectedMatchId, setSelectedMatchId] = useState<string>("");
    const [participantsStats, setParticipantsStats] = useState<ParticipantStats[]>([]); // create type for this

    const { data: matches, isLoading: isMatchesLoading, isError: isMatchesError, error: matchesError } = useQuery<SquadMatchResponse[], Error>({
        queryKey: ["squads", squadId, "matches"],
        queryFn: () => getMatches(Number(squadId)),
        staleTime: 2 * 60 * 1000,
        refetchOnWindowFocus: false
    });

    const { data: matchDetails, refetch: refetchMatchDetails, isFetching: isMatchDetailsFetching, isLoading: isMatchDetailsLoading, isError: isMatchDetailsError, error: matchDetailsError } = useQuery<SquadMatchDetailsResponse, Error>({
        queryKey: ["squads", squadId, "matches", selectedMatchId],
        queryFn: () => getMatchDetails(Number(squadId), selectedMatchId),
        enabled: selectedMatchId != "",
        refetchOnWindowFocus: false
    });

    const { data: squad, isLoading: isSquadLoading } = useQuery<SquadResponse, Error>({
        queryKey: ["squads", squadId],
        queryFn: () => getSquad(Number(squadId))
    });

    const { data: squadMembers, isLoading: isSquadMembersLoading } = useQuery<SquadMemberResponse[], Error>({
        queryKey: ["squads", squadId, "members"],
        queryFn: () => getSquadMembers(Number(squadId))
    });


    // Set the first match as selected when data loads
    useEffect(() => {
        if (matches && matches.length > 0 && !selectedMatch) {
            setSelectedMatch(matches[0]);
            setSelectedMatchId(matches[0].matchId);
        }
    }, [matches, selectedMatch]);

    useEffect(() => {
        parseMatchDetails(JSON.parse(matchDetails?.statsJson ?? "{}"));
    }, [matchDetails]);

    const formatDate = (dateString: string) => {
        const date = new Date(dateString);
        return date.toLocaleDateString("en-CA", { month: "short", day: "numeric", year: "numeric" });
    };

    const formatTime = (dateString: string) => {
        const date = new Date(dateString);
        return date.toLocaleTimeString("en-CA", { hour: "numeric", minute: "2-digit" });
    };

    const formatDuration = (seconds: number) => {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${mins}m ${secs}s`;
    };

    const getQueueBadge = (queueId: number) => {
        const queueMap: { [key: number]: { label: string; color: string } } = {
            420: { label: 'Ranked Solo', color: 'blue' },
            440: { label: 'Ranked Flex', color: 'grape' },
            400: { label: 'Normal Draft', color: 'green' },
            430: { label: 'Normal Blind', color: 'teal' },
            450: { label: 'ARAM', color: 'orange' },
        };
        return queueMap[queueId] || { label: `Queue ${queueId}`, color: 'gray' };
    };

    // create type for raw data
    const parseMatchDetails = (matchDetails: any) => {
        console.log(matchDetails)
        console.log(matchDetails.participantsStats)
        setParticipantsStats(matchDetails.participantsStats)

        // do graph/chart stuff here

    };

    const handleMatchDetails = (match: SquadMatchResponse) => {
        setSelectedMatch(match);
        setSelectedMatchId(match.matchId);
    };

    const checkSquadMember = (puuid: string) => {
        if (squadMembers) {
            for (const squadMember of squadMembers) {
                if (squadMember.puuid === puuid) return true;
            }
        }
        return false;
    }


    if (isMatchesError) {
        return (
            <Container size="xl" py="xl">
                <Text c="red">Failed to load matches: {matchesError.message}</Text>
            </Container>
        );
    }

    if (!matches || matches.length === 0) {
        return (
            <Container size="xl" py="xl">
                <Box ta="center" py="xl">
                    <Text size="lg" c="dimmed" mb="md">
                        No match history available
                    </Text>
                    <Text size="sm" c="dimmed">
                        Matches will appear here once your squad plays together
                    </Text>
                </Box>
            </Container>
        );
    }

    return (
        <Container size="xl" py="xl">
            <Stack gap="xl">
                <Button
                    leftSection={<IconArrowLeft size={16} />}
                    variant="subtle"
                    onClick={() => navigate('/squads')}
                    style={{ alignSelf: 'flex-start' }}
                >
                    Back to Squads
                </Button>

                {/* Squad Summary Section */}
                {squad && (
                    <Card shadow="sm" padding="lg" radius="md" withBorder>
                        <Stack gap="md">
                            <Group justify="space-between" wrap="nowrap">
                                <Group gap="md">
                                    <Avatar src={squad.iconUrl} size="lg" radius="md" />
                                    <Box>
                                        <Title order={2}>{squad.name}</Title>
                                        <Group gap="xs" mt={4}>
                                            <Text size="sm" c="dimmed">
                                                Created {formatDate(squad.createdAt)}
                                            </Text>
                                            <Text size="sm" c="dimmed">•</Text>
                                            <Text size="sm" c="dimmed">
                                                {squadMembers?.length || 0} {squadMembers?.length === 1 ? 'Member' : 'Members'}
                                            </Text>
                                        </Group>
                                    </Box>
                                </Group>
                                <Group gap="xs">
                                    <Button
                                        leftSection={<IconEdit size={16} />}
                                        variant="light"
                                        size="sm"
                                    >
                                        Edit Squad
                                    </Button>
                                </Group>
                            </Group>

                            <Divider />

                            {/* Squad Members */}
                            <Stack gap="sm">
                                <Group justify="space-between">
                                    <Title order={4}>Squad Members</Title>
                                    <Button
                                        leftSection={<IconPlus size={16} />}
                                        variant="light"
                                        size="sm"
                                    >
                                        Add Member
                                    </Button>
                                </Group>

                                {isSquadMembersLoading ? (
                                    <Group justify="center" py="xl">
                                        <Loader size="sm" />
                                        <Text c="dimmed">Loading members...</Text>
                                    </Group>
                                ) : squadMembers && squadMembers.length > 0 ? (
                                    <Grid gutter="md">
                                        {squadMembers.map((member) => (
                                            <Grid.Col key={member.puuid} span={{ base: 12, sm: 6, md: 4 }}>
                                                <SquadMemberCard
                                                    member={member}
                                                    onEdit={(m) => console.log('Edit', m)}
                                                    onDelete={(m) => console.log('Delete', m)}
                                                />
                                            </Grid.Col>
                                        ))}
                                    </Grid>
                                ) : (
                                    <Box ta="center" py="xl">
                                        <Text c="dimmed">No members yet</Text>
                                        <Text size="sm" c="dimmed" mt="xs">
                                            Add members to start tracking squad performance
                                        </Text>
                                    </Box>
                                )}
                            </Stack>
                        </Stack>
                    </Card>
                )}

                {
                    isMatchesLoading ?
                        (
                            <Stack align="center" gap="md">
                                <Loader size="lg" />
                                <Text c="dimmed">Loading match history...</Text>
                            </Stack>
                        )
                        :
                        (
                            <>

                                {/* Match History Section */}
                                <Title order={2}>Match History</Title>

                                <Grid gutter="lg">
                                    <Grid.Col span={{ base: 12, md: 4 }}>
                                        <ScrollArea h={600} type="auto">
                                            <Stack gap="sm">
                                                {matches.map((match) => {
                                                    const queueInfo = getQueueBadge(match.queueId);
                                                    const isSelected = selectedMatch?.matchId === match.matchId;

                                                    return (
                                                        <Card
                                                            key={match.matchId}
                                                            padding="md"
                                                            radius="md"
                                                            withBorder
                                                            style={{
                                                                cursor: 'pointer',
                                                                backgroundColor: isSelected ? 'var(--mantine-color-dark-6)' : undefined,
                                                                borderColor: isSelected ? 'var(--mantine-color-blue-6)' : undefined,
                                                            }}
                                                            onClick={() => handleMatchDetails(match)}
                                                        >
                                                            <Stack gap="xs">
                                                                {/* Date & Time */}
                                                                <Group justify="space-between">
                                                                    <Text size="sm" fw={500}>
                                                                        {formatDate(match.gameStart)}
                                                                    </Text>
                                                                    <Text size="xs" c="dimmed">
                                                                        {formatTime(match.gameStart)}
                                                                    </Text>
                                                                </Group>

                                                                {/* Queue Type */}
                                                                <Badge color={queueInfo.color} variant="light" size="sm">
                                                                    {queueInfo.label}
                                                                </Badge>

                                                                {/* Game Mode & Duration */}
                                                                <Group justify="space-between">
                                                                    <Text size="xs" c="dimmed">
                                                                        {match.mode}
                                                                    </Text>
                                                                    <Text size="xs" c="dimmed">
                                                                        {formatDuration(match.durationSeconds)}
                                                                    </Text>
                                                                </Group>
                                                            </Stack>
                                                        </Card>
                                                    );
                                                })}
                                            </Stack>
                                        </ScrollArea>
                                    </Grid.Col>

                                    {/* Match Details */}
                                    <Grid.Col span={{ base: 12, md: 8 }}>
                                        {selectedMatch ? (
                                            <Stack gap="md">
                                                <Card padding="md" radius="md" withBorder>
                                                    <Stack gap="sm">
                                                        <Group justify="space-between">
                                                            <Title order={3}>Match Details</Title>
                                                            {isMatchDetailsFetching && <Loader size="sm" />}
                                                        </Group>

                                                        <Grid gutter="xs">
                                                            <Grid.Col span={6}>
                                                                <Card padding="xs" radius="sm" withBorder>
                                                                    <Text size="xs" c="dimmed" mb={4}>Queue</Text>
                                                                    <Badge
                                                                        color={getQueueBadge(selectedMatch.queueId).color}
                                                                        variant="light"
                                                                        size="sm"
                                                                    >
                                                                        {getQueueBadge(selectedMatch.queueId).label}
                                                                    </Badge>
                                                                </Card>
                                                            </Grid.Col>
                                                            <Grid.Col span={6}>
                                                                <Card padding="xs" radius="sm" withBorder>
                                                                    <Text size="xs" c="dimmed" mb={4}>Duration</Text>
                                                                    <Text size="sm" fw={500}>
                                                                        {formatDuration(selectedMatch.durationSeconds)}
                                                                    </Text>
                                                                </Card>
                                                            </Grid.Col>
                                                            <Grid.Col span={6}>
                                                                <Card padding="xs" radius="sm" withBorder>
                                                                    <Text size="xs" c="dimmed" mb={4}>Mode</Text>
                                                                    <Text size="sm" fw={500}>
                                                                        {selectedMatch.mode}
                                                                    </Text>
                                                                </Card>
                                                            </Grid.Col>
                                                            <Grid.Col span={6}>
                                                                <Card padding="xs" radius="sm" withBorder>
                                                                    <Text size="xs" c="dimmed" mb={4}>Date</Text>
                                                                    <Text size="sm" fw={500}>
                                                                        {formatDate(selectedMatch.gameStart)}
                                                                    </Text>
                                                                </Card>
                                                            </Grid.Col>
                                                        </Grid>
                                                    </Stack>
                                                </Card>

                                                {/* Participants List */}
                                                <Card padding="md" radius="md" withBorder>
                                                    <Stack gap="sm">
                                                        <Title order={4}>Player Performance</Title>
                                                        <ScrollArea h={450} type="auto" offsetScrollbars>
                                                            <Stack gap="sm" pr="xs">
                                                                {participantsStats && participantsStats.length > 0 ? (
                                                                    participantsStats.map((pStats: ParticipantStats) => (
                                                                        <ParticipantCard
                                                                            key={pStats.puuid}
                                                                            stats={pStats}
                                                                            isSquadMember={checkSquadMember(pStats.puuid)}
                                                                        />
                                                                    ))
                                                                ) : (
                                                                    <Text c="dimmed" ta="center" py="xl">
                                                                        {isMatchDetailsFetching ? 'Loading participant stats...' : 'No participant data available'}
                                                                    </Text>
                                                                )}
                                                            </Stack>
                                                        </ScrollArea>
                                                    </Stack>
                                                </Card>
                                            </Stack>
                                        ) : (
                                            <Card padding="lg" radius="md" withBorder h={600}>
                                                <Stack align="center" justify="center" h="100%">
                                                    <Text c="dimmed" ta="center">
                                                        Select a match to view details
                                                    </Text>
                                                </Stack>
                                            </Card>
                                        )}
                                    </Grid.Col>
                                </Grid>
                            </>
                        )
                }

            </Stack>
        </Container>
    );
}

export default SquadDetails
