namespace Multiverse_Rumble_API.Models
{
    /// <summary>
    /// Representa el resultado de una partida 1v1 jugada en Multiverse Rumble.
    /// </summary>
    public class MatchResult
    {
        public int Id { get; set; }

        public string Player1Character { get; set; } = string.Empty;

        public string Player2Character { get; set; } = string.Empty;

        public string WinnerCharacter { get; set; } = string.Empty;

        public int RoundsPlayed { get; set; }

        public int DurationSeconds { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}