

export type ParticipantChallenges = {
    kda: number;
    killParticipation: number;
    teamDamagePercentage: number;
    damagePerMinute: number;
    goldPerMinute: number;

    laneMinionsFirst10Minutes: number;
    maxCsAdvantageOnLaneOpponent: number;
    maxLevelLeadLaneOpponent: number;
    earlyLaningPhaseGoldExpAdvantage: number;
    laningPhaseGoldExpAdvantage: number;

    dragonTakedowns: number;
    riftHeraldTakedowns: number;
    baronTakedowns: number;

    teamBaronKills: number;
    teamRiftHeraldKills: number;
    teamElderDragonKills: number;

    visionScorePerMinute: number;
    visionScoreAdvantageLaneOpponent: number;

    controlWardsPlaced: number;
    wardTakedowns: number;
    wardsGuarded: number;
    wardTakedownsBefore20M: number;

    enemyChampionImmobilizations: number;
    immobilizeAndKillWithAlly: number;

    damageTakenOnTeamPercentage: number;
    takedowns: number;
    soloKills: number;
    multikills: number;
    killingSprees: number;
};

export type ParticipantStats = {
    puuid: string;
    riotIdGameName: string;
    riotIdTagline: string;
    teamId: number;
    participantId: number;
    teamPosition: string;
    individualPosition: string;
    lane: string;
    role: string;

    championId: number;
    championName: string;

    kills: number;
    deaths: number;
    assists: number;
    win: boolean;

    totalMinionsKilled: number;
    neutralMinionsKilled: number;
    timePlayed: number;

    goldEarned: number;
    goldSpent: number;

    totalDamageDealtToChampions: number;
    totalDamageTaken: number;

    damageDealtToObjectives: number;
    damageDealtToTurrets: number;
    damageDealtToBuildings: number;

    dragonKills: number;
    baronKills: number;
    inhibitorTakedowns: number;
    turretTakedowns: number;

    visionScore: number;
    wardsPlaced: number;
    wardsKilled: number;
    detectorWardsPlaced: number;
    visionWardsBoughtInGame: number;
    timeCCingOthers: number;

    totalHealsOnTeammates: number;
    totalDamageShieldedOnTeammates: number;

    // computed fields from your service:
    totalCs: number;
    csPerMinute: number;
    killShare: number;
    goldShare: number;
    damageTakenToDealtRatio: number;

    dragonParticipation: number;
    heraldParticipation: number;
    baronParticipation: number;
    objectiveParticipation: number;

    challenges: ParticipantChallenges;
};
