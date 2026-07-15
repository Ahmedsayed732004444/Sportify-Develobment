namespace Sportiva.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        //config.NewConfig<UserProfile, ProfileResponse>()
        //    .Map(dest => dest.FullName, src => src.User.FullName);
        // ✅ ضيف الـ tuple mapping
        //config.NewConfig<(ApplicationUser User, IEnumerable<string> Roles), UserResponse>()
        //    .Map(dest => dest.Id, src => src.User.Id)
        //    .Map(dest => dest.FirstName, src => src.User.FirstName)
        //    .Map(dest => dest.LastName, src => src.User.LastName)
        //    .Map(dest => dest.Email, src => src.User.Email)
        //    .Map(dest => dest.IsDisabled, src => src.User.IsDisabled)
        //    .Map(dest => dest.Roles, src => src.Roles);



    }
}