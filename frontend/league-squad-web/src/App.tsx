import './App.css';
import { Routes, Route } from 'react-router-dom';
import { Burger, Box } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';

// Pages
import Landing from './routes/Landing.tsx';
import Squads from './routes/Squads.tsx';
import Matches from './routes/Matches.tsx';
import SquadDetails from './routes/SquadDetails.tsx';
import Login from './routes/Login.tsx';
import Register from './routes/Register.tsx';
import Profile from './routes/Profile.tsx';
import AuthProvider from './components/AuthProvider.tsx';
import Sidebar from './components/Sidebar.tsx';


export default function App() {
    const [opened, { toggle, close }] = useDisclosure(false);

    return (
        <>
            <AuthProvider>
                {!opened && (
                    <Box
                        style={{
                            position: 'fixed',
                            top: '20px',
                            left: '20px',
                            zIndex: 1000,
                        }}
                    >
                        <Burger
                            opened={false}
                            onClick={toggle}
                            size="md"
                            aria-label="Toggle navigation"
                        />
                    </Box>
                )}

                <Sidebar opened={opened} onClose={close} />

                <Routes>
                    <Route path="/" element={<Landing />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/profile" element={<Profile />} />
                    <Route path="/squads">
                        <Route index element={<Squads />} />
                        <Route path=":squadId" element={<SquadDetails />} >
                            <Route path="matches" element={<Matches />} />
                        </Route>
                    </Route>
                </Routes>
            </AuthProvider>
        </>
    )
}
