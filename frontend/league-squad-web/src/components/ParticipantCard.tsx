import { Card, Group, Stack, Text, Badge, Divider, Grid, Box, Tooltip, Tabs } from "@mantine/core";
import { IconSword, IconEye, IconTrophy, IconCoins, IconTarget } from "@tabler/icons-react";
import type { ParticipantStats } from "../types/ParticipantDtos";

type ParticipantCardTypes = {
    stats: ParticipantStats;
    isSquadMember: boolean;
};

const ParticipantCard = ({ stats, isSquadMember = false }: ParticipantCardTypes) => {
    // Use pre-calculated KDA from challenges
    const kda = stats.challenges.kda;

    // Get KDA color
    const getKdaColor = (kdaValue: number) => {
        if (kdaValue >= 5) return 'grape';
        if (kdaValue >= 3) return 'blue';
        if (kdaValue >= 2) return 'cyan';
        if (kdaValue >= 1) return 'gray';
        return 'red';
    };

    const formatPercentage = (num: number) => {
        return `${(num * 100).toFixed(0)}%`;
    };

    const formatRole = (role: string) => {
        return role === 'UTILITY' ? 'SUPPORT' : role;
    };

    return (
        <Card 
            shadow="sm" 
            padding="sm" 
            radius="md" 
            withBorder
            style={{
                borderColor: stats.win 
                    ? 'var(--mantine-color-green-6)' 
                    : 'var(--mantine-color-red-6)',
                borderWidth: isSquadMember ? '3px' : '1px',
                borderStyle: isSquadMember ? 'solid' : undefined,
                backgroundColor: isSquadMember 
                    ? 'var(--mantine-color-dark-6)' 
                    : undefined
            }}
        >
            <Stack gap="xs">
                {/* Header: Champion + Role + KDA */}
                <Group justify="space-between" wrap="nowrap">
                    <Group gap="sm" style={{ flex: 1 }}>
                        <Box>
                            <Text fw={600} size="md">{stats.riotIdGameName} #{stats.riotIdTagline}</Text>
                            <Text size="xs" c="dimmed">{stats.championName}</Text>
                            <Group gap={4} mt={"sm"}>
                                <Badge size="md" variant="light" color={stats.win ? 'green' : 'red'}>
                                    {stats.win ? 'Victory' : 'Defeat'}
                                </Badge>
                                {stats.teamPosition && (
                                    <Badge size="md" variant="filled" color="gray">
                                        {formatRole(stats.teamPosition.toUpperCase())}
                                    </Badge>
                                )}
                                {isSquadMember && (
                                    <Badge size="sm" variant="filled" color="blue">
                                        Squad
                                    </Badge>
                                )}
                            </Group>
                        </Box>
                    </Group>
                    
                    {/* KDA Display from challenges */}
                    <Tooltip label={`${stats.kills}/${stats.deaths}/${stats.assists}`}>
                        <Badge 
                            size="lg" 
                            variant="filled" 
                            color={getKdaColor(kda)}
                            style={{ minWidth: '60px' }}
                        >
                            {kda.toFixed(2)} KDA
                        </Badge>
                    </Tooltip>
                </Group>

                <Divider />

                {/* Core Stats Grid */}
                <Grid gutter="xs">
                    <Grid.Col span={6}>
                        <Stack gap={4}>
                            <Group gap="xs">
                                <IconSword size={16} color="var(--mantine-color-red-6)" />
                                <Text size="xs" fw={500} c="dimmed">Combat</Text>
                            </Group>
                            <Group gap="sm">
                                <Tooltip label="Kills">
                                    <Badge variant="light" color="green" size="sm">
                                        {stats.kills}K
                                    </Badge>
                                </Tooltip>
                                <Tooltip label="Deaths">
                                    <Badge variant="light" color="red" size="sm">
                                        {stats.deaths}D
                                    </Badge>
                                </Tooltip>
                                <Tooltip label="Assists">
                                    <Badge variant="light" color="blue" size="sm">
                                        {stats.assists}A
                                    </Badge>
                                </Tooltip>
                            </Group>
                            <Text size="xs" c="dimmed">
                                {formatPercentage(stats.challenges.killParticipation)} Kill Part.
                            </Text>
                        </Stack>
                    </Grid.Col>

                    {/* Farm & Gold */}
                    <Grid.Col span={6}>
                        <Stack gap={4}>
                            <Group gap="xs">
                                <IconCoins size={16} color="var(--mantine-color-yellow-6)" />
                                <Text size="xs" fw={500} c="dimmed">Economy</Text>
                            </Group>
                            <Group gap="sm">
                                <Tooltip label={`${stats.totalCs} CS (${stats.csPerMinute.toFixed(1)}/min)`}>
                                    <Badge variant="light" color="orange" size="sm">
                                        {stats.totalCs} CS
                                    </Badge>
                                </Tooltip>
                                <Tooltip label={`${stats.challenges.goldPerMinute.toFixed(0)} Gold/min`}>
                                    <Badge variant="light" color="yellow" size="sm">
                                        {(stats.goldEarned / 1000).toFixed(1)}k
                                    </Badge>
                                </Tooltip>
                            </Group>
                            <Text size="xs" c="dimmed">
                                {stats.challenges.goldPerMinute.toFixed(0)} g/min
                            </Text>
                        </Stack>
                    </Grid.Col>

                    {/* Damage */}
                    <Grid.Col span={6}>
                        <Stack gap={4}>
                            <Group gap="xs">
                                <IconTarget size={16} color="var(--mantine-color-pink-6)" />
                                <Text size="xs" fw={500} c="dimmed">Damage</Text>
                            </Group>
                            <Tooltip label="Total Damage to Champions">
                                <Badge variant="light" color="pink" size="sm">
                                    {(stats.totalDamageDealtToChampions / 1000).toFixed(1)}k
                                </Badge>
                            </Tooltip>
                            <Text size="xs" c="dimmed">
                                {formatPercentage(stats.challenges.teamDamagePercentage)} Team DMG
                            </Text>
                            <Text size="xs" c="dimmed">
                                {stats.challenges.damagePerMinute.toFixed(0)} DMG/min
                            </Text>
                        </Stack>
                    </Grid.Col>

                    {/* Vision */}
                    <Grid.Col span={6}>
                        <Stack gap={4}>
                            <Group gap="xs">
                                <IconEye size={16} color="var(--mantine-color-cyan-6)" />
                                <Text size="xs" fw={500} c="dimmed">Vision</Text>
                            </Group>
                            <Group gap="sm">
                                <Tooltip label={`${stats.challenges.visionScorePerMinute.toFixed(1)} Vision Score/min`}>
                                    <Badge variant="light" color="cyan" size="sm">
                                        {stats.visionScore} VS
                                    </Badge>
                                </Tooltip>
                                <Tooltip label="Control Wards">
                                    <Badge variant="light" color="teal" size="sm">
                                        {stats.challenges.controlWardsPlaced} CW
                                    </Badge>
                                </Tooltip>
                            </Group>
                            <Text size="xs" c="dimmed">
                                {stats.wardsPlaced}👁️ / {stats.challenges.wardTakedowns}❌
                            </Text>
                        </Stack>
                    </Grid.Col>
                </Grid>

                {/* Objectives */}
                {(stats.challenges.dragonTakedowns > 0 || 
                  stats.challenges.baronTakedowns > 0 || 
                  stats.challenges.riftHeraldTakedowns > 0) && (
                    <>
                        <Divider />
                        <Stack gap={4}>
                            <Group gap="xs">
                                <IconTrophy size={16} color="var(--mantine-color-violet-6)" />
                                <Text size="xs" fw={500} c="dimmed">Objectives</Text>
                            </Group>
                            <Group gap="md">
                                {stats.challenges.dragonTakedowns > 0 && (
                                    <Tooltip label={`${stats.challenges.dragonTakedowns} Dragon Takedowns`}>
                                        <Badge variant="light" color="violet" size="sm">
                                            🐉 {stats.challenges.dragonTakedowns}
                                        </Badge>
                                    </Tooltip>
                                )}
                                {stats.challenges.riftHeraldTakedowns > 0 && (
                                    <Tooltip label={`${stats.challenges.riftHeraldTakedowns} Herald Takedowns`}>
                                        <Badge variant="light" color="grape" size="sm">
                                            👁️ {stats.challenges.riftHeraldTakedowns}
                                        </Badge>
                                    </Tooltip>
                                )}
                                {stats.challenges.baronTakedowns > 0 && (
                                    <Tooltip label={`${stats.challenges.baronTakedowns} Baron Takedowns`}>
                                        <Badge variant="light" color="indigo" size="sm">
                                            👹 {stats.challenges.baronTakedowns}
                                        </Badge>
                                    </Tooltip>
                                )}
                            </Group>
                        </Stack>
                    </>
                )}

                {/* Advanced Stats (if interesting) */}
                {(stats.challenges.soloKills > 0 || stats.challenges.multikills > 0) && (
                    <>
                        <Divider />
                        <Group gap="md" justify="center">
                            {stats.challenges.soloKills > 0 && (
                                <Tooltip label="Solo Kills">
                                    <Badge variant="light" color="orange" size="sm">
                                        ⚔️ {stats.challenges.soloKills} Solo
                                    </Badge>
                                </Tooltip>
                            )}
                            {stats.challenges.multikills > 0 && (
                                <Tooltip label="Multikills">
                                    <Badge variant="light" color="red" size="sm">
                                        💥 {stats.challenges.multikills} Multi
                                    </Badge>
                                </Tooltip>
                            )}
                            {stats.challenges.killingSprees > 0 && (
                                <Tooltip label="Killing Sprees">
                                    <Badge variant="light" color="grape" size="sm">
                                        🔥 {stats.challenges.killingSprees} Spree
                                    </Badge>
                                </Tooltip>
                            )}
                        </Group>
                    </>
                )}
            </Stack>
        </Card>
    );
};

export default ParticipantCard;
