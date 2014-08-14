using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using FluentMigrator;
using iScrimmage.Core.Extensions;
using iScrimmage.Core.Security;
using iScrimmage.Migrations.Extensions;

namespace iScrimmage.Data.Migrations
{
    [Migration(201407281700, "Initial Schema Tables")]
    public class Migration201407281700 : Migration
    {
        private const string systemId = "71A54F04-6177-4289-8FF9-6A35245369E6";

        private Dictionary<string, Guid> idMap;


        public override void Down()
        {
            // Delete.Table("Photo").InSchema("blt");  //does not exist
            
            Delete.Table("RosterMember").InSchema("blt");
            Delete.Table("Roster").InSchema("blt");
            Delete.Table("Role").InSchema("blt");
            Delete.Table("Team").InSchema("blt");
            Delete.Table("Location").InSchema("blt");
            Delete.Table("EventUmpire").InSchema("blt");
            Delete.Table("EventDivisionClass").InSchema("blt");
            Delete.Table("Class").InSchema("blt");
            Delete.Table("Division").InSchema("blt");
            Delete.Table("Event").InSchema("blt");
            Delete.Table("EventType").InSchema("blt");
            Delete.Table("Invite").InSchema("blt");
            Delete.Table("Contact").InSchema("blt");
            Delete.Table("MemberWaiver").InSchema("blt");
            Delete.Table("Waiver").InSchema("blt");
            Delete.Table("WaiverStatus").InSchema("blt");
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
                .WithColumn("OldId").AsString().Nullable().Indexed("IX_Member_OldId")
                .WithColumn("Status").AsString(125).Nullable()
                .WithColumn("PhotoVerified").AsBoolean().WithDefaultValue(0)
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
                .WithColumn("FromMemberId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("FK_Invite_FromMember", "blt", "Member", "Id")
                .WithColumn("Token").AsString(250).NotNullable().Indexed("IX_Invite_Token")
                .WithColumn("ToEmail").AsString(125).NotNullable().Indexed("IX_Invite_ToEmail")
                .WithColumn("SentOn").AsDate().NotNullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "EventType")
                .WithColumn("Description").AsString(60).NotNullable();

            Create.iScrimmageTable("blt", "Event")
                .WithColumn("Type").AsGuid().NotNullable().ForeignKey("FK_Event_EventType", "blt", "EventType", "Id")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Event_Name")
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

            Create.iScrimmageTable("blt", "EventUmpire")
                .WithColumn("MemberId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("FK_EventUmpire_Member", "blt", "Member", "Id")
                .WithColumn("EventId").AsGuid().NotNullable().ForeignKey("FK_EventUmpire_League", "blt", "Event", "Id")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Class")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Class_Name")
                .WithColumn("Handicap").AsInt32().NotNullable().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "EventDivisionClass")
                .WithColumn("EventId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("FK_EventDivisionClass_Event", "blt", "Event", "Id")
                .WithColumn("DivisionId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("FK_EventDivisionClass_Division", "blt", "Division", "Id")
                .WithColumn("ClassId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("FK_EventDivisionClass_Class", "blt", "Class", "Id")
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Location")
                .WithColumn("Name").AsString(125).NotNullable()
                .WithColumn("Url").AsString(250).Nullable()
                .WithColumn("PhoneNumber").AsString(125).Nullable()
                .WithColumn("Address").AsString(125).Nullable()
                .WithColumn("City").AsString(125).Nullable()
                .WithColumn("State").AsFixedLengthString(2).Nullable()
                .WithColumn("Zip").AsString(10).Nullable()
                .WithColumn("Notes").AsString(500).Nullable()
                .WithColumn("Latitude").AsDecimal(18, 5).Nullable()
                .WithColumn("Longitude").AsDecimal(18, 5).Nullable()
                .WithAuditColumns();

            // FluentMigrator does not support the geography type.
            //Execute.Sql("ALTER TABLE blt.Location ADD Point geography");

            Create.iScrimmageTable("blt", "Team")

                .WithColumn("LocationId").AsGuid().Nullable().ForeignKey("FK_Team_Location", "blt", "Location", "Id")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Team_Name")
                .WithColumn("HtmlDescription").AsString(int.MaxValue).Nullable()
                .WithColumn("Url").AsString(250).Nullable()
                .WithColumn("LookingForPlayers").AsBoolean().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Role")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Role_Name")
                .WithColumn("Description").AsString(1000).Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Roster")
                .WithColumn("RosterName").AsString(250).NotNullable()
                .WithColumn("TeamId").AsGuid().Nullable().ForeignKey("FK_Roster_Team", "blt", "Team", "Id")
                .WithColumn("EventDivisionClassId").AsGuid().NotNullable().ForeignKey("FK_Roster_EventDivisionClass", "blt", "EventDivisionClass", "Id")
                .WithColumn("RosterIsLocked").AsBoolean().WithDefaultValue(0)
                .WithColumn("RosterLockedOn").AsDate().Nullable()
                .WithColumn("RosterLockedBy").AsGuid().Nullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "RosterMember")
                .WithColumn("RosterId").AsGuid().NotNullable().ForeignKey("FK_RosterMember_Roster", "blt", "Roster", "Id")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_RosterMember_Member", "blt", "Member", "Id")
                .WithColumn("RoleId").AsGuid().Nullable().ForeignKey("FK_RosterMember_Role", "blt", "Role", "Id")
                .WithColumn("JerseyNumber").AsInt32().WithDefaultValue(0)
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "Waiver")
                .WithColumn("StartDate").AsDate().NotNullable()
                .WithColumn("EndDate").AsDate().NotNullable()
                .WithAuditColumns();

            Create.iScrimmageTable("blt", "WaiverStatus")
                .WithColumn("Description").AsString(125).NotNullable();

            Create.iScrimmageTable("blt", "MemberWaiver")
                .WithColumn("MemberId").AsGuid().NotNullable().ForeignKey("FK_MemberWaiver_Member", "blt", "Member", "Id")
                .WithColumn("WaiverId").AsGuid().NotNullable().ForeignKey("FK_MemberWaiver_Waiver", "blt", "Waiver", "Id")
                .WithColumn("WaiverStatusId").AsGuid().NotNullable().ForeignKey("FK_MemberWaiver_WaiverStatus", "blt", "WaiverStatus", "Id")
                .WithColumn("SignedWaiverId").AsString(250).Nullable()
                .WithAuditColumns();

            addDefaultData();
            Execute.WithConnection(migrateLeagueData);
            Execute.WithConnection(migrateMemberData);


            //handle geography items
            //Execute.Sql("UPDATE blt.[Location] SET [Point] = geography::STPointFromText('POINT(' + CAST([Longitude] AS VARCHAR(20)) + ' ' +  CAST([Latitude] AS VARCHAR(20)) + ')', 4326)");
        }

        private void addDefaultData()
        {

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

            Insert.IntoTable("WaiverStatus").InSchema("blt").Row(new
            {
                Id = getMappedId("WaiverStatus_NotSigned"),
                Description = "Not Signed"
            });
            Insert.IntoTable("WaiverStatus").InSchema("blt").Row(new
            {
                Id = getMappedId("WaiverStatus_Sent"),
                Description = "Sent"
            });
            Insert.IntoTable("WaiverStatus").InSchema("blt").Row(new
            {
                Id = getMappedId("WaiverStatus_Viewed"),
                Description = "Viewed"
            });
            Insert.IntoTable("WaiverStatus").InSchema("blt").Row(new
            {
                Id = getMappedId("WaiverStatus_Signed"),
                Description = "Signed"
            });

            Insert.IntoTable("EventType").InSchema("blt").Row(new
            {
                Id = getMappedId("EventType_League"),
                Description = "League"
            });

            Insert.IntoTable("EventType").InSchema("blt").Row(new
            {
                Id = getMappedId("EventType_Tournament"),
                Description = "Tournament"
            });

            Insert.IntoTable("EventType").InSchema("blt").Row(new
            {
                Id = getMappedId("EventType_Scrimmage"),
                Description = "Scrimmage"
            });

            Insert.IntoTable("Waiver").InSchema("blt").Row(new
            {
                Id = getMappedId("initialWaiver"),
                StartDate = new DateTime(2014, 8, 1),
                EndDate = new DateTime(2015, 7, 31),
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });
        }

        private void migrateLeagueData(IDbConnection conn, IDbTransaction tran)
        {
            migrateLeaguesData(tran);
            migrateDivisionsData(tran);
            migrateLeagueDivisionsData(tran);
            migrateClassesData(tran);
            migrateLocationsData(tran);
        }

        private void migrateMemberData(IDbConnection conn, IDbTransaction tran)
        {
            migrateCoachesData(tran);
            migrateGuardiansData(tran);
            migrateManagersData(tran);
            migratePlayersData(tran);
            migrateUmpiresData(tran);
            migrateWaiverData(tran);
        }

        private void migrateLeaguesData(IDbTransaction tran)
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
                cmd.CommandText =
                    @"SELECT [Id],[Type],[Name],[HtmlDescription],[Url],[StartDate],[EndDate],[RegistrationStartDate],[RegistrationEndDate],[RosterLockedOn],[IsActive],[MinimumDatesAvailable],[WaiverRequired] FROM [dbo].[Leagues]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Event" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        record.Id = newId;
                        record.Type =getEventType(reader.GetValueOrNull<string>(columns.Type).ToString());
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

            foreach (ExpandoObject record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Event", record);
                }
            }
        }

        private Guid getEventType(string evt)
        {
            switch (evt)
            {
                case "League":
                    return getMappedId("EventType_League");
                case "Tournament":
                    return getMappedId("EventType_Tournament");
                default:
                    return getMappedId("EventType_Scrimmage");
                
            }
        
        }

        private void migrateDivisionsData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
                          {
                              Id = 0,
                              Name = 1,
                              MaxAge = 2
                          };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [Id],[Name],[MaxAge] FROM [dbo].[Divisions]";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Division_" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

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
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Division", record);
                }
            }
        }

        private void migrateLeagueDivisionsData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
                          {
                              LeagueId = 0,
                              DivisionId = 1
                          };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT [League_id],[Division_id] FROM [dbo].[LeagueDivisions]";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldLeagueId = "League_" + reader.GetInt32(columns.LeagueId);
                        Guid newLeagueId = getMappedId(oldLeagueId);
                        string oldDivisionId = "Division_" + reader.GetInt32(columns.DivisionId);
                        Guid newDivisionId = getMappedId(oldDivisionId);

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
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "LeagueDivision", record);
                }
            }
        }

        private void migrateClassesData(IDbTransaction tran)
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
                        string oldId = "Class_" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

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

            foreach (ExpandoObject record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Class", record);
                }
            }
        }

        private void migrateLocationsData(IDbTransaction tran)
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
                cmd.CommandText =
                    @"SELECT [Id],[Name],[Url],[GroundsKeeperPhone],[Address],[City],[State],[Zip],[Notes],[Latitude],[Longitude],[Point] FROM [dbo].[Locations]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Location_" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();
                        record.Id = newId;
                        record.Name = reader.GetValueOrNull<string>(columns.Name);
                        record.Url = reader.GetValueOrNull<string>(columns.Url);
                        record.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        record.Address = reader.GetValueOrNull<string>(columns.Address);
                        record.City = reader.GetValueOrNull<string>(columns.City);
                        record.State = "FL";
                        record.Zip = reader.GetValueOrNull<string>(columns.Zip);
                        record.Notes = reader.GetValueOrNull<string>(columns.Notes);
                        record.Latitude = reader.GetValueOrNull<string>(columns.Latitiude);
                        record.Longitude = reader.GetValueOrNull<string>(columns.Longitude);
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Today;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (ExpandoObject record in insertRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    insertRecord(cmd, "blt", "Location", record);
                }
            }
        }

        private void migrateCoachesData(IDbTransaction tran)
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
                cmd.CommandText =
                    @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Coaches]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Coach_" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetValueOrNull<string>(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetValueOrNull<string>(columns.FirstName);
                        member.LastName = reader.GetValueOrNull<string>(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        if (reader.GetValueOrNull<string>(columns.Photo) != null &&
                            reader.GetValueOrNull<string>(columns.PhotoType) != null)
                            member.Photo = reader.GetValueOrNull<string>(columns.Photo) + "." +
                                           reader.GetValueOrNull<string>(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        if (reader.GetValueOrNull<string>(columns.InviteToken) != null)
                        {
                            dynamic invite = new ExpandoObject();
                            invite.Id = Guid.NewGuid();
                            invite.FromMemberId = newId;
                            invite.ToEmail = reader.GetValueOrNull<string>(columns.Email);
                            invite.Token = reader.GetValueOrNull<string>(columns.InviteToken);
                            invite.SentOn = reader.GetValueOrNull<DateTime>(columns.InvitationSentOn);
                            invite.CreatedBy = systemId;
                            invite.CreatedOn = DateTime.Today;
                            insertInviteRecords.Add(invite);
                        }

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                    }
                }
            }

            foreach (ExpandoObject record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (ExpandoObject record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (ExpandoObject record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateGuardiansData(IDbTransaction tran)
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

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText =
                    @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Address],[City],[State],[Zip],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Guardians]";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Guardian_" + reader.GetValueOrNull<Int32>(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();
                        dynamic address = new ExpandoObject();


                        member.Id = newId;
                        member.Email = reader.GetValueOrNull<string>(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetValueOrNull<string>(columns.FirstName);
                        member.LastName = reader.GetValueOrNull<string>(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;
                        member.OldId = oldId;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        address.Id = Guid.NewGuid();
                        address.MemberId = newId;
                        address.Type = "Home";
                        address.Address = reader.GetValueOrNull<string>(columns.Address);
                        address.City = reader.GetValueOrNull<string>(columns.City);
                        address.State = reader.GetValueOrNull<string>(columns.State);
                        address.Zip = reader.GetValueOrNull<string>(columns.Zip);
                        address.CreatedBy = systemId;
                        address.CreatedOn = DateTime.Today;

                        if (reader.GetValueOrNull<string>(columns.InviteToken) != null)
                        {
                            dynamic invite = new ExpandoObject();
                            invite.Id = Guid.NewGuid();
                            invite.FromMemberId = newId;
                            invite.ToEmail = reader.GetValueOrNull<string>(columns.Email);
                            invite.Token = reader.GetValueOrNull<string>(columns.InviteToken);
                            invite.SentOn = reader.GetValueOrNull<DateTime>(columns.InvitationSentOn);
                            invite.CreatedBy = systemId;
                            invite.CreatedOn = DateTime.Today;
                            insertInviteRecords.Add(invite);
                        }


                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                        insertContactRecords.Add(address);
                    }
                }
            }

            foreach (var record in insertMemberRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (var record in insertContactRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (var record in insertInviteRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateManagersData(IDbTransaction tran)
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
                cmd.CommandText =
                    @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Managers]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Manager_" + reader.GetValueOrNull<Int32>(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();


                        member.Id = newId;
                        member.Email = reader.GetValueOrNull<string>(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetValueOrNull<string>(columns.FirstName);
                        member.LastName = reader.GetValueOrNull<string>(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        if (reader.GetValueOrNull<string>(columns.Photo) != null &&
                            reader.GetValueOrNull<string>(columns.PhotoType) != null)
                            member.Photo = reader.GetValueOrNull<string>(columns.Photo) + "." +
                                           reader.GetValueOrNull<string>(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;


                        if (reader.GetValueOrNull<string>(columns.InviteToken) != null)
                        {
                            dynamic invite = new ExpandoObject();
                            invite.Id = Guid.NewGuid();
                            invite.FromMemberId = newId;
                            invite.ToEmail = reader.GetValueOrNull<string>(columns.Email);
                            invite.Token = reader.GetValueOrNull<string>(columns.InviteToken);
                            invite.SentOn = reader.GetValueOrNull<DateTime>(columns.InvitationSentOn);
                            invite.CreatedBy = systemId;
                            invite.CreatedOn = DateTime.Today;
                            insertInviteRecords.Add(invite);
                        }

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                    }
                }
            }

            foreach (ExpandoObject record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (ExpandoObject record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (ExpandoObject record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migratePlayersData(IDbTransaction tran)
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
                              UserId = 12,
                              Status = 13,
                              Photo = 14,
                              PhotoVerified = 15,
                              WaiverStatus = 16,
                              SignWaiverId = 17
                          };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText =
                    @"SELECT [Id],[Guardian_id],[Email],[FirstName],[LastName],[DateOfBirth],[Gender],[PhoneNumber],[JerseyNumber],[InvitationSentOn],[InviteToken],[IsLookingForTeam],[User_id] FROM [dbo].[Players]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Player_" + reader.GetInt32(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();

                        member.Id = newId;
                        member.OldId = oldId;
                        if (reader.GetValueOrNull<Int32>(columns.GuardianId) != null)
                            member.GuardianId =
                                getMappedId("Guardian_" + reader.GetValueOrNull<Int32>(columns.GuardianId));
                        member.Email = reader.GetValueOrNull<string>(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetValueOrNull<string>(columns.FirstName);
                        member.LastName = reader.GetValueOrNull<string>(columns.LastName);
                        if (reader.GetValueOrNull<string>(columns.Gender) != null)
                        {
                            string gender = reader.GetValueOrNull<string>(columns.Gender).ToString();
                            member.Gender = gender.ToLower() == "male" ? "M" : "F";
                        }
                        else
                        {
                            member.Gender = "M"; //safe assumption
                        }
                        member.DateOfBirth = reader.GetValueOrNull<DateTime>(columns.DateOfBirth);
                        member.LookingForTeam = reader.GetValueOrNull<bool>(columns.IsLookingForTeam);
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        if (reader.GetValueOrNull<string>(columns.InviteToken) != null)
                        {
                            dynamic invite = new ExpandoObject();
                            invite.Id = Guid.NewGuid();
                            invite.FromMemberId = newId;
                            invite.ToEmail = reader.GetValueOrNull<string>(columns.Email);
                            invite.Token = reader.GetValueOrNull<string>(columns.InviteToken);
                            invite.SentOn = reader.GetValueOrNull<DateTime>(columns.InvitationSentOn);
                            invite.CreatedBy = systemId;
                            invite.CreatedOn = DateTime.Today;
                            insertInviteRecords.Add(invite);
                        }

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                    }
                }
            }

            foreach (ExpandoObject record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (ExpandoObject record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (ExpandoObject record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateUmpiresData(IDbTransaction tran)
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
                cmd.CommandText =
                    @"SELECT [Id],[Email],[FirstName],[LastName],[PhoneNumber],[Photo],[PhotoType],[InvitationSentOn],[InviteToken],[User_id] FROM [dbo].[Umpires]";

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "Umpire_" + reader.GetValueOrNull<Int32>(columns.Id);
                        Guid newId = getMappedId(oldId);

                        dynamic member = new ExpandoObject();
                        dynamic phone = new ExpandoObject();

                        member.Id = newId;
                        member.Email = reader.GetValueOrNull<string>(columns.Email);
                        member.Password = PasswordHash.CreateHash(oldId);
                        member.EmailVerified = false;
                        member.FirstName = reader.GetValueOrNull<string>(columns.FirstName);
                        member.LastName = reader.GetValueOrNull<string>(columns.LastName);
                        member.DateOfBirth = "1/1/1900";
                        if (reader.GetValueOrNull<string>(columns.Photo) != null &&
                            reader.GetValueOrNull<string>(columns.PhotoType) != null)
                            member.Photo = reader.GetValueOrNull<string>(columns.Photo) + "." +
                                           reader.GetValueOrNull<string>(columns.PhotoType);
                        member.LookingForTeam = false;
                        member.Umpire = true;
                        member.CreatedBy = systemId;
                        member.CreatedOn = DateTime.Today;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;

                        if (reader.GetValueOrNull<string>(columns.InviteToken) != null)
                        {
                            dynamic invite = new ExpandoObject();
                            invite.Id = Guid.NewGuid();
                            invite.FromMemberId = newId;
                            invite.ToEmail = reader.GetValueOrNull<string>(columns.Email);
                            invite.Token = reader.GetValueOrNull<string>(columns.InviteToken);
                            invite.SentOn = reader.GetValueOrNull<DateTime>(columns.InvitationSentOn);
                            invite.CreatedBy = systemId;
                            invite.CreatedOn = DateTime.Today;
                            insertInviteRecords.Add(invite);
                        }

                        insertMemberRecords.Add(member);
                        insertContactRecords.Add(phone);
                    }
                }
            }


            foreach (ExpandoObject record in insertMemberRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Member", record);
                }
            }

            foreach (ExpandoObject record in insertContactRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Contact", record);
                }
            }

            foreach (ExpandoObject record in insertInviteRecords)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Invite", record);
                }
            }
        }

        private void migrateWaiverData(IDbTransaction tran)
        {
            var insertWaiverRecord = new List<ExpandoObject>();

            var columns = new
            {
                WaiverStatus = 0,
                SignWaiverId = 1,
                MemberId = 2
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText =
                    @"SELECT TeamPlayers.WaiverStatus, TeamPlayers.SignWaiverId, blt.Member.Id FROM  blt.Member INNER JOIN TeamPlayers ON REPLACE(blt.Member.OldId, 'Player_', '') = TeamPlayers.Id WHERE (blt.Member.OldId LIKE 'Player%') AND SignWaiverId IS NOT NULL order by blt.Member.Id";


                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        dynamic waiver = new ExpandoObject();

                        waiver.Id = Guid.NewGuid();
                        waiver.MemberId = reader.GetValueOrNull<Guid>(columns.MemberId);
                        waiver.WaiverId = getMappedId("initialWaiver");
                        waiver.WaiverStatusId =
                            GetWaiverStatusCode(reader.GetValueOrNull<string>(columns.WaiverStatus).ToString());
                        waiver.SignedWaiverId = reader.GetValueOrNull<string>(columns.SignWaiverId);
                        waiver.CreatedBy = systemId;
                        waiver.CreatedOn = DateTime.Today;
                        insertWaiverRecord.Add(waiver);
                    }
                }
            }

            foreach (ExpandoObject record in insertWaiverRecord)
            {
                using (IDbCommand cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "MemberWaiver", record);
                }
            }

        }

        private Guid GetWaiverStatusCode(string code)
        {
            switch (code)
            {
                case "Signed":
                    return getMappedId("WaiverStatus_Signed");
                case "RequestSent":
                    return getMappedId("WaiverStatus_Sent");
                case "Viewed":
                    return getMappedId("WaiverStatus_Viewed");
                default:
                    return getMappedId("WaiverStatus_NotSigned");
            }
        }
        //private void updateAdminMembers(IDbConnection conn, IDbTransaction tran)
        //{

        //}

        private Guid getMappedId(string key)
        {
            if (idMap.ContainsKey(key))
            {
                return idMap[key];
            }
            Guid newId = Guid.NewGuid();
            idMap[key] = newId;
            return newId;
        }

        private void insertRecord(IDbCommand command, string schema, string table,
            IEnumerable<KeyValuePair<string, object>> record)
        {
            const string sql = "insert into {0}.{1} ({2}) values ({3})";
            var columns = new List<string>();
            var values = new List<string>();

            foreach (var kvp in record)
            {
                string paramName = "@" + kvp.Key;
                object paramValue = kvp.Value;

                if (paramValue != null)
                {
                    command.Parameters.Add(new SqlParameter(paramName, paramValue));

                    columns.Add(kvp.Key);
                    values.Add(paramName);
                }
            }

            string colString = "[" + String.Join("],[", columns.ToArray()) + "]";
            string valString = String.Join(",", values.ToArray());

            command.CommandText = String.Format(sql, schema, table, colString, valString);
            command.ExecuteNonQuery();
        }

        //private void updateRecord(IDbCommand command, string schema, string table, ExpandoObject record)
        //{
        //    var sql = "update {0}.{1} set {2} where [Id] = @Id";
        //    var sets = new List<string>();

        //    foreach (KeyValuePair<string, object> kvp in record)
        //    {
        //        var paramName = "@" + kvp.Key;
        //        var paramValue = kvp.Value;

        //        if (paramValue != null)
        //        {
        //            command.Parameters.Add(new SqlParameter(paramName, paramValue));

        //            sets.Add("[" + kvp.Key + "] = " + paramName);
        //        }
        //    }

        //    var setString = String.Join(", ", sets.ToArray());

        //    command.CommandText = String.Format(sql, schema, table, setString);
        //    command.ExecuteNonQuery();
        //}
    }
}