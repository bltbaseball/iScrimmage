using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Expressions;
using iScrimmage.Core.Security;
using iScrimmage.Migrations.Extensions;

namespace iScrimmage.Data.Migrations
{
    [Migration(201407281700, "Initial Schema Tables")]
    public class Migration201407281700 : Migration
    {
        private string systemId = "71A54F04-6177-4289-8FF9-6A35245369E6";

        private Dictionary<string, Guid> idMap;
 
        public override void Down()
        {
            Delete.Table("Photo").InSchema("blt");
            Delete.Table("TeamMember").InSchema("blt");
            Delete.Table("Role").InSchema("blt");
            Delete.Table("Team").InSchema("blt");
            Delete.Table("Location").InSchema("blt");
            Delete.Table("Class").InSchema("blt");
            Delete.Table("LeagueDivision").InSchema("blt");
            Delete.Table("Division").InSchema("blt");
            Delete.Table("League").InSchema("blt");
            Delete.Table("Invite").InSchema("blt");
            Delete.Table("Contact").InSchema("blt");
            Delete.Table("Member").InSchema("blt");

            Delete.Schema("blt");
        }

        public override void Up()
        {
            idMap = new Dictionary<string, Guid>();

            Create.Schema("blt");

            Create.iScrimmageTable("blt", "Member")
                .WithColumn("GuardianId").AsGuid().Nullable().ForeignKey("FK_Member_Guardian", "blt", "Member", "Id")
                .WithColumn("Email").AsString(125).Nullable().Indexed("IX_Member_Email")
                .WithColumn("Password").AsString(250).Nullable()
                .WithColumn("VerificationToken").AsString(250).Nullable().Indexed("IX_Member_VerificationToken")
                .WithColumn("EmailVerified").AsBoolean().WithDefaultValue(0)
                .WithColumn("ResetToken").AsString(250).Nullable().Indexed("IX_Member_ResetToken")
                .WithColumn("ResetTokenExpiresOn").AsDateTime().Nullable()
                .WithColumn("FirstName").AsString(125).Nullable()
                .WithColumn("LastName").AsString(125).Nullable()
                .WithColumn("DateOfBirth").AsDate().Nullable()
                .WithColumn("LookingForTeam").AsBoolean().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Contact")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_Contact_Member", "blt", "Member", "Id")
                .WithColumn("Type").AsString(125).NotNullable().Indexed("IX_Contact_Type")
                .WithColumn("PhoneNumber").AsString(125).Nullable()
                .WithColumn("Address").AsString(125).Nullable()
                .WithColumn("City").AsString(125).Nullable()
                .WithColumn("State").AsString(2).Nullable()
                .WithColumn("Zip").AsString(10).Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Invite")
                .WithColumn("FromMemberId").AsGuid().NotNullable().ForeignKey("FK_Invite_FromMember", "blt", "Member", "Id")
                .WithColumn("Token").AsString(250).NotNullable().Indexed("IX_Invite_Token")
                .WithColumn("ToEmail").AsString(125).NotNullable().Indexed("IX_Invite_ToEmail")
                .WithColumn("SentOn").AsDate().NotNullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "League")
                .WithColumn("Type").AsString(125).NotNullable().Indexed("IX_League_Type")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_League_Name")
                .WithColumn("HtmlDescription").AsString(int.MaxValue).Nullable()
                .WithColumn("Url").AsString(250).Nullable()
                .WithColumn("StartDate").AsDate().NotNullable()
                .WithColumn("EndDate").AsDate().NotNullable()
                .WithColumn("RegistrationStartDate").AsDate().NotNullable()
                .WithColumn("RegistrationEndDate").AsDate().NotNullable()
                .WithColumn("RosterLockedOn").AsDate().Nullable()
                .WithColumn("MinimumDatesAvailable").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)
                .WithColumn("WaiverRequired").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Division")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Division_Name")
                .WithColumn("MaxAge").AsInt32().NotNullable().WithDefaultValue(17)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "LeagueDivision")
                .WithColumn("LeagueId").AsGuid().NotNullable().ForeignKey("FK_LeagueDivision_League", "blt", "League", "Id")
                .WithColumn("DivisionId").AsGuid().NotNullable().ForeignKey("FK_LeagueDivision_Division", "blt", "Division", "Id")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Class")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Class_Name")
                .WithColumn("Handicap").AsInt32().NotNullable().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Location")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Location_Name")
                .WithColumn("Url").AsString(250).NotNullable()
                .WithColumn("PhoneNumber").AsString(125).Nullable()
                .WithColumn("Address").AsString(125).Nullable()
                .WithColumn("City").AsString(125).Nullable()
                .WithColumn("State").AsString(2).Nullable()
                .WithColumn("Zip").AsString(10).Nullable()
                .WithColumn("Notes").AsString(500).Nullable()
                .WithColumn("Latitude").AsDecimal(18,5).Nullable()
                .WithColumn("Longitude").AsDecimal(18,5).Nullable()
                .WithColumn("Point").AsString(125).Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Team")
                .WithColumn("ClassId").AsGuid().NotNullable().ForeignKey("FK_Team_Class", "blt", "Class", "Id")
                .WithColumn("DivisionId").AsGuid().NotNullable().ForeignKey("FK_Team_Division", "blt", "Division", "Id")
                .WithColumn("LeagueId").AsGuid().NotNullable().ForeignKey("FK_Team_League", "blt", "League", "Id")
                .WithColumn("LocationId").AsGuid().Nullable().ForeignKey("FK_Team_Location", "blt", "Location", "Id")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Team_Name")
                .WithColumn("HtmlDescription").AsString(int.MaxValue).Nullable()
                .WithColumn("Url").AsString(250).Nullable()
                .WithColumn("LookingForPlayers").AsBoolean().WithDefaultValue(0)
                .WithColumn("RosterIsLocked").AsBoolean().WithDefaultValue(0)
                .WithColumn("RosterLockedOn").AsDate().Nullable()
                .WithColumn("RosterLockedBy").AsGuid().Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Role")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Role_Name")
                .WithColumn("Description").AsString(1000).Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "TeamMember")
                .WithColumn("TeamId").AsGuid().Nullable().ForeignKey("FK_TeamMember_Team", "blt", "Team", "Id")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_TeamMember_Member", "blt", "Member", "Id")
                .WithColumn("RoleId").AsGuid().Nullable().ForeignKey("FK_TeamMember_Role", "blt", "Role", "Id")
                .WithColumn("PhotoVerified").AsBoolean().WithDefaultValue(0)
                .WithColumn("JerseyNumber").AsInt32().WithDefaultValue(0)
                .WithColumn("Status").AsString(125).Nullable()
                .WithColumn("WaiverStatus").AsString(125).Nullable()
                .WithColumn("SignedWaiverId").AsString(250).Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Photo")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_Photo_Member", "blt", "Member", "Id")
                .WithColumn("LeagueId").AsGuid().Nullable().ForeignKey("FK_Photo_League", "blt", "League", "Id")
                .WithColumn("TeamId").AsGuid().Nullable().ForeignKey("FK_Photo_Team", "blt", "Team", "Id")
                .WithColumn("Type").AsString(125).Nullable()
                .WithColumn("Url").AsString(125).Nullable()
                .WithAuditColumns();

            var systemMember = new
            {
                Id = systemId,
                Email = "adrian@bltbaseball.com",
                Password = PasswordHash.CreateHash("b@53b@!!"),
                VerificationToken = "xxx",
                EmailVerified = true,
                FirstName = "Adrian",
                LastName = "Farmer",
                DateOfBirth = "1/1/1900",
                LookingForTeam = false,
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            };

            Insert.IntoTable("Member").InSchema("blt").Row(systemMember);

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Coach"),
                Name = "Coach",
                Description = "Team Coach",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Manager"),
                Name = "Manager",
                Description = "Team Manager",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Player"),
                Name = "Player",
                Description = "Team Player",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Umpire"),
                Name = "Umpire",
                Description = "League Umpire",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            //Execute.WithConnection(migrateMemberData);
        }

        private void migrateMemberData(IDbConnection conn, IDbTransaction tran)
        {
            migrateCoachesData(conn, tran);
        }

        private void migrateCoachesData(IDbConnection conn, IDbTransaction tran)
        {
            var insert = new StringBuilder();

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[CreatedOn],[InviteToken],[InvitationSentOn],[User_id],[CreatedBy_id] FROM [dbo].[Coaches]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Coach_" + reader.GetInt32(0);
                        var newId = getMappedId(oldId);

                        var member = new
                        {
                            Id = newId,
                            Email = reader.GetString(1),
                            Password = PasswordHash.CreateHash(oldId),
                            EmailVerified = false,
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            DateOfBirth = "1/1/1900",
                            LookingForTeam = false,
                            CreatedBy = systemId,
                            CreatedOn = DateTime.Today
                        };

                        var contact = new
                        {
                            Id = Guid.NewGuid(),
                            MemberId = newId,
                            Type = "Mobile",
                            PhoneNumber = reader.GetString(4)
                        };

                        var invite = new
                        {
                            Id = Guid.NewGuid(),
                            FromMemberId = systemId,
                            ToTmail = reader.GetString(1),
                            Token = reader.GetString(8),
                            SentOn = reader.GetDateTime(9)
                        };

                        var photo = new
                        {
                            Id = Guid.NewGuid(),
                            MemberId = newId,
                            Type = reader.GetString(6),
                            Url = reader.GetString(5)
                        };

                        Insert.IntoTable("Member").InSchema("blt").Row(member);
                        Insert.IntoTable("Contact").InSchema("blt").Row(contact);
                        Insert.IntoTable("Invite").InSchema("blt").Row(invite);
                        Insert.IntoTable("Photo").InSchema("blt").Row(photo);
                    }
                }
            }
        }

        private Guid getMappedId(string key)
        {
            if (idMap.ContainsKey(key))
            {
                return idMap[key];
            }
            else
            {
                var newId = Guid.NewGuid();

                idMap[key] = newId;

                return newId;
            }
        }
    }
}
