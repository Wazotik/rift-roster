import React from 'react'
import { getSquad } from '../api/squads';

const SquadDetails = () => {

    // Get a squad
    const { data: squad, isLoading: isSquadLoading, isError: isSquadError, error: squadError } = useQuery({
        queryKey: ["squads", squadId],
        queryFn: () => getSquad(squadId)
    });


    return (
        <div>SquadDetails</div>
    )
}

export default SquadDetails