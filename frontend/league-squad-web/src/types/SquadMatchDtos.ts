export interface SquadMatchResponse {
    squadId: number,
    matchId: string,
    reasonForAddition?: string,
    queueId: number,
    gameStart: string,
    gameEnd: string,
    durationSeconds: number,
    mode: string,
    gameType: string,
    mapId: number,
    createdAt: number
}

// export interface SquadMatchRequest {

// }