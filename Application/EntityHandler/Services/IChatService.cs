using Application.DTOs;

namespace EntityHandler.Services.Interface
{
    public interface IChatService
    {
        Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request, Guid userId);
    }
}
