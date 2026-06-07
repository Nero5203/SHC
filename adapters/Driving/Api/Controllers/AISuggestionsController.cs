using api.Authorization;
using application.Dto.AI;
using application.Ports.Driving.Auth;
using application.Ports.Driving.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ai-suggestions")]
    public class AISuggestionsController : ControllerBase
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IGenerateAISuggestionsUseCase _generateAISuggestionsUseCase;
        private readonly IGetPendingAISuggestionsUseCase _getPendingAISuggestionsUseCase;
        private readonly IAcceptAISuggestionUseCase _acceptAISuggestionUseCase;
        private readonly IDismissAISuggestionUseCase _dismissAISuggestionUseCase;

        public AISuggestionsController(
            ICurrentUserService currentUserService,
            IGenerateAISuggestionsUseCase generateAISuggestionsUseCase,
            IGetPendingAISuggestionsUseCase getPendingAISuggestionsUseCase,
            IAcceptAISuggestionUseCase acceptAISuggestionUseCase,
            IDismissAISuggestionUseCase dismissAISuggestionUseCase)
        {
            _currentUserService = currentUserService;
            _generateAISuggestionsUseCase = generateAISuggestionsUseCase;
            _getPendingAISuggestionsUseCase = getPendingAISuggestionsUseCase;
            _acceptAISuggestionUseCase = acceptAISuggestionUseCase;
            _dismissAISuggestionUseCase = dismissAISuggestionUseCase;
        }

        [HttpPost("user/{userId:guid}/generate")]
        public async Task<ActionResult<IReadOnlyList<AISuggestionResponseDto>>> GenerateSuggestions(Guid userId)
        {
            if (_currentUserService.UserId != userId)
            {
                return Forbid();
            }

            var suggestions = await _generateAISuggestionsUseCase.ExecuteAsync(userId);
            return Ok(suggestions.Select(MapSuggestion).ToList());
        }

        [HttpGet("user/{userId:guid}/pending")]
        public async Task<ActionResult<IReadOnlyList<AISuggestionResponseDto>>> GetPendingSuggestions(Guid userId)
        {
            if (_currentUserService.UserId != userId)
            {
                return Forbid();
            }

            var suggestions = await _getPendingAISuggestionsUseCase.ExecuteAsync(userId);
            return Ok(suggestions.Select(MapSuggestion).ToList());
        }

        [HttpPut("{suggestionId:guid}/accept")]
        public async Task<ActionResult<AISuggestionResponseDto>> AcceptSuggestion(Guid suggestionId)
        {
            if (_currentUserService.UserId == null)
            {
                return Forbid();
            }

            var suggestion = await _acceptAISuggestionUseCase.ExecuteAsync(_currentUserService.UserId.Value, suggestionId);
            if (suggestion == null)
            {
                return NotFound();
            }

            return Ok(MapSuggestion(suggestion));
        }

        [HttpPut("{suggestionId:guid}/dismiss")]
        public async Task<ActionResult<AISuggestionResponseDto>> DismissSuggestion(Guid suggestionId)
        {
            if (_currentUserService.UserId == null)
            {
                return Forbid();
            }

            var suggestion = await _dismissAISuggestionUseCase.ExecuteAsync(_currentUserService.UserId.Value, suggestionId);
            if (suggestion == null)
            {
                return NotFound();
            }

            return Ok(MapSuggestion(suggestion));
        }

        private static AISuggestionResponseDto MapSuggestion(Domain.Entities.AI.AISuggestion suggestion)
        {
            return new AISuggestionResponseDto
            {
                Id = suggestion.Id,
                UserId = suggestion.UserId,
                FileItemId = suggestion.FileItemId,
                FileName = suggestion.FileItem?.FileName,
                Type = suggestion.Type,
                Status = suggestion.Status,
                Title = suggestion.Title,
                Description = suggestion.Description,
                SuggestedValue = suggestion.SuggestedValue,
                CreatedAt = suggestion.CreatedAt,
                ExpiresAt = suggestion.ExpiresAt,
                AcceptedAt = suggestion.AcceptedAt,
                DismissedAt = suggestion.DismissedAt
            };
        }
    }
}
