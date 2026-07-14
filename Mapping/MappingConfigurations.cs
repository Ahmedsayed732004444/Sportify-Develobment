using Sportiva.Contracts.Authentication;
using Sportiva.Contracts.Shared.Enums;
using Sportiva.Contracts.Shared.Summaries;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Enums;
using System.Linq;

namespace Sportiva.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        // ClubSubscription mapping configurations
        config.NewConfig<ClubSubscription, ClubSubscriptionResponse>()
            .Map(dest => dest.SubscriptionId, src => src.Id)
            .Map(dest => dest.Club, src => src.Club)
            .Map(dest => dest.Plan, src => src.Plan)
            .Map(dest => dest.StartDate, src => src.StartDate)
            .Map(dest => dest.EndDate, src => src.EndDate)
            .Map(dest => dest.IsActive, src => src.Status == SubscriptionStatus.Active)
            .Map(dest => dest.PaymentsCount, src => src.Payments.Count)
            .Map(dest => dest.LastPayment, src => src.Payments
                .OrderByDescending(p => p.PaidAt)
                .ThenByDescending(p => p.Id)
                .FirstOrDefault());

        config.NewConfig<SubscriptionPlan, SubscriptionPlanSummary>()
            .Map(dest => dest.PlanId, src => src.Id);

        config.NewConfig<SubscriptionPayment, SubscriptionPaymentSummary>()
            .Map(dest => dest.PaymentId, src => src.Id);
    }
}