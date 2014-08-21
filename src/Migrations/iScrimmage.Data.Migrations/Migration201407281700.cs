using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
            Delete.Table("Role").InSchema("blt");


            Delete.Schema("blt");
        }

        public override void Up()
        {
            //Debugger.Launch();

            idMap = new Dictionary<string, Guid>();

            Create.Schema("blt");


            Create.iScrimmageTable("blt", "Role")
                .WithColumn("Name").AsString(125).NotNullable().Unique("IX_Role_Name")
                .WithColumn("Description").AsString(1000).Nullable()
                .WithAuditColumns();

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
                .WithColumn("RoleId").AsGuid().NotNullable().ForeignKey("FK_Member_Role", "blt", "Role", "Id")
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


            Create.iScrimmageTable("blt", "Roster")
                .WithColumn("RosterName").AsString(250).NotNullable()
                .WithColumn("TeamId").AsGuid().Nullable().ForeignKey("FK_Roster_Team", "blt", "Team", "Id")
                .WithColumn("EventDivisionClassId").AsGuid().NotNullable().ForeignKey("FK_Roster_EventDivisionClass", "blt", "EventDivisionClass", "Id")
                .WithColumn("RosterIsLocked").AsBoolean().WithDefaultValue(0)
                .WithColumn("RosterLockedOn").AsDate().Nullable()
                .WithColumn("RosterLockedBy").AsGuid().Nullable()
                .WithColumn("OldId").AsInt16().Nullable()
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
            Execute.WithConnection(migrateEventData);


            //handle geography items
            //Execute.Sql("UPDATE blt.[Location] SET [Point] = geography::STPointFromText('POINT(' + CAST([Longitude] AS VARCHAR(20)) + ' ' +  CAST([Latitude] AS VARCHAR(20)) + ')', 4326)");
        }

        private void migrateLeagueData(IDbConnection conn, IDbTransaction tran)
        {
            migrateLeaguesData(tran);
            migrateDivisionsData(tran);
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

        private void migrateEventData(IDbConnection conn, IDbTransaction tran)
        {
            migrateEventDivisionClassData(tran);
            migrateTeamData(tran);
            migrateTeamRosterPlayerData(tran);
            migrateTeamRosterCoachData(tran);
            migrateTeamRosterManagerData(tran);
        }

        #region default data

        private void addDefaultData()
        {
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
                Description = "Umpire",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Guardian"),
                Name = "Guardian",
                Description = "Guardian",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });

            Insert.IntoTable("Role").InSchema("blt").Row(new
            {
                Id = getMappedId("Role_Admin"),
                Name = "Admin",
                Description = "Admin",
                CreatedBy = systemId,
                CreatedOn = DateTime.Today
            });
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
                CreatedOn = DateTime.Today,
                RoleId = getMappedId("Role_Admin")
            };
            Insert.IntoTable("Member").InSchema("blt").Row(systemMember);

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

        #endregion

        #region League Data
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
                        record.Type = getEventType(reader.GetValueOrNull<string>(columns.Type).ToString());
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
        #endregion

        #region Member Data


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
                        member.OldId = oldId;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;
                        member.RoleId = getMappedId("Role_Coach");

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
                        member.RoleId = getMappedId("Role_Guardian");

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
                        member.OldId = oldId;
                        member.RoleId = getMappedId("Role_Manager");

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
                Photo = 13,
                Status = 14,
                PhotoVerified = 15,
                WaiverStatus = 16,
                SignWaiverId = 17
            };

            using (IDbCommand cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText =
                    @"SELECT Players.Id, Players.Guardian_id, Players.Email, Players.FirstName, Players.LastName, Players.DateOfBirth, Players.Gender, Players.PhoneNumber, Players.JerseyNumber, Players.InvitationSentOn, Players.InviteToken, Players.IsLookingForTeam, Players.User_id,(SELECT TOP 1 Photo from TeamPlayers where Player_id = Players.Id) as Photo, (SELECT TOP 1 Status from TeamPlayers where Player_id = Players.Id) as Status  FROM Players ";

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
                        member.OldId = oldId;
                        member.RoleId = getMappedId("Role_Player");
                        member.Photo = reader.GetValueOrNull<string>(columns.Photo);
                        var status = 0;
                        if (reader.GetValueOrNull<string>(columns.Status) != null && reader.GetValueOrNull<string>(columns.Status).ToString().ToLower() == "active") status = 1;
                        member.Status = status;

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
                        member.OldId = oldId;

                        phone.Id = Guid.NewGuid();
                        phone.MemberId = newId;
                        phone.Type = "Mobile";
                        phone.PhoneNumber = reader.GetValueOrNull<string>(columns.PhoneNumber);
                        phone.CreatedBy = systemId;
                        phone.CreatedOn = DateTime.Today;
                        member.RoleId = getMappedId("Role_Umpire");

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
        #endregion

        #region Event and Roster Data

        private void migrateTeamData(IDbTransaction tran)
        {
            var insertTeamRecords = new List<ExpandoObject>();
            var insertRosterRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Name = 1,
                Url = 2,
                CreatedOn = 3,
                ClassId = 4,
                DivisionId = 5,
                LeagueId = 6,
                IsRosterLocked = 7,
                RosterLockedOn = 8,
                RosterLockedBy = 9,
                LocationId = 10,
                IsLookingForPlayers = 11,
                HtmlDescription = 12,
                DivisionName = 13,
                LeagueName = 14,
                ClassName = 15
            };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT Teams.Id, Teams.Name, Teams.Url, Teams.CreatedOn, Teams.Class_id, Teams.Division_id, Teams.League_id, Teams.IsRosterLocked, Teams.RosterLockedOn, Teams.RosterLockedBy_id, Teams.Location_id, Teams.IsLookingForPlayers, Teams.HtmlDescription, Divisions.Name, Leagues.Name, TeamClasses.Name FROM Teams INNER JOIN Leagues ON Teams.League_id = Leagues.Id INNER JOIN Divisions ON Teams.Division_id = Divisions.Id INNER JOIN TeamClasses ON Teams.Class_id = TeamClasses.Id";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oldId = "TeamName_" + reader.GetValueOrNull<string>(columns.Name).ToString().Trim().ToLower();
                        Guid newId = getMappedId(oldId);

                        dynamic teamrecord = new ExpandoObject();

                        teamrecord.Id = newId;
                        teamrecord.Name = reader.GetValueOrNull<string>(columns.Name).ToString().Trim();
                        if (reader.GetValueOrNull<int>(columns.LocationId) != null)
                            teamrecord.LocationId = getMappedId("Location_" + reader.GetValueOrNull<int>(columns.LocationId));
                        teamrecord.Url = reader.GetValueOrNull<string>(columns.Url);
                        teamrecord.LookingForPlayers = reader.GetValueOrNull<int>(columns.IsLookingForPlayers);
                        teamrecord.CreatedBy = systemId;
                        teamrecord.CreatedOn = DateTime.Today;


                        dynamic rosterrecord = new ExpandoObject();
                        var newrosterid = getMappedId("Roster_" + reader.GetInt32(columns.Id) + "|" + reader.GetInt32(columns.ClassId) + "|" +
                                        reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.LeagueId));
                        rosterrecord.Id = newrosterid;
                        rosterrecord.RosterName = reader.GetValueOrNull<string>(columns.Name).ToString().Trim() + " (" +
                                                  reader.GetValueOrNull<string>(columns.LeagueName) + ", " +
                                                  reader.GetValueOrNull<string>(columns.DivisionName) + ", " +
                                                  reader.GetValueOrNull<string>(columns.ClassName) + ")";
                        rosterrecord.TeamId = newId;
                        rosterrecord.EventDivisionClassId =
                            getMappedId("ClDiLe" + reader.GetInt32(columns.ClassId) + "|" +
                                        reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.LeagueId));
                        rosterrecord.CreatedBy = systemId;
                        rosterrecord.CreatedOn = DateTime.Today;
                        rosterrecord.OldId = reader.GetValueOrNull<Int16>(columns.Id);

                        insertTeamRecords.Add(teamrecord);
                        insertRosterRecords.Add(rosterrecord);
                    }
                }
            }

            foreach (var record in insertTeamRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    try
                    {
                        insertRecord(cmd, "blt", "Team", record);
                    }
                    catch (Exception e)
                    {
                        //this prevents the addition of additional team records because the old database used a single table where the new uses 2.
                        //so a single team now can have multiple rosters
                        //simple filtering of the List<Dynamic> is problematic so this was the workaround.
                        EventLog.WriteEntry("BLT", e.Message);
                    }

                }
            }

            foreach (var record in insertRosterRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "Roster", record);

                }
            }
        }

        private void migrateEventDivisionClassData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                ClassId = 0,
                DivisionId = 1,
                EventId = 2
            };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"select Class_id, Division_id, League_id from dbo.Teams group by Class_id, Division_id, League_id ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "ClDiLe" + reader.GetInt32(columns.ClassId) + "|" + reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.EventId);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();
                        record.Id = newId;
                        record.EventId = getMappedId("Event" + reader.GetInt32(columns.EventId));
                        record.DivisionId = getMappedId("Division_" + reader.GetInt32(columns.DivisionId));
                        record.ClassId = getMappedId("Class_" + reader.GetInt32(columns.ClassId));
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

                    insertRecord(cmd, "blt", "EventDivisionClass", record);
                }
            }
        }

        private void migrateTeamRosterPlayerData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                Id = 0,
                Status = 1,
                CreatedOn = 2,
                TeamId = 3,
                PlayerId = 4,
                JerseyNumber = 5,
                TeamName = 6,
                ClassId = 7,
                DivisionId = 8,
                LeagueId = 9
            };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT TeamPlayers.Id, TeamPlayers.Status, TeamPlayers.CreatedOn, TeamPlayers.Team_id, TeamPlayers.Player_id, TeamPlayers.JerseyNumber, Teams.Name, Teams.Class_id, Teams.Division_id, Teams.League_id FROM TeamPlayers INNER JOIN Teams ON TeamPlayers.Team_id = Teams.Id ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "RosterMemberPlayer_" + reader.GetInt32(columns.Id);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        var newrosterid = getMappedId("Roster_" + reader.GetInt32(columns.TeamId) + "|" + reader.GetInt32(columns.ClassId) + "|" +
                                        reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.LeagueId));

                        record.Id = newId;
                        record.RosterId = newrosterid;
                        record.MemberId = getMappedId("Player_" + reader.GetInt32(columns.PlayerId));
                        record.RoleId = getMappedId("Role_Player");
                        record.JerseyNumber = reader.GetValueOrNull<string>(columns.JerseyNumber);
                        record.CreatedBy = systemId;
                        record.CreatedOn = reader.GetValueOrNull<DateTime>(columns.CreatedOn);

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "RosterMember", record);
                }
            }
        }

        private void migrateTeamRosterCoachData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                TeamId = 0,
                CoachId = 1,
                ClassId = 2,
                DivisionId = 3,
                LeagueId = 4
            };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT  Team_id,Coach_id, Teams.Class_id, Teams.Division_id, Teams.League_id FROM [BLT].[dbo].[TeamCoaches] INNER JOIN Teams ON [TeamCoaches].Team_id = Teams.Id";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "RosterMemberCoach_" + reader.GetInt32(columns.TeamId) + "|" + reader.GetInt32(columns.CoachId);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        var newrosterid = getMappedId("Roster_" + reader.GetInt32(columns.TeamId) + "|" + reader.GetInt32(columns.ClassId) + "|" +
                                        reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.LeagueId));

                        record.Id = newId;
                        record.RosterId = newrosterid;
                        record.MemberId = getMappedId("Coach_" + reader.GetInt32(columns.CoachId));
                        record.RoleId = getMappedId("Role_Coach");
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Now;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "RosterMember", record);
                }
            }
        }

        private void migrateTeamRosterManagerData(IDbTransaction tran)
        {
            var insertRecords = new List<ExpandoObject>();

            var columns = new
            {
                TeamId = 0,
                ManagerId = 1,
                ClassId = 2,
                DivisionId = 3,
                LeagueId = 4
            };

            using (var cmd = tran.Connection.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = @"SELECT  Team_id,Manager_id, Teams.Class_id, Teams.Division_id, Teams.League_id FROM [BLT].[dbo].[TeamManagers] INNER JOIN Teams ON [TeamManagers].Team_id = Teams.Id";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var oldId = "RosterMemberManager_" + reader.GetInt32(columns.TeamId) + "|" + reader.GetInt32(columns.ManagerId);
                        var newId = getMappedId(oldId);

                        dynamic record = new ExpandoObject();

                        var newrosterid = getMappedId("Roster_" + reader.GetInt32(columns.TeamId) + "|" + reader.GetInt32(columns.ClassId) + "|" +
                                        reader.GetInt32(columns.DivisionId) + "|" + reader.GetInt32(columns.LeagueId));

                        record.Id = newId;
                        record.RosterId = newrosterid;
                        record.MemberId = getMappedId("Manager_" + reader.GetInt32(columns.ManagerId));
                        record.RoleId = getMappedId("Role_Manager");
                        record.CreatedBy = systemId;
                        record.CreatedOn = DateTime.Now;

                        insertRecords.Add(record);
                    }
                }
            }

            foreach (var record in insertRecords)
            {
                using (var cmd = tran.Connection.CreateCommand())
                {
                    cmd.Transaction = tran;

                    insertRecord(cmd, "blt", "RosterMember", record);
                }
            }
        }
        #endregion

        #region Helper Methods
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
        #endregion

    }
}