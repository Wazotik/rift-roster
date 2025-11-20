import { Card, Group, Stack, Text, Badge, ActionIcon, Tooltip } from "@mantine/core";
import { IconEdit, IconTrash } from "@tabler/icons-react";
import type { SquadMemberResponse } from "../types/SquadMemberDtos";

type SquadMemberCardProps = {
    member: SquadMemberResponse;
    onEdit?: (member: SquadMemberResponse) => void;
    onDelete?: (member: SquadMemberResponse) => void;
};

const SquadMemberCard = ({ member, onEdit, onDelete }: SquadMemberCardProps) => {
    return (
        <Card shadow="sm" padding="md" radius="md" withBorder>
            <Stack gap="sm">
                {/* Header with Name and Actions */}
                <Group justify="space-between" wrap="nowrap">
                    <Stack gap={4} style={{ flex: 1 }}>
                        <Text fw={600} size="md">
                            {member.gameName} #{member.tagLine}
                        </Text>
                        {member.alias && (
                            <Text size="xs" c="dimmed">
                                "{member.alias}"
                            </Text>
                        )}
                    </Stack>
                    
                    <Group gap="xs">
                        {onEdit && (
                            <Tooltip label="Edit member">
                                <ActionIcon
                                    variant="subtle"
                                    color="blue"
                                    onClick={() => onEdit(member)}
                                >
                                    <IconEdit size={18} />
                                </ActionIcon>
                            </Tooltip>
                        )}
                        {onDelete && (
                            <Tooltip label="Remove member">
                                <ActionIcon
                                    variant="subtle"
                                    color="red"
                                    onClick={() => onDelete(member)}
                                >
                                    <IconTrash size={18} />
                                </ActionIcon>
                            </Tooltip>
                        )}
                    </Group>
                </Group>

                {/* Badges for Role and Region */}
                <Group gap="xs">
                    {member.role && (
                        <Badge variant="light" color="blue" size="sm">
                            {member.role}
                        </Badge>
                    )}
                    {member.region && (
                        <Badge variant="dot" color="gray" size="sm">
                            {member.region}
                        </Badge>
                    )}
                </Group>

                {/* Member Since */}
                <Text size="xs" c="dimmed">
                    Member since {new Date(member.createdAt).toLocaleDateString("en-US", { 
                        month: "short", 
                        day: "numeric", 
                        year: "numeric" 
                    })}
                </Text>
            </Stack>
        </Card>
    );
};

export default SquadMemberCard;
