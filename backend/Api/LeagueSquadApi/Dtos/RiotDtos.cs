namespace LeagueSquadApi.Dtos
{
    public class RiotDtos
    {
        public record RiotAccountResponse(
            string Puuid,
            string GameName,
            string TagLine,
            string? Region
        );

        public record RiotAccountCoreResponse(string Puuid, string GameName, string TagLine);

        public record RiotAccountRegionResponse(string Puuid, string Game, string Region);

        // contract for frontend
        public record RiotMatchResponse(
            string MatchId,
            int QueueId,
            List<string> ParticipantsIds,
            List<RiotMatchParticipant> Participants,
            string ParticipantsJson,
            string TimelineJson,
            DateTimeOffset GameStart,
            DateTimeOffset GameEnd,
            int DurationSeconds,
            string Mode,
            string GameType,
            int MapId
        );

        // injest from riot
        public record RiotMatchDto(RiotMatchMetadata Metadata, RiotMatchInfo Info);

        public record RiotMatchMetadata(string MatchId, List<string> Participants);

        public record RiotMatchInfo(
            int QueueId,
            List<RiotMatchParticipant> Participants,
            long GameStartTimestamp,
            long GameEndTimestamp,
            int GameDuration,
            string GameMode,
            string GameType,
            int MapId
        );

        public record RiotMatchParticipant(
            string Puuid,
            int TeamId,
            int ParticipantId,
            string TeamPosition,
            int ChampionId,
            int Kills,
            int Deaths,
            int Assists,
            bool Win
        );

        // internal (not external api) contract of riot data
        public record RiotRawParticipantForStats(
            string Puuid,
            string RiotIdGameName,
            string RiotIdTagline,
            int TeamId,
            int ParticipantId,
            string TeamPosition,
            string IndividualPosition,
            string Lane,
            string Role,
            int ChampionId,
            string ChampionName,
            int Kills,
            int Deaths,
            int Assists,
            bool Win,
            int TotalMinionsKilled,
            int NeutralMinionsKilled,
            int TimePlayed,
            int GoldEarned,
            int GoldSpent,
            int TotalDamageDealtToChampions,
            int TotalDamageTaken,
            int DamageDealtToObjectives,
            int DamageDealtToTurrets,
            int DamageDealtToBuildings,
            int DragonKills,
            int BaronKills,
            int RiftHeraldKills,
            int InhibitorTakedowns,
            int TurretTakedowns,
            int VisionScore,
            int WardsPlaced,
            int WardsKilled,
            int DetectorWardsPlaced,
            int VisionWardsBoughtInGame,
            int TimeCCingOthers,
            int TotalHealsOnTeammates,
            int TotalDamageShieldedOnTeammates,
            // need to be computed
            RiotParticipantChallenges Challenges
        )
        {
            public int TotalCs { get; set; }
            public double CsPerMinute { get; set; }
            public double KillShare { get; set; }
            public double GoldShare { get; set; }
            public double ObjectiveParticipation { get; set; }
            public double DamageTakenToDealtRatio { get; set; }
        };

        public record RiotParticipantChallenges(
            double Kda,
            double KillParticipation,
            double TeamDamagePercentage,
            double DamagePerMinute,
            double GoldPerMinute,
            int LaneMinionsFirst10Minutes,
            double MaxCsAdvantageOnLaneOpponent,
            int MaxLevelLeadLaneOpponent,
            int EarlyLaningPhaseGoldExpAdvantage,
            int LaningPhaseGoldExpAdvantage,
            int DragonTakedowns,
            int RiftHeraldTakedowns,
            int BaronTakedowns,
            int TeamBaronKills,
            int TeamRiftHeraldKills,
            int TeamElderDragonKills,
            double VisionScorePerMinute,
            double VisionScoreAdvantageLaneOpponent,
            int ControlWardsPlaced,
            int WardTakedowns,
            int WardsGuarded,
            int WardTakedownsBefore20M,
            int EnemyChampionImmobilizations,
            int ImmobilizeAndKillWithAlly,
            double DamageTakenOnTeamPercentage,
            int Takedowns,
            int SoloKills,
            int Multikills,
            int KillingSprees
        );

        public record RiotHttpResult<T>
        {
            public int StatusCode { get; init; }
            public T Value { get; init; }

            public bool IsSuccessful
            {
                get { return StatusCode >= 200 && StatusCode <= 299; }
            }

            private RiotHttpResult() { }

            public static RiotHttpResult<T> Ok(T value, int statusCode = 200)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode, Value = value };
            }

            public static RiotHttpResult<T> Fail(int statusCode, T value)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode, Value = value };
            }

            public static RiotHttpResult<T> Fail(int statusCode)
            {
                return new RiotHttpResult<T> { StatusCode = statusCode };
            }
        }

        public record RiotHttpResult
        {
            public int StatusCode { get; init; }

            private RiotHttpResult() { }

            public bool IsSuccessful
            {
                get { return StatusCode >= 200 && StatusCode <= 299; }
            }

            public static RiotHttpResult Ok(int statusCode = 200)
            {
                return new RiotHttpResult { StatusCode = statusCode };
            }

            public static RiotHttpResult Fail(int statusCode)
            {
                return new RiotHttpResult { StatusCode = statusCode };
            }
        }
    }
}
