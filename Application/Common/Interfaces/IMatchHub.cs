
using Domain.Contracts;

namespace Application.Common.Interfaces;

public interface IMatchHub
{
    Task SendVerification(LocationVerificationContract verify);
}
