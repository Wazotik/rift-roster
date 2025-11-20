export interface SquadResponse {
    id: number,
    name: string,
    iconUrl: string,
    createdAt: string
}

// export interface UpdateSquadRequest {
//     name: string,
// }
export type UpdateSquadRequest = Omit<SquadResponse, "id" | "createdAt">;


// export interface CreateSquadRequest {
//     name: string,
// }
export type CreateSquadRequest = Omit<SquadResponse, "id" | "createdAt">;

