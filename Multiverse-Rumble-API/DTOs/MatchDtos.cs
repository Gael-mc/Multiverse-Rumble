namespace Multiverse_Rumble_API.DTOs
{
    public class CreateMatchDto
    {
        public string Player1Character { get; set; } = string.Empty;
        public string Player2Character { get; set; } = string.Empty;
        public string WinnerCharacter { get; set; } = string.Empty;
        public int RoundsPlayed { get; set; }
        public int DurationSeconds { get; set; }
    }

    public class MatchResponseDto
    {
        public int Id { get; set; }
        public string Player1Character { get; set; } = string.Empty;
        public string Player2Character { get; set; } = string.Empty;
        public string WinnerCharacter { get; set; } = string.Empty;
        public int RoundsPlayed { get; set; }
        public int DurationSeconds { get; set; }
        public DateTime PlayedAt { get; set; }
    }

    public class CharacterStatsDto
    {
        public string Character { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int TotalMatches { get; set; }
        public double WinRate { get; set; }
    }
}