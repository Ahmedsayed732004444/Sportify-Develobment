namespace Sportiva.Specifications;

/// <summary>جيب UserProfile مع User و Posts</summary>
public class UserProfileWithDetailsSpec : BaseSpecification<UserProfile>
{
    public UserProfileWithDetailsSpec(string userId)
    {
        Criteria = u => u.UserId == userId;
        AddInclude(u => u.User);
        AddInclude(u => u.Posts);
    }
}

/// <summary>جيب UserProfile مع User بس (بدون Posts) - أسرع</summary>
public class UserProfileByUserIdSpec : BaseSpecification<UserProfile>
{
    public UserProfileByUserIdSpec(string userId)
    {
        Criteria = u => u.UserId == userId;
        AddInclude(u => u.User);
    }
}
