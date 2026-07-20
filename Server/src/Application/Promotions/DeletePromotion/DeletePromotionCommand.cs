using Application.Abstractions.Messaging;

namespace Application.Promotions.DeletePromotion;

public sealed record DeletePromotionCommand(Guid Id) : ICommand;
