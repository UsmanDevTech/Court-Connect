using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Commands;

public sealed record ReadChatHeadCommand(int chatHeadId) : IRequest<Result>;
internal sealed class ReadChatHeadCommandHandler : IRequestHandler<ReadChatHeadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public ReadChatHeadCommandHandler(IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(ReadChatHeadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var head = await _context.ChatHeads.Include(x => x.ChatMembers).Include(x => x.ChatConversations).Where(x => x.Id == request.chatHeadId).FirstOrDefaultAsync();
        if (head == null)
            throw new NotFoundException("Chat head not found!");


        var chatMember = head.ChatMembers.Where(c => c.ParticipantId == userId).FirstOrDefault();
        var lastSeenMsgId = head.ChatConversations.OrderByDescending(d => d.Created).FirstOrDefault();
        if (lastSeenMsgId != null)
            chatMember.lastReadMessage(lastSeenMsgId.Id);
       
        
        _context.ChatMembers.Update(chatMember);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}