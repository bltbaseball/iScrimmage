using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Expressions;
using iScrimmage.Core.Security;
using iScrimmage.Core.Extensions;
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
            Delete.Table("Roster").InSchema("blt");
            Delete.Table("TeamMember").InSchema("blt");
            Delete.Table("Role").InSchema("blt");
            Delete.Table("Team").InSchema("blt");
            Delete.Table("Location").InSchema("blt");
            Delete.Table("Class").InSchema("blt");
            Delete.Table("LeagueUmpire").InSchema("blt");
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
            //Debugger.Launch();

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
                .WithColumn("Gender").AsFixedLengthString(1).Nullable()
                .WithColumn("Photo").AsString().Nullable()
                .WithColumn("LookingForTeam").AsBoolean().WithDefaultValue(0)
                .WithColumn("Umpire").AsBoolean().WithDefaultValue(0).Indexed("IX_Member_Umpire")
                .WithColumn("SiteAdmin").AsBoolean().WithDefaultValue(0)
                .WithColumn("OldId").AsInt32().Nullable().Indexed("IX_Member_OldId")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Contact")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_Contact_Member", "blt", "Member", "Id")
                .WithColumn("Type").AsString(125).NotNullable().Indexed("IX_Contact_Type")
                .WithColumn("PhoneNumber").AsString(125).Nullable()
                .WithColumn("Address").AsString(125).Nullable()
                .WithColumn("City").AsString(125).Nullable()
                .WithColumn("State").AsFixedLengthString(2).Nullable()
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

            Create.iScrimmageTable("blt", "LeagueUmpire")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_LeagueUmpire_Member", "blt", "Member", "Id")
                .WithColumn("LeagueId").AsGuid().NotNullable().ForeignKey("FK_LeagueUmpire_League", "blt", "League", "Id")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Class")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Class_Name")
                .WithColumn("Handicap").AsInt32().NotNullable().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "LeagueDivision")
                .WithColumn("LeagueId").AsGuid().NotNullable().ForeignKey("FK_LeagueDivision_League", "blt", "League", "Id")
                .WithColumn("DivisionId").AsGuid().NotNullable().ForeignKey("FK_LeagueDivision_Division", "blt", "Division", "Id")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Location")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Location_Name")
                .WithColumn("Url").AsString(250).NotNullable()
                .WithColumn("PhoneNumber").AsString(125).Nullable()
                .WithColumn("Address").AsString(125).Nullable()
                .WithColumn("City").AsString(125).Nullable()
                .WithColumn("State").AsFixedLengthString(2).Nullable()
                .WithColumn("Zip").AsString(10).Nullable()
                .WithColumn("Notes").AsString(500).Nullable()
                .WithColumn("Latitude").AsDecimal(18,5).Nullable()
                .WithColumn("Longitude").AsDecimal(18,5).Nullable()
                .WithAuditColumns();

            // FluentMigrator does not support the geography type.
            Execute.Sql("ALTER TABLE blt.Location ADD Point geography");

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

            Create.iScrimmageTable("blt", "Roster")
                .WithColumn("TeamMemberId").AsGuid().NotNullable().ForeignKey("FK_Roster_TeamMember", "blt", "TeamMember", "Id")
                .WithColumn("LeagueId").AsGuid().NotNullable().ForeignKey("FK_Roster_League", "blt", "League", "Id")
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

        private void migrateLeagueData(IDbConnection conn, IDbTransaction tran)
        {
            migrateLeaguesData(conn, tran);
            migrateDivisionsData(conn, tran);
            migrateLeagueDivisionsData(conn, tran);
            migrateClassesData(conn, tran);
            migrateLocationsData(conn, tran);
        }

        private void migrateMemberData(IDbConnection conn, IDbTransaction tran)
        {
            migrateCoachesData(conn, tran);
            migrateGuardiansData(conn, tran);
            migrateManagersData(conn, tran);
            migratePlayersData(conn, tran);
            migrateUmpiresData(conn, tran);
        }

        private void migrateLeaguesData(IDbConnection conn, IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>(); 

            var columns = new
            {
                Id = 0,
                Type = 1,
                Name = 2,
                Description = 3,
                Url = 4,
                StartDate = 5,
                EndDate = 6,
                RegistrationStartDate = 7,
                RegistrationEndDate = 8,
                RosterLockedOn = 9,
                IsActive = 10,
                MinimumDatesAvailable = 11,
                WaiverRequired = 12
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Type],[Name],[HtmlDescription],[Url],[StartDate],[EndDate],[RegistrationStartDate],[RegistrationEndDate],[RosterLockedOn],[IsActive],[MinimumDatesAvailable],[WaiverRequired] FROM [dbo].[Leagues]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "League" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        record.Id = newId;
                        record.Type = reader.GetValueOrNull<string>(columns.Type);
                        record.Name = reader.GetValueOrNull<string>(columns.Name);
                        record.HtmlDescription = reader.GetValueOrNull<string>(columns.Description);
                        record.Url = reader.GetValueOrNull<string>(columns.Url);
                        record.StartDate = reader.GetValueOrNull<DateTime>(columns.StartDate);
                        record.EndDate = reader.GetValueOrNull<DateTime>(columns.EndDate);
                        record.RegistrationStartDate = reader.GetValueOrNull<DateTime>(columns.RegistrationStartDate);
                        record.RegistrationEndDate = reader.GetValueOrNull<DateTime>(columns.RegistrationEndDate);
                        record.RosterLockedOn = reader.GetValueOrNull<DateTime>(columns.RosterLockedOn);
                        record.MinimumDatesAvailable = reader.GetValueOrNull<Int16>(columns.MinimumDatesAvailable);
                        record.IsActive = reader.GetValueOrNull<Boolean>(columns.IsActive);
                        record.WaiverRequired = reader.GetValueOrNull<Boolean>(columns.WaiverRequired);
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "League", record);
                }
            }
        }

        private void migrateDivisionsData(IDbConnection conn, IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Name = 1,
                MaxAge = 2
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Name],[MaxAge] FROM [dbo].[Divisions]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Division_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        record.Id = newId;
                        record.Name = reader.GetValueOrNull<string>(columns.Name);
                        record.MaxAge = reader.GetValueOrNull<int>(columns.MaxAge);
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Division", record);
                }
            }
        }

        private void migrateLeagueDivisionsData(IDbConnection conn, IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                LeagueId = 0,
                DivisionId = 1
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [League_id],[Division_id] FROM [dbo].[LeagueDivisions]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldLeagueId = "League_" + reader.GetInt32(columns.LeagueId);
                        var newLeagueId = getMappedId(oldLeagueId);
                        var oldDivisionId = "Division_" + reader.GetInt32(columns.DivisionId);
                        var newDivisionId = getMappedId(oldDivisionId);

                        dynamic record = new ExpandoObject();

                        record.DivisionId = Guid.NewGuid();
                        record.LeagueId = newLeagueId;
                        record.DivisionId = newDivisionId;
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "LeagueDivision", record);
                }
            }
        }

        private void migrateClassesData(IDbConnection conn, IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Name = 1,
                Handicap = 2
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Name],[Handicap] FROM [dbo].[TeamClasses]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Class_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        record.Id = newId;
                        record.Name = reader.GetValueOrNull<string>(columns.Name);
                        record.Handicap = reader.GetValueOrNull<int>(columns.Handicap);
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Class", record);
                }
            }
        }

        private void migrateLocationsData(IDbConnection conn, IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Name = 1,
                Url = 2,
                PhoneNumber = 3,
                Address = 4,
                City = 5,
                State = 6,
                Zip = 7,
                Notes = 8,
                Latitiude = 9,
                Longitude = 10,
                Point = 11
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Name],[Url],[GroundsKeeperPhone],[Address],[City],[State],[Zip],[Notes],[Latitude],[Longitude],[Point] FROM [dbo].[Locations]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Location_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        record.Id = newId;
                        record.Name = reader.GetValueOrNull<string>(columns.Name);
                        record.Url = reader.GetValueOrNull<string>(columns.Url);
                        record.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        record.Address = reader.GetValueOrNull<string>(columns.Address);
                        record.City = reader.GetValueOrNull<string>(columns.City);
                        record.State = reader.GetValueOrNull<string>(columns.State);
                        record.Zip = reader.GetValueOrNull<string>(columns.Zip);
                        record.Notes = reader.GetValueOrNull<string>(columns.Notes);
                        record.Latitiude = reader.GetValueOrNull<string>(columns.Latitiude);
                        record.Longitude = reader.GetValueOrNull<string>(columns.Longitude);
                        record.Point = reader.GetValueOrNull<string>(columns.Point);
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Location", record);
                }
            }
        }

        private void migrateCoachesData(IDbConnection conn, IDbTransaction tran)
        {
            var insertMemberRecords = new List<ExpandoObject>();
            var insertContactRecords = new List<ExpandoObject>();
            var insertInviteRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Email = 1,
                FirstName = 2,
                LastName = 3,
                PhoneNumber = 4,
                Photo = 5,
                PhotoType = 6,
                InvitationSentOn = 7,
                InviteToken = 8,
                UserId = 9
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Coaches]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Coach_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic invite = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetString(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetString(columns.FirstName);
                        member.LastName = reader.GetString(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        member.Photo = reader.GetString(columns.Photo) + "." + reader.GetString(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetString(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        invite.Id = Guid.NewGuid();
                        invite.FromMemberId = systemId;
                        invite.ToTmail = reader.GetString(columns.Email);
                        invite.Token = reader.GetString(columns.InviteToken);
                        invite.SentOn = reader.GetDateTime(columns.InvitationSentOn);
                        invite.CreatedBy = systemId;
                        invite.CreatedOn = DateTime.Today;

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertInviteRecords.Add(invite);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateGuardiansData(IDbConnection conn, IDbTransaction tran)
        {
            var insertMemberRecords = new List<ExpandoObject>();
            var insertContactRecords = new List<ExpandoObject>();
            var insertInviteRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Email = 1,
                FirstName = 2,
                LastName = 3,
                PhoneNumber = 4,
                Address = 5,
                City = 6,
                State = 7,
                Zip = 8,
                InvitationSentOn = 9,
                InviteToken = 10,
                UserId = 11
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Address],[City],[State],[Zip],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Guardians]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Guardian_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic address = new ExpandoObject();
                        dynamic invite = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetString(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetString(columns.FirstName);
                        member.LastName = reader.GetString(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetString(columns.PhoneNumber);

                        address.Id = Guid.NewGuid();
                        address.MemberId = newId;
                        address.Type = "Home";
                        address.Address = reader.GetString(columns.Address);
                        address.City = reader.GetString(columns.City);
                        address.State = reader.GetString(columns.State);
                        address.Zip = reader.GetString(columns.Zip);

                        invite.Id = Guid.NewGuid();
                        invite.FromMemberId = systemId;
                        invite.ToTmail = reader.GetString(columns.Email);
                        invite.Token = reader.GetString(columns.InviteToken);
                        invite.SentOn = reader.GetDateTime(columns.InvitationSentOn);

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertContactRecords.Add(address);
                        insertInviteRecords.Add(invite);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateManagersData(IDbConnection conn, IDbTransaction tran)
        {
            var insertMemberRecords = new List<ExpandoObject>();
            var insertContactRecords = new List<ExpandoObject>();
            var insertInviteRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Email = 1,
                FirstName = 2,
                LastName = 3,
                PhoneNumber = 4,
                Photo = 5,
                PhotoType = 6,
                InvitationSentOn = 7,
                InviteToken = 8,
                UserId = 9
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Managers]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Manager_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic invite = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetString(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetString(columns.FirstName);
                        member.LastName = reader.GetString(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        member.Photo = reader.GetString(columns.Photo) + "." + reader.GetString(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetString(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        invite.Id = Guid.NewGuid();
                        invite.FromMemberId = systemId;
                        invite.ToTmail = reader.GetString(columns.Email);
                        invite.Token = reader.GetString(columns.InviteToken);
                        invite.SentOn = reader.GetDateTime(columns.InvitationSentOn);
                        invite.CreatedBy = systemId;
                        invite.CreatedOn = DateTime.Today;

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertInviteRecords.Add(invite);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migratePlayersData(IDbConnection conn, IDbTransaction tran)
        {
            var insertMemberRecords = new List<ExpandoObject>();
            var insertContactRecords = new List<ExpandoObject>();
            var insertInviteRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                GuardianId = 1,
                Email = 2,
                FirstName = 3,
                LastName = 4,
                DateOfBirth = 5,
                Gender = 6,
                PhoneNumber = 7,
                JerseyNumber = 8,
                InvitationSentOn = 9,
                InviteToken = 10,
                IsLookingForTeam = 11,
                UserId = 12
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Guardian_id],[Email],[FirstName],[LastName],[DateOfBirth],[Gender],[PhoneNumber],[JerseyNumber],[InvitationSentOn],[InviteToken],[IsLookingForTeam],[User_id] FROM [dbo].[Players]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Player_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic invite = new ExpandoObject();

                        member.Id = newId;
                        member.OldId = reader.GetInt32(columns.Id);
                        member.GuardianId = getMappedId("Guardian_" + reader.GetInt32(columns.GuardianId));
                        member.Email = reader.GetString(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetString(columns.FirstName);
                        member.LastName = reader.GetString(columns.LastName);
                        member.Gender = reader.GetString(columns.Gender).ToLower() == "male" ? "M" : "F";
                        member.DateOfBirth = reader.GetDateTime(columns.DateOfBirth);
                        member.LookingForTeam = reader.GetBoolean(columns.IsLookingForTeam);
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetString(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        invite.Id = Guid.NewGuid();
                        invite.FromMemberId = systemId;
                        invite.ToTmail = reader.GetString(columns.Email);
                        invite.Token = reader.GetString(columns.InviteToken);
                        invite.SentOn = reader.GetDateTime(columns.InvitationSentOn);
                        invite.CreatedBy = systemId;
                        invite.CreatedOn = DateTime.Today;

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertInviteRecords.Add(invite);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateUmpiresData(IDbConnection conn, IDbTransaction tran)
        {
            var insertMemberRecords = new List<ExpandoObject>();
            var insertContactRecords = new List<ExpandoObject>();
            var insertInviteRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Email = 1,
                FirstName = 2,
                LastName = 3,
                PhoneNumber = 4,
                Photo = 5,
                PhotoType = 6,
                InvitationSentOn = 7,
                InviteToken = 8,
                UserId = 9
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Umpires]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "Umpire_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic invite = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetString(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetString(columns.FirstName);
                        member.LastName = reader.GetString(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        member.Photo = reader.GetString(columns.Photo) + "." + reader.GetString(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.Umpire = true;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetString(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        invite.Id = Guid.NewGuid();
                        invite.FromMemberId = systemId;
                        invite.ToTmail = reader.GetString(columns.Email);
                        invite.Token = reader.GetString(columns.InviteToken);
                        invite.SentOn = reader.GetDateTime(columns.InvitationSentOn);
                        invite.CreatedBy = systemId;
                        invite.CreatedOn = DateTime.Today;

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertInviteRecords.Add(invite);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void updateAdminMembers(IDbConnection conn, IDbTransaction tran)
        {
            
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

        private void insertRecord(IDbCommand command, string schema, string table, ExpandoObject record)
        {
            var sql = "insert into {0}.{1} ({2}) values ({3})";
            var columns = new List<string>();
            var values = new List<string>();

            foreach (KeyValuePair<string, object> kvp in record)
            {
                var paramName = "@" + kvp.Key;
                var paramValue = kvp.Value;

                if (paramValue != null)
                {
                    command.Parameters.Add(new SqlParameter(paramName, paramValue));

                    columns.Add(kvp.Key);
                    values.Add(paramName);
                }
            }

            var colString = "[" + String.Join("],[", columns.ToArray()) + "]";
            var valString = String.Join(",", values.ToArray());

            command.CommandText = String.Format(sql, schema, table, colString, valString);
            command.ExecuteNonQuery();
        }

        private void updateRecord(IDbCommand command, string schema, string table, ExpandoObject record)
        {
            var sql = "update {0}.{1} set {2} where [Id] = @Id";
            var sets = new List<string>();

            foreach (KeyValuePair<string, object> kvp in record)
            {
                var paramName = "@" + kvp.Key;
                var paramValue = kvp.Value;

                if (paramValue != null)
                {
                    command.Parameters.Add(new SqlParameter(paramName, paramValue));

                    sets.Add("[" + kvp.Key + "] = " + paramName);
                }
            }

            var setString = String.Join(", ", sets.ToArray());

            command.CommandText = String.Format(sql, schema, table, setString);
            command.ExecuteNonQuery();
        }
    }
}
