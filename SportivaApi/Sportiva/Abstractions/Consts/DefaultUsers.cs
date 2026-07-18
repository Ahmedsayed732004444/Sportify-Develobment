namespace Sportiva.Abstractions.Consts;

public static class DefaultUsers
{
    public partial class Admin
    {
        public const string Id = "0191a4b6-c4fc-752e-9d95-40b30fa7a9b6";
        public const string Email = "sayed732004444@gmail.com";
        public const string PasswordHash = "AQAAAAIAAYagAAAAEKRku5u6K325Irl1Utujiuil/WUhjTvShS9mJLXxO+2v/GKrMT1Ofhdp/0taFUO2bA==";
        public const string SecurityStamp = "55BF92C9EF0249CDA210D85D1A851BC9";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b42a925b8e";
    }

    public partial class TesterAdmin
    {
        public const string Id = "0191b2c3-c4fc-752e-9d95-40b30fa7a9b7";
        public const string Email = "admin@sportify.com";
        public const string PasswordHash = "AQAAAAIAAYagAAAAEF+nP3oIur8ZNGiku2rjPpIZ0zeD9A6/kdsn0J6C2n7YJtRyMA2iHrvr5W+1D2i++w=="; // Admin123!
        public const string SecurityStamp = "55BF92C9EF0249CDA210D85D1A851BD0";
        public const string ConcurrencyStamp = "0191a4b6-c4fc-752e-9d95-40b42a925b8f";
    }
}