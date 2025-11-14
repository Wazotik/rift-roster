export interface SquadMemberResponse {
    squadId: number,
    puuid: string,
    role?: string,
    alias?: string, 
    createdAt: string, 
    gameName: string, 
    tagLine: string, 
    region?: string, 
}

// Split to AddSquadMemberRequest and GetSquadMemberRequest
export interface AddSquadMemberRequest {
    puuid: string,
    role?: string, 
    alias?: string
}

