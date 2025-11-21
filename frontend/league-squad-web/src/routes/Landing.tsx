import { useNavigate } from 'react-router-dom';
import { Container, Flex, Title, Text, Button, Card, Badge, Box, Accordion } from '@mantine/core';

const Landing = () => {
    const navigate = useNavigate();

    const currentFeatures = [
        {
            value: 'squad-management',
            title: 'Squad Management',
            icon: '🛡️',
            status: 'complete',
            description: 'Create and manage squads persistently',
            details: 'Fully functional system for creating, updating, and organizing your League of Legends teams. Add or remove members, and keep your squad roster up to date.'
        },
        {
            value: 'match-tracking',
            title: 'Match Data Sync',
            icon: '📊',
            status: 'complete',
            description: 'Pull and store recent matches for all squad members',
            details: 'Automatically fetch and persist match history for every member of your squad through the Riot Games API.'
        },
        {
            value: 'squad-dashboard',
            title: 'Squad Dashboard',
            icon: '📈',
            status: 'in-progress',
            description: 'View roster, matches, and basic team metrics',
            details: 'A centralized dashboard showing your squad\'s recent matches, member statistics, and collective performance data.'
        },
    ];

    const futurePhases = [
        {
            value: 'phase-2',
            title: 'Phase 2: Insights & Playstyle Analysis',
            items: [
                'Playstyle tags based on timeline metrics',
                'Weekly squad summaries',
                'Match visualizations (gold/XP graphs)',
                'Insights system for strengths and weaknesses'
            ]
        },
        {
            value: 'phase-3',
            title: 'Phase 3: Social & Gamified Experience',
            items: [
                'Squad challenges (e.g., "Win 3 games this week")',
                'Progression badges',
                'Public/private squad profiles',
                'Comparative leaderboards'
            ]
        },
        {
            value: 'phase-4',
            title: 'Phase 4: Advanced Analytics & AI',
            items: [
                'AI-generated match summaries',
                'Synergy analytics ("Your jungle-mid duo performs best together")',
                'Champion/role recommendations',
                'End-of-season "Squad Wrapped" summaries'
            ]
        }
    ];

    const getStatusBadge = (status: string) => {
        if (status === 'complete') {
            return <Badge color="green" variant="light">Live</Badge>;
        }
        if (status === 'in-progress') {
            return <Badge color="blue" variant="light">In Progress</Badge>;
        }
        return <Badge color="gray" variant="light">Planned</Badge>;
    };

    return (
        <Box>
            {/* Hero Section */}
            <Box
                style={{
                    background: 'linear-gradient(135deg, #1e3a8a 0%, #5b21b6 100%)',
                    minHeight: '65vh',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                }}
            >
                <Container size="md">
                    <Flex direction="column" align="center" gap="xl" style={{ textAlign: 'center', color: 'white' }}>
                        <Title order={1} style={{ fontSize: '3.5rem', fontWeight: 900, letterSpacing: '-0.02em' }}>
                            RiftRoster
                        </Title>
                        <Title order={2} style={{ fontSize: '2rem', fontWeight: 600, opacity: 0.95 }}>
                            Track Your League Squad Like a Pro
                        </Title>
                        <Text size="xl" style={{ fontSize: '1.25rem', opacity: 0.9, maxWidth: '650px' }}>
                            Bring your team's performance into one shared dashboard. View recent matches, collective win rates, and playstyle patterns that reveal your squad's identity.
                        </Text>
                        <Button 
                            size="xl" 
                            onClick={() => navigate('/squads')}
                            style={{ marginTop: '1rem' }}
                        >
                            View My Squads
                        </Button>
                    </Flex>
                </Container>
            </Box>

            {/* Current Features Section */}
            <Container size="lg" py={80}>
                <Title order={2} ta="center" mb={60}>
                    Current Features
                </Title>
                
                <Accordion variant="separated" radius="md">
                    {currentFeatures.map((feature) => (
                        <Accordion.Item key={feature.value} value={feature.value}>
                            <Accordion.Control>
                                <Flex align="center" gap="md">
                                    <Text size="2rem" style={{ lineHeight: 1 }}>
                                        {feature.icon}
                                    </Text>
                                    <Box style={{ flex: 1 }}>
                                        <Flex align="center" gap="sm" mb={4}>
                                            <Title order={4}>{feature.title}</Title>
                                            {getStatusBadge(feature.status)}
                                        </Flex>
                                        <Text size="sm" c="dimmed">
                                            {feature.description}
                                        </Text>
                                    </Box>
                                </Flex>
                            </Accordion.Control>
                            <Accordion.Panel>
                                <Text>{feature.details}</Text>
                            </Accordion.Panel>
                        </Accordion.Item>
                    ))}
                </Accordion>
            </Container>

            {/* Roadmap Section */}
            <Box style={{ background: '#f8f9fa' }} py={80}>
                <Container size="lg">
                    <Title order={2} ta="center" mb={60}>
                        Roadmap: What's Coming
                    </Title>
                    
                    <Accordion variant="separated" radius="md">
                        {futurePhases.map((phase) => (
                            <Accordion.Item key={phase.value} value={phase.value}>
                                <Accordion.Control>
                                    <Title order={4}>{phase.title}</Title>
                                </Accordion.Control>
                                <Accordion.Panel>
                                    <Flex direction="column" gap="sm">
                                        {phase.items.map((item, idx) => (
                                            <Flex key={idx} align="center" gap="sm">
                                                <Text c="dimmed" size="sm">•</Text>
                                                <Text size="sm">{item}</Text>
                                            </Flex>
                                        ))}
                                    </Flex>
                                </Accordion.Panel>
                            </Accordion.Item>
                        ))}
                    </Accordion>
                </Container>
            </Box>

            {/* Footer */}
            <Box py={40} style={{ borderTop: '1px solid #e0e0e0' }}>
                <Container>
                    <Flex direction="column" align="center" gap="xs">
                        <Text ta="center" c="dimmed">
                            Made with ❤️ for the League community
                        </Text>
                        <Text ta="center" c="dimmed" size="sm">
                            © {new Date().getFullYear()} Wahaj Haider
                        </Text>
                    </Flex>
                </Container>
            </Box>
        </Box>
    );
};

export default Landing;
