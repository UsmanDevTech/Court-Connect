using Application.Chat.Commands;
using Application.Chat.Queries;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Contracts;
using Domain.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Api.Controllers.V1;

public class ChatController : BaseController
{
    private readonly IChatHub _chatHubService;
    public ChatController(IChatHub chatHubService)
    {
        _chatHubService = chatHubService;
    }

    [HttpPost(Routes.Chat.create_chat_head)]
    public async Task<ChatHeadContract> createChatHeadAsync([FromBody] CreateChatHeadCommand result, CancellationToken token)
    {
        return await Mediator.Send(result, token);
    }

    [HttpPost(Routes.Chat.get_chat_head)]
    public async Task<PaginationResponseBaseWithActionRequest<List<ChatHeadContract>>> getChatHeadAsync([FromBody] GetChatHeadPaginationQuery result, CancellationToken token)
    {
        return await Mediator.Send(result, token);
    }

    [HttpPost(Routes.Chat.get_chat_by_id)]
    public async Task<PaginationResponseBase<List<ConversationChat>>> getChatById([FromBody] GetChatByChatHeadIdPaginationQuery request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpDelete(Routes.Chat.delete_chat_head)]
    public async Task<Result> deleteChatHead([FromQuery] int ChatHeadId, CancellationToken token)
    {
        return await Mediator.Send(new DeleteChatHeadCommand(ChatHeadId), token);
    }

    [HttpPost(Routes.Chat.send_message)]
    public async Task<Result> sendMessageAsync([FromBody] SendMessageCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPut(Routes.Chat.read_chat)]
    public async Task<Result> readChatHeadAsync([FromBody] ReadChatHeadCommand request, CancellationToken token)
    {
        return await Mediator.Send(request, token);
    }

    [HttpPost(Routes.Chat.add_chat_member)]
    public async Task<ChatHeadContract> addChatMemberAsync([FromBody] AddGroupMemberCommand result, CancellationToken token)
    {
        return await Mediator.Send(result, token);
    }

    
}
