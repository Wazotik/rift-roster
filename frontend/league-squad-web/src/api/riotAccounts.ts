import { apiGet } from "../lib/apiClient";
import type { RiotAccountResponse } from "../types/RiotAccountDtos";

export const getRiotAccountUsingPuuid = async (puuid: string): Promise<RiotAccountResponse> => {
    const riotAccount = await apiGet<RiotAccountResponse>(`/riot-account/${puuid}`);
    return riotAccount;
} 

export const getRiotAccountUsingRiotId = async (gameName: string, tagLine: string): Promise<RiotAccountResponse> => {
    const riotAccount = await apiGet<RiotAccountResponse>(`/riot-account/${gameName}/${tagLine}`);
    return riotAccount;
} 

