using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class UserMapping : ClassMap<User>
    {
        public UserMapping()
        {
            Table("webpages_Membership");
            Id(c => c.Id).Column("UserId").GeneratedBy.Native();
            Map(c => c.CreateDate);
            //Map(c => c.Username, "UserName").Not.Nullable().Unique();
            Map(c => c.Password);
            Map(c => c.Email).Unique();
            Map(c => c.FirstName);
            Map(c => c.LastName);
            Map(c => c.Role).Not.Nullable().CustomType<int>();
            Map(c => c.ConfirmationToken);
            Map(c => c.IsConfirmed);
            Map(c => c.IsEmailConfirmed);
            Map(c => c.LastPasswordFailureDate);
            Map(c => c.PasswordFailuresSinceLastSuccess);
            Map(c => c.PasswordChangedDate);
            Map(c => c.PasswordSalt);
            Map(c => c.PasswordVerificationToken);
            Map(c => c.PasswordVerificationTokenExpirationDate);
            HasMany(c => c.OAuthMemberships).KeyColumn("UserId").Cascade.Delete();
            //HasOne(c => c.Profile);
            //References(c => c.Profile).Not.Nullable();
            HasMany(c => c.MessageLogs).KeyColumn("SentBy_id").Inverse().Cascade.Delete();
        }
    }

    //public class UsersProfileMapping : ClassMap<Web.Models.UsersProfile>
    //{
    //    public UsersProfileMapping()
    //    {
    //        Table("UserProfile");
    //        Id(c => c.Id, "UserId").GeneratedBy.Foreign("User");
    //        HasOne(c => c.User).Constrained();
    //        Map(c => c.UserName);
    //    }
    //}

    public class OAuthMembershipMapping : ClassMap<OAuthMembership>
    {
        public OAuthMembershipMapping()
        {
            Table("webpages_OAuthMembership");
            CompositeId()
                .KeyReference(c => c.User, "UserId")
                .KeyProperty(c => c.Provider, "Provider");
            Map(c => c.ProviderUserId);
        }
    }
}