import { apiGet, apiPost, apiPut, apiDelete } from "../lib/apiClient";
import type { CreateSquadRequest, SquadResponse, UpdateSquadRequest } from "../types/SquadDtos";
import type { SquadMatchDetailsResponse } from "../types/SquadMatchDetailsResponse";
import type { SquadMatchResponse } from "../types/SquadMatchDtos";


export const createSquad = async (req: CreateSquadRequest): Promise<SquadResponse> => {
    const squad = await apiPost<CreateSquadRequest, SquadResponse>("/squads", req);
    return squad;
};

export const getSquad = async (id: number): Promise<SquadResponse> => {
    const squad = await apiGet<SquadResponse>(`/squads/${id}`)
    return squad;
};

export const getAllSquads = async (): Promise<SquadResponse[]> => {
    const squads = await apiGet<SquadResponse[]>("/squads");
    return squads;
};

export const updateSquad = async (id: number, req: UpdateSquadRequest): Promise<SquadResponse> => {
    const squad = await apiPut<UpdateSquadRequest, SquadResponse>(`/squads/${id}`, req);
    return squad;
};

export const deleteSquad = async (id: number): Promise<number> => {
    const statusCode = await apiDelete<number>(`/squads/${id}`);
    return statusCode;
};

export const getMatches = async (id: number): Promise<SquadMatchResponse[]> => {
    const matches = await apiGet<SquadMatchResponse[]>(`/squads/${id}/matches`);
    return matches;
};

export const getMatchDetails = async (squadId: number, matchId: string): Promise<SquadMatchDetailsResponse> => {
    const matchDetails = await apiGet<SquadMatchDetailsResponse>(`/squads/${squadId}/matches/${matchId}`);
    return matchDetails;
};