
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK982A10FEC2306C27]') AND parent_object_id = OBJECT_ID('LeagueDivisions'))
alter table LeagueDivisions  drop constraint FK982A10FEC2306C27


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK982A10FECF7F4E2D]') AND parent_object_id = OBJECT_ID('LeagueDivisions'))
alter table LeagueDivisions  drop constraint FK982A10FECF7F4E2D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK16AF0593ED3E7B11]') AND parent_object_id = OBJECT_ID('Players'))
alter table Players  drop constraint FK16AF0593ED3E7B11


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK16AF0593580565D8]') AND parent_object_id = OBJECT_ID('Players'))
alter table Players  drop constraint FK16AF0593580565D8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA1D5AD58E8EB03AE]') AND parent_object_id = OBJECT_ID('Teams'))
alter table Teams  drop constraint FKA1D5AD58E8EB03AE


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA1D5AD58CF7F4E2D]') AND parent_object_id = OBJECT_ID('Teams'))
alter table Teams  drop constraint FKA1D5AD58CF7F4E2D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA1D5AD58C2306C27]') AND parent_object_id = OBJECT_ID('Teams'))
alter table Teams  drop constraint FKA1D5AD58C2306C27


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKA1D5AD58917CD841]') AND parent_object_id = OBJECT_ID('Teams'))
alter table Teams  drop constraint FKA1D5AD58917CD841


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKCAA865B787F24DCF]') AND parent_object_id = OBJECT_ID('TeamManagers'))
alter table TeamManagers  drop constraint FKCAA865B787F24DCF


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKCAA865B7702EDB08]') AND parent_object_id = OBJECT_ID('TeamManagers'))
alter table TeamManagers  drop constraint FKCAA865B7702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK609D063CD7119F81]') AND parent_object_id = OBJECT_ID('TeamCoaches'))
alter table TeamCoaches  drop constraint FK609D063CD7119F81


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK609D063C702EDB08]') AND parent_object_id = OBJECT_ID('TeamCoaches'))
alter table TeamCoaches  drop constraint FK609D063C702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB13B0820702EDB08]') AND parent_object_id = OBJECT_ID('TeamPlayers'))
alter table TeamPlayers  drop constraint FKB13B0820702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKB13B08205B38EDAF]') AND parent_object_id = OBJECT_ID('TeamPlayers'))
alter table TeamPlayers  drop constraint FKB13B08205B38EDAF


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK4AFE2B793487CD0]') AND parent_object_id = OBJECT_ID('webpages_OAuthMembership'))
alter table webpages_OAuthMembership  drop constraint FK4AFE2B793487CD0


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK8D9A0C88580565D8]') AND parent_object_id = OBJECT_ID('Managers'))
alter table Managers  drop constraint FK8D9A0C88580565D8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK8D9A0C8885F8119E]') AND parent_object_id = OBJECT_ID('Managers'))
alter table Managers  drop constraint FK8D9A0C8885F8119E


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKE5B7031D580565D8]') AND parent_object_id = OBJECT_ID('Coaches'))
alter table Coaches  drop constraint FKE5B7031D580565D8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKE5B7031D85F8119E]') AND parent_object_id = OBJECT_ID('Coaches'))
alter table Coaches  drop constraint FKE5B7031D85F8119E


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC22285F0C2306C27]') AND parent_object_id = OBJECT_ID('Umpires'))
alter table Umpires  drop constraint FKC22285F0C2306C27


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC22285F0580565D8]') AND parent_object_id = OBJECT_ID('Umpires'))
alter table Umpires  drop constraint FKC22285F0580565D8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC22285F085F8119E]') AND parent_object_id = OBJECT_ID('Umpires'))
alter table Umpires  drop constraint FKC22285F085F8119E


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857B72E8615]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857B72E8615


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857B725F2B3]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857B725F2B3


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857917CD841]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857917CD841


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D85785F8119E]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D85785F8119E


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D85762B7EE64]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D85762B7EE64


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D8572CCDD557]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D8572CCDD557


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D85710CA3831]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D85710CA3831


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857DB8B5149]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857DB8B5149


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857D4A92C58]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857D4A92C58


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857CF7F4E2D]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857CF7F4E2D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKBC93D857C20A29C8]') AND parent_object_id = OBJECT_ID('Games'))
alter table Games  drop constraint FKBC93D857C20A29C8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5665D8E4580565D8]') AND parent_object_id = OBJECT_ID('Guardians'))
alter table Guardians  drop constraint FK5665D8E4580565D8


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKE60EC16BC2306C27]') AND parent_object_id = OBJECT_ID('Fees'))
alter table Fees  drop constraint FKE60EC16BC2306C27


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5A5E66FD224667FA]') AND parent_object_id = OBJECT_ID('FeePayments'))
alter table FeePayments  drop constraint FK5A5E66FD224667FA


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5A5E66FD702EDB08]') AND parent_object_id = OBJECT_ID('FeePayments'))
alter table FeePayments  drop constraint FK5A5E66FD702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5A5E66FD1662A180]') AND parent_object_id = OBJECT_ID('FeePayments'))
alter table FeePayments  drop constraint FK5A5E66FD1662A180


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD9575DE1702EDB08]') AND parent_object_id = OBJECT_ID('AvailableDates'))
alter table AvailableDates  drop constraint FKD9575DE1702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKD9575DE1917CD841]') AND parent_object_id = OBJECT_ID('AvailableDates'))
alter table AvailableDates  drop constraint FKD9575DE1917CD841


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK955B3338C2306C27]') AND parent_object_id = OBJECT_ID('Brackets'))
alter table Brackets  drop constraint FK955B3338C2306C27


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK955B3338CF7F4E2D]') AND parent_object_id = OBJECT_ID('Brackets'))
alter table Brackets  drop constraint FK955B3338CF7F4E2D


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK6E9596D310CA3831]') AND parent_object_id = OBJECT_ID('BracketTeams'))
alter table BracketTeams  drop constraint FK6E9596D310CA3831


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK6E9596D3702EDB08]') AND parent_object_id = OBJECT_ID('BracketTeams'))
alter table BracketTeams  drop constraint FK6E9596D3702EDB08


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC2C14E6719E08AEA]') AND parent_object_id = OBJECT_ID('MessageLogs'))
alter table MessageLogs  drop constraint FKC2C14E6719E08AEA


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK3AF505CF6DF68560]') AND parent_object_id = OBJECT_ID('PlayerGameStats'))
alter table PlayerGameStats  drop constraint FK3AF505CF6DF68560


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK3AF505CF5C4A3B9F]') AND parent_object_id = OBJECT_ID('PlayerGameStats'))
alter table PlayerGameStats  drop constraint FK3AF505CF5C4A3B9F


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FKC710518A10CA3831]') AND parent_object_id = OBJECT_ID('BracketGenerator'))
alter table BracketGenerator  drop constraint FKC710518A10CA3831


    if exists (select * from dbo.sysobjects where id = object_id(N'Divisions') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Divisions

    if exists (select * from dbo.sysobjects where id = object_id(N'LeagueDivisions') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table LeagueDivisions

    if exists (select * from dbo.sysobjects where id = object_id(N'Leagues') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Leagues

    if exists (select * from dbo.sysobjects where id = object_id(N'Players') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Players

    if exists (select * from dbo.sysobjects where id = object_id(N'TeamClasses') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TeamClasses

    if exists (select * from dbo.sysobjects where id = object_id(N'Teams') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Teams

    if exists (select * from dbo.sysobjects where id = object_id(N'TeamManagers') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TeamManagers

    if exists (select * from dbo.sysobjects where id = object_id(N'TeamCoaches') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TeamCoaches

    if exists (select * from dbo.sysobjects where id = object_id(N'TeamPlayers') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table TeamPlayers

    if exists (select * from dbo.sysobjects where id = object_id(N'webpages_Membership') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table webpages_Membership

    if exists (select * from dbo.sysobjects where id = object_id(N'webpages_OAuthMembership') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table webpages_OAuthMembership

    if exists (select * from dbo.sysobjects where id = object_id(N'Managers') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Managers

    if exists (select * from dbo.sysobjects where id = object_id(N'Coaches') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Coaches

    if exists (select * from dbo.sysobjects where id = object_id(N'Umpires') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Umpires

    if exists (select * from dbo.sysobjects where id = object_id(N'Games') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Games

    if exists (select * from dbo.sysobjects where id = object_id(N'Locations') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Locations

    if exists (select * from dbo.sysobjects where id = object_id(N'Guardians') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Guardians

    if exists (select * from dbo.sysobjects where id = object_id(N'PayPalPayments') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PayPalPayments

    if exists (select * from dbo.sysobjects where id = object_id(N'Fees') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Fees

    if exists (select * from dbo.sysobjects where id = object_id(N'FeePayments') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table FeePayments

    if exists (select * from dbo.sysobjects where id = object_id(N'AvailableDates') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table AvailableDates

    if exists (select * from dbo.sysobjects where id = object_id(N'[BracketResult]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table [BracketResult]

    if exists (select * from dbo.sysobjects where id = object_id(N'Brackets') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table Brackets

    if exists (select * from dbo.sysobjects where id = object_id(N'BracketTeams') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table BracketTeams

    if exists (select * from dbo.sysobjects where id = object_id(N'MessageLogs') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table MessageLogs

    if exists (select * from dbo.sysobjects where id = object_id(N'PlayerGameStats') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table PlayerGameStats

    if exists (select * from dbo.sysobjects where id = object_id(N'BracketGenerator') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table BracketGenerator

    create table Divisions (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null unique,
       MaxAge INT not null,
       CreatedOn DATETIME not null,
       primary key (Id)
    )

    create table LeagueDivisions (
        Division_id INT not null,
       League_id INT not null
    )

    create table Leagues (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null,
       Url NVARCHAR(255) null,
       StartDate DATETIME not null,
       EndDate DATETIME null,
       RegistrationStartDate DATETIME not null,
       RegistrationEndDate DATETIME not null,
       RosterLockedOn DATETIME null,
       IsActive BIT null,
       WaiverRequired BIT null,
       CreatedOn DATETIME not null,
       Type NVARCHAR(255) null,
       HtmlDescription NVARCHAR(255) null,
       MinimumDatesAvailable INT null,
       primary key (Id)
    )

    create table Players (
        Id INT IDENTITY NOT NULL,
       FirstName NVARCHAR(255) not null,
       LastName NVARCHAR(255) not null,
       DateOfBirth DATETIME null,
       Email NVARCHAR(255) null,
       Gender NVARCHAR(255) not null,
       JerseyNumber NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       InvitationSentOn DATETIME null,
       InviteToken NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       IsLookingForTeam BIT null,
       Guardian_id INT null,
       User_id INT null,
       primary key (Id)
    )

    create table TeamClasses (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null,
       Handicap INT null,
       CreatedOn DATETIME not null,
       primary key (Id)
    )

    create table Teams (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null,
       Url NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       IsLookingForPlayers BIT null,
       HtmlDescription NVARCHAR(255) null,
       Class_id INT null,
       Division_id INT null,
       League_id INT null,
       Location_id INT null,
       primary key (Id)
    )

    create table TeamManagers (
        Team_id INT not null,
       Manager_id INT not null
    )

    create table TeamCoaches (
        Team_id INT not null,
       Coach_id INT not null
    )

    create table TeamPlayers (
        Id INT IDENTITY NOT NULL,
       IsPhotoVerified BIT not null,
       Photo NVARCHAR(255) null,
       Status NVARCHAR(255) null,
       SignWaiverId NVARCHAR(255) null,
       WaiverStatus NVARCHAR(255) null,
       JerseyNumber NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       Team_id INT not null,
       Player_id INT not null,
       primary key (Id)
    )

    create table webpages_Membership (
        UserId INT IDENTITY NOT NULL,
       CreateDate DATETIME null,
       Password NVARCHAR(255) null,
       Email NVARCHAR(255) null unique,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       Role INT not null,
       ConfirmationToken NVARCHAR(255) null,
       IsConfirmed BIT null,
       IsEmailConfirmed BIT null,
       LastPasswordFailureDate DATETIME null,
       PasswordFailuresSinceLastSuccess INT null,
       PasswordChangedDate DATETIME null,
       PasswordSalt NVARCHAR(255) null,
       PasswordVerificationToken NVARCHAR(255) null,
       PasswordVerificationTokenExpirationDate DATETIME null,
       primary key (UserId)
    )

    create table webpages_OAuthMembership (
        UserId INT not null,
       Provider NVARCHAR(255) not null,
       ProviderUserId NVARCHAR(255) null,
       primary key (UserId, Provider)
    )

    create table Managers (
        Id INT IDENTITY NOT NULL,
       Email NVARCHAR(255) null,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       Photo NVARCHAR(255) null,
       PhotoType NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       InviteToken NVARCHAR(255) null,
       InvitationSentOn DATETIME null,
       User_id INT null,
       CreatedBy_id INT null,
       primary key (Id)
    )

    create table Coaches (
        Id INT IDENTITY NOT NULL,
       Email NVARCHAR(255) null,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       Photo NVARCHAR(255) null,
       PhotoType NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       InviteToken NVARCHAR(255) null,
       InvitationSentOn DATETIME null,
       User_id INT null,
       CreatedBy_id INT null,
       primary key (Id)
    )

    create table Umpires (
        Id INT IDENTITY NOT NULL,
       Email NVARCHAR(255) null,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       Photo NVARCHAR(255) null,
       PhotoType NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       InviteToken NVARCHAR(255) null,
       InvitationSentOn DATETIME null,
       League_id INT null,
       User_id INT null,
       CreatedBy_id INT null,
       primary key (Id)
    )

    create table Games (
        Id INT IDENTITY NOT NULL,
       GameDate DATETIME not null,
       Innings INT null,
       HomeTeamScore INT null,
       AwayTeamScore INT null,
       CreatedOn DATETIME not null,
       BracketBracket INT null,
       BracketPosition INT null,
       Status NVARCHAR(255) null,
       Field NVARCHAR(255) null,
       HomeTeam_id INT null,
       AwayTeam_id INT null,
       Location_id INT null,
       CreatedBy_id INT null,
       PlateUmpire_id INT null,
       FieldUmpire_id INT null,
       Bracket_id INT null,
       TeamToConfirmGame_id INT null,
       TeamToRequestGame_id INT null,
       Division_id INT null,
       Umpire_id INT null,
       primary key (Id)
    )

    create table Locations (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) null,
       Address NVARCHAR(255) null,
       City NVARCHAR(255) null,
       State NVARCHAR(255) null,
       Zip NVARCHAR(255) null,
       Url NVARCHAR(255) null,
       Notes NVARCHAR(255) null,
       Latitude DOUBLE PRECISION null,
       Longitude DOUBLE PRECISION null,
       CreatedOn DATETIME not null,
       GroundsKeeperPhone NVARCHAR(255) null,
       primary key (Id)
    )

    create table Guardians (
        Id INT IDENTITY NOT NULL,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       Email NVARCHAR(255) null,
       PhoneNumber NVARCHAR(255) null,
       Address NVARCHAR(255) null,
       City NVARCHAR(255) null,
       State NVARCHAR(255) null,
       Zip NVARCHAR(255) null,
       InvitationSentOn DATETIME null,
       InviteToken NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       User_id INT null,
       primary key (Id)
    )

    create table PayPalPayments (
        Id INT IDENTITY NOT NULL,
       Address NVARCHAR(255) null,
       Amount DECIMAL(19,5) null,
       City NVARCHAR(255) null,
       Country NVARCHAR(255) null,
       CreatedOn DATETIME null,
       Currency NVARCHAR(255) null,
       Email NVARCHAR(255) null,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       State NVARCHAR(255) null,
       Status NVARCHAR(255) null,
       TransactionId NVARCHAR(255) null,
       Zip NVARCHAR(255) null,
       Raw NVARCHAR(255) null,
       InvoiceId NVARCHAR(255) null,
       Option1 NVARCHAR(255) null,
       Option2 NVARCHAR(255) null,
       primary key (Id)
    )

    create table Fees (
        Id INT IDENTITY NOT NULL,
       Amount DECIMAL(19,5) not null,
       Notes NVARCHAR(255) null,
       Description NVARCHAR(255) null,
       Name NVARCHAR(255) not null,
       IsRequired BIT not null,
       CreatedOn DATETIME not null,
       League_id INT null,
       primary key (Id)
    )

    create table FeePayments (
        Id INT IDENTITY NOT NULL,
       Amount DECIMAL(19,5) not null,
       Note NVARCHAR(255) null,
       Type NVARCHAR(255) null,
       Method NVARCHAR(255) null,
       Status NVARCHAR(255) null,
       TransactionId NVARCHAR(255) null,
       CreatedOn DATETIME not null,
       CompletedOn DATETIME null,
       Fee_id INT null,
       Team_id INT null,
       Payment_id INT null,
       primary key (Id)
    )

    create table AvailableDates (
        Id INT IDENTITY NOT NULL,
       Date DATETIME not null,
       GameScheduled BIT not null,
       IsHome BIT not null,
       IsAway BIT not null,
       DistanceFromLocation INT null,
       Team_id INT not null,
       Location_id INT not null,
       primary key (Id)
    )

    create table [BracketResult] (
        Id INT IDENTITY NOT NULL,
       Team1 INT null,
       Team2 INT null,
       Bracket INT null,
       Position INT null,
       GameNumber INT null,
       primary key (Id)
    )

    create table Brackets (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) null,
       CreatedOn DATETIME null,
       League_id INT null,
       Division_id INT null,
       primary key (Id)
    )

    create table BracketTeams (
        Id INT IDENTITY NOT NULL,
       Standing INT null,
       Bracket_id INT null,
       Team_id INT null,
       primary key (Id)
    )

    create table MessageLogs (
        Id INT IDENTITY NOT NULL,
       Body NVARCHAR(255) null,
       RecipientCount INT null,
       Recipients NVARCHAR(255) null,
       SentOn DATETIME null,
       Subject NVARCHAR(255) null,
       SentBy_id INT null,
       primary key (Id)
    )

    create table PlayerGameStats (
        Id INT IDENTITY NOT NULL,
       InningsPitched DOUBLE PRECISION null,
       InningsOuts DOUBLE PRECISION null,
       PitchesThrown DOUBLE PRECISION null,
       TeamPlayer_id INT not null,
       Game_id INT not null,
       primary key (Id)
    )

    create table BracketGenerator (
        Id INT IDENTITY NOT NULL,
       Sequence INT null,
       Team1 INT null,
       Team2 INT null,
       Position INT null,
       Bracket INT null,
       GameNumber INT null,
       Bracket_id INT null,
       primary key (Id)
    )

    alter table LeagueDivisions 
        add constraint FK982A10FEC2306C27 
        foreign key (League_id) 
        references Leagues

    alter table LeagueDivisions 
        add constraint FK982A10FECF7F4E2D 
        foreign key (Division_id) 
        references Divisions

    alter table Players 
        add constraint FK16AF0593ED3E7B11 
        foreign key (Guardian_id) 
        references Guardians

    alter table Players 
        add constraint FK16AF0593580565D8 
        foreign key (User_id) 
        references webpages_Membership

    alter table Teams 
        add constraint FKA1D5AD58E8EB03AE 
        foreign key (Class_id) 
        references TeamClasses

    alter table Teams 
        add constraint FKA1D5AD58CF7F4E2D 
        foreign key (Division_id) 
        references Divisions

    alter table Teams 
        add constraint FKA1D5AD58C2306C27 
        foreign key (League_id) 
        references Leagues

    alter table Teams 
        add constraint FKA1D5AD58917CD841 
        foreign key (Location_id) 
        references Locations

    alter table TeamManagers 
        add constraint FKCAA865B787F24DCF 
        foreign key (Manager_id) 
        references Managers

    alter table TeamManagers 
        add constraint FKCAA865B7702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table TeamCoaches 
        add constraint FK609D063CD7119F81 
        foreign key (Coach_id) 
        references Coaches

    alter table TeamCoaches 
        add constraint FK609D063C702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table TeamPlayers 
        add constraint FKB13B0820702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table TeamPlayers 
        add constraint FKB13B08205B38EDAF 
        foreign key (Player_id) 
        references Players

    alter table webpages_OAuthMembership 
        add constraint FK4AFE2B793487CD0 
        foreign key (UserId) 
        references webpages_Membership

    alter table Managers 
        add constraint FK8D9A0C88580565D8 
        foreign key (User_id) 
        references webpages_Membership

    alter table Managers 
        add constraint FK8D9A0C8885F8119E 
        foreign key (CreatedBy_id) 
        references webpages_Membership

    alter table Coaches 
        add constraint FKE5B7031D580565D8 
        foreign key (User_id) 
        references webpages_Membership

    alter table Coaches 
        add constraint FKE5B7031D85F8119E 
        foreign key (CreatedBy_id) 
        references webpages_Membership

    alter table Umpires 
        add constraint FKC22285F0C2306C27 
        foreign key (League_id) 
        references Leagues

    alter table Umpires 
        add constraint FKC22285F0580565D8 
        foreign key (User_id) 
        references webpages_Membership

    alter table Umpires 
        add constraint FKC22285F085F8119E 
        foreign key (CreatedBy_id) 
        references webpages_Membership

    alter table Games 
        add constraint FKBC93D857B72E8615 
        foreign key (HomeTeam_id) 
        references Teams

    alter table Games 
        add constraint FKBC93D857B725F2B3 
        foreign key (AwayTeam_id) 
        references Teams

    alter table Games 
        add constraint FKBC93D857917CD841 
        foreign key (Location_id) 
        references Locations

    alter table Games 
        add constraint FKBC93D85785F8119E 
        foreign key (CreatedBy_id) 
        references webpages_Membership

    alter table Games 
        add constraint FKBC93D85762B7EE64 
        foreign key (PlateUmpire_id) 
        references Umpires

    alter table Games 
        add constraint FKBC93D8572CCDD557 
        foreign key (FieldUmpire_id) 
        references Umpires

    alter table Games 
        add constraint FKBC93D85710CA3831 
        foreign key (Bracket_id) 
        references Brackets

    alter table Games 
        add constraint FKBC93D857DB8B5149 
        foreign key (TeamToConfirmGame_id) 
        references Teams

    alter table Games 
        add constraint FKBC93D857D4A92C58 
        foreign key (TeamToRequestGame_id) 
        references Teams

    alter table Games 
        add constraint FKBC93D857CF7F4E2D 
        foreign key (Division_id) 
        references Divisions

    alter table Games 
        add constraint FKBC93D857C20A29C8 
        foreign key (Umpire_id) 
        references Umpires

    alter table Guardians 
        add constraint FK5665D8E4580565D8 
        foreign key (User_id) 
        references webpages_Membership

    alter table Fees 
        add constraint FKE60EC16BC2306C27 
        foreign key (League_id) 
        references Leagues

    alter table FeePayments 
        add constraint FK5A5E66FD224667FA 
        foreign key (Fee_id) 
        references Fees

    alter table FeePayments 
        add constraint FK5A5E66FD702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table FeePayments 
        add constraint FK5A5E66FD1662A180 
        foreign key (Payment_id) 
        references PayPalPayments

    alter table AvailableDates 
        add constraint FKD9575DE1702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table AvailableDates 
        add constraint FKD9575DE1917CD841 
        foreign key (Location_id) 
        references Locations

    alter table Brackets 
        add constraint FK955B3338C2306C27 
        foreign key (League_id) 
        references Leagues

    alter table Brackets 
        add constraint FK955B3338CF7F4E2D 
        foreign key (Division_id) 
        references Divisions

    alter table BracketTeams 
        add constraint FK6E9596D310CA3831 
        foreign key (Bracket_id) 
        references Brackets

    alter table BracketTeams 
        add constraint FK6E9596D3702EDB08 
        foreign key (Team_id) 
        references Teams

    alter table MessageLogs 
        add constraint FKC2C14E6719E08AEA 
        foreign key (SentBy_id) 
        references webpages_Membership

    alter table PlayerGameStats 
        add constraint FK3AF505CF6DF68560 
        foreign key (TeamPlayer_id) 
        references TeamPlayers

    alter table PlayerGameStats 
        add constraint FK3AF505CF5C4A3B9F 
        foreign key (Game_id) 
        references Games

    alter table BracketGenerator 
        add constraint FKC710518A10CA3831 
        foreign key (Bracket_id) 
        references Brackets
