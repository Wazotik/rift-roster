import type { AddSquadMemberRequest, SquadMemberResponse } from "../types/SquadMemberDtos";
import { apiGet, apiPost } from "../lib/apiClient";


export const getSquadMembers = async (squadId: number): Promise<SquadMemberResponse[]> => {
    const squadMembers = await apiGet<SquadMemberResponse[]>(`/squads/${squadId}/members`);
    return squadMembers;
};

export const addSquadMember = async (squadId: number, req: AddSquadMemberRequest): Promise<SquadMemberResponse> => {
    const squadMember = await apiPost<AddSquadMemberRequest, SquadMemberResponse>(`/squads/${squadId}/members`, req);
    return squadMember;
};
