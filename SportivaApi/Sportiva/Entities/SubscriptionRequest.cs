using System;
using Sportiva.Enums;

namespace Sportiva.Entities;

public enum SubscriptionRequestType
{
    Renew,
    Upgrade
}

public class SubscriptionRequest
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ClubId { get; set; } = string.Empty;
    public Club Club { get; set; } = default!;
    public string PlanId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = default!;
    public SubscriptionRequestType RequestType { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Note { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}
