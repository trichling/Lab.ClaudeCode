using Microsoft.AspNetCore.Mvc;
using LogicQuiz.Api.DTOs;
using LogicQuiz.Api.Services;

namespace LogicQuiz.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly ILogger<QuizController> _logger;

    public QuizController(IQuizService quizService, ILogger<QuizController> logger)
    {
        _quizService = quizService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameStateDto>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PlayerName))
            {
                return BadRequest("Player name is required");
            }

            if (request.Difficulty < 1 || request.Difficulty > 3)
            {
                return BadRequest("Difficulty must be 1 (Easy), 2 (Medium), or 3 (Hard)");
            }

            var gameState = await _quizService.StartGameAsync(request.PlayerName, request.Difficulty);
            return Ok(gameState);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting game for player {PlayerName}", request.PlayerName);
            return StatusCode(500, "An error occurred while starting the game");
        }
    }

    [HttpPost("answer")]
    public async Task<ActionResult<SubmitAnswerResponse>> SubmitAnswer([FromBody] SubmitAnswerRequest request)
    {
        try
        {
            var response = await _quizService.SubmitAnswerAsync(
                request.SessionId,
                request.QuestionId,
                request.SelectedFallacyTypeId
            );
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while submitting answer");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting answer for session {SessionId}", request.SessionId);
            return StatusCode(500, "An error occurred while submitting the answer");
        }
    }

    [HttpGet("result/{sessionId}")]
    public async Task<ActionResult<GameResultDto>> GetGameResult(int sessionId)
    {
        try
        {
            var result = await _quizService.GetGameResultAsync(sessionId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting game result");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game result for session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while getting the game result");
        }
    }
}
