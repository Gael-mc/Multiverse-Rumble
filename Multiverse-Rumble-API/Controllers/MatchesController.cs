using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Multiverse_Rumble_API.Data;
using Multiverse_Rumble_API.DTOs;
using Multiverse_Rumble_API.Models;

namespace Multiverse_Rumble_API.Controllers
{
    /// <summary>
    /// Endpoints REST para registrar y consultar el historial de partidas 1v1.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el historial completo de partidas, ordenado de más reciente a más antigua.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetMatches()
        {
            var matches = await _context.Matches
                .OrderByDescending(m => m.PlayedAt)
                .Select(m => ToDto(m))
                .ToListAsync();

            return Ok(matches);
        }

        /// <summary>
        /// Obtiene el detalle de una partida específica por su Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MatchResponseDto>> GetMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match is null) return NotFound();

            return Ok(ToDto(match));
        }

        /// <summary>
        /// Registra el resultado de una partida recién jugada.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MatchResponseDto>> CreateMatch(CreateMatchDto dto)
        {
            if (dto.WinnerCharacter != dto.Player1Character && dto.WinnerCharacter != dto.Player2Character)
            {
                return BadRequest("WinnerCharacter debe coincidir con Player1Character o Player2Character.");
            }

            var match = new MatchResult
            {
                Player1Character = dto.Player1Character,
                Player2Character = dto.Player2Character,
                WinnerCharacter = dto.WinnerCharacter,
                RoundsPlayed = dto.RoundsPlayed,
                DurationSeconds = dto.DurationSeconds,
                PlayedAt = DateTime.UtcNow
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, ToDto(match));
        }

        /// <summary>
        /// Elimina un registro de partida.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match is null) return NotFound();

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Devuelve estadísticas de victorias/derrotas para un personaje específico.
        /// </summary>
        [HttpGet("stats/{character}")]
        public async Task<ActionResult<CharacterStatsDto>> GetCharacterStats(string character)
        {
            var relevantMatches = await _context.Matches
                .Where(m => m.Player1Character == character || m.Player2Character == character)
                .ToListAsync();

            var wins = relevantMatches.Count(m => m.WinnerCharacter == character);
            var total = relevantMatches.Count;

            var stats = new CharacterStatsDto
            {
                Character = character,
                Wins = wins,
                Losses = total - wins,
                TotalMatches = total,
                WinRate = total == 0 ? 0 : Math.Round((double)wins / total * 100, 1)
            };

            return Ok(stats);
        }

        private static MatchResponseDto ToDto(MatchResult m) => new()
        {
            Id = m.Id,
            Player1Character = m.Player1Character,
            Player2Character = m.Player2Character,
            WinnerCharacter = m.WinnerCharacter,
            RoundsPlayed = m.RoundsPlayed,
            DurationSeconds = m.DurationSeconds,
            PlayedAt = m.PlayedAt
        };
    }
}