import type { AddSquadMemberRequest, DeleteSquadMemberRequest, SquadMemberResponse } from "../types/SquadMemberDtos";
import { apiDelete, apiGet, apiPost } from "../lib/apiClient";


export const getSquadMembers = async (squadId: number): Promise<SquadMemberResponse[]> => {
    const squadMembers = await apiGet<SquadMemberResponse[]>(`/squads/${squadId}/members`);
    return squadMembers;
};

export const addSquadMember = async (squadId: number, req: AddSquadMemberRequest): Promise<SquadMemberResponse> => {
    const squadMember = await apiPost<AddSquadMemberRequest, SquadMemberResponse>(`/squads/${squadId}/members`, req);
    return squadMember;
};

export const deleteSquadMember = async (req: DeleteSquadMemberRequest): Promise<number> => {
    const statusCode = await apiDelete<number>(`/squads/${req.squadId}/members/${req.puuid}`)
    return statusCode;
};
