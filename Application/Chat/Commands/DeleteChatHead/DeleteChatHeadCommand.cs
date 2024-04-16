
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Chat.Commands;

public sealed record DeleteChatHeadCommand(int chatHeadId) : IRequest<Result>;
internal sealed class DeleteChatHeadCommandHandler : IRequestHandler<DeleteChatHeadCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public DeleteChatHeadCommandHandler(IApplicationDbContext context,
       ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<Result> Handle(DeleteChatHeadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId == null)
            throw new NotFoundException("User not found");

        var head = await _context.ChatHeads.Include(x => x.ChatMembers).Include(x => x.ChatConversations).Where(x => x.Id == request.chatHeadId).FirstOrDefaultAsync();
        if (head == null)
            throw new NotFoundException("Chat head not found!");

        var delChatHead = head.ChatMembers.Where(c => c.ParticipantId == userId).FirstOrDefault();
        if (delChatHead != null)
        {
            delChatHead.isChatHeadDelete(true);

            var msgId = head.ChatConversations.OrderBy(d => d.Created).LastOrDefault();
            if (msgId != null)
                delChatHead.headDelete(msgId.Id);

            _context.ChatMembers.Update(delChatHead);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}