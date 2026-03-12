using Sportiva.Contracts.Profile;

namespace Sportiva.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<UserProfile, ProfileResponse>()
            .Map(dest => dest.FullName, src => src.User.FullName);




    }
}