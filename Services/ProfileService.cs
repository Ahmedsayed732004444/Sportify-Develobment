using Sportiva.Contracts.Profile;
using Sportiva.Repositories;

namespace Sportiva.Services;
public class ProfileService(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ProfileResponse>> GetAsync(string userId,CancellationToken ct=default)
    {
        var spec = new UserProfileByUserIdSpec(userId);
        var profile = await _unitOfWork.UserProfiles.GetAsync(spec, ct);
        if (profile is null)
            return Result.Failure<ProfileResponse>(ProfileErrors.ProfileNotFound);
        return Result.Success(profile.Adapt<ProfileResponse>());
    }


}
