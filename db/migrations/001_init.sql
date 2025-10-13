
-- CREATE TABLES

create table player (
  puuid TEXT PRIMARY KEY,
  game_name TEXT NOT NULL,
  tag_line TEXT NOT NULL,
  region TEXT,
  platform TEXT,
  created_at TIMESTAMPTZ DEFAULT now()
);


create table match (
  match_id TEXT PRIMARY KEY,
  start_time TIMESTAMPTZ,
  patch TEXT,
  queue INT,
  has_timeline BOOLEAN NOT NULL DEFAULT FALSE,
  created_at TIMESTAMPTZ DEFAULT now()
);


create table match_timeline (
  match_id TEXT NOT NULL references match(match_id) ON DELETE CASCADE,
  fetched_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  frame_interval_ms INT,
  timeline_json JSONB,
  PRIMARY KEY (match_id)
);

create table Participant (
  match_id TEXT NOT NULL references match(match_id) ON DELETE CASCADE,
  participant_id INT NOT NULL,
  puuid TEXT NOT NULL references player(puuid),
  team_id INT NOT NULL,
  team_position TEXT,
  champion_id INT,
  kills INT NOT NULL DEFAULT 0,
  deaths INT NOT NULL DEFAULT 0,
  assists INT NOT NULL DEFAULT 0,
  PRIMARY KEY (match_id, participant_id),
  UNIQUE (match_id, puuid)
);


create table squad (
  id BIGSERIAL PRIMARY KEY,
  name TEXT NOT NULL,
  created_at TIMESTAMPTZ DEFAULT now()
);

create table squad_member (
  squad_id BIGINT references squad(id) ON DELETE CASCADE,
  puuid TEXT NOT NULL references player(puuid),
  alias TEXT,
  added_at TIMESTAMPTZ DEFAULT now(),
  PRIMARY KEY (squad_id, puuid)
);

create table squad_match (
  squad_id BIGINT references squad(id) ON DELETE CASCADE,
  match_id TEXT NOT NULL references match(match_id) ON DELETE CASCADE,
  reason_for_addition TEXT,
  created_at TIMESTAMPTZ DEFAULT now(),
  PRIMARY KEY (squad_id, match_id)
);


-- CREATE INDEXES

CREATE INDEX idx_players_riot_id        ON player (LOWER(game_name), UPPER(tag_line));
CREATE INDEX idx_matches_start_time     ON match (start_time DESC);
CREATE INDEX idx_match_timelines_fetch  ON match_timeline (fetched_at DESC);
CREATE INDEX idx_participants_match     ON participant (match_id);
CREATE INDEX idx_participants_puuid     ON participant (puuid);
CREATE INDEX idx_squad_matches_created  ON squad_match (squad_id, created_at DESC);