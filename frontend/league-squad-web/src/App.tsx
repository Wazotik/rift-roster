import './App.css';
import { Routes, Route } from 'react-router-dom';

// Pages
import Landing from './routes/Landing.tsx';
import Squads from './routes/Squads.tsx';
import Matches from './routes/Matches.tsx';
import SquadDetails from './routes/SquadDetails.tsx';
import Login from './routes/Login.tsx';
import Register from './routes/Register.tsx';


export default function App() {
    return (
        <>
            {/* <NavBar /> */}
            <Routes>
                <Route path="/" element={<Landing />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/squads">
                    <Route index element={<Squads />} />
                    <Route path=":squadId" element={<SquadDetails />} >
                        <Route path="matches" element={<Matches />} />
                    </Route>
                </Route>
            </Routes>
        </>
    )
}

