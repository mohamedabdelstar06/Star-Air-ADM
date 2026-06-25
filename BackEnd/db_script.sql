IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Aircraft] (
    [Id] int NOT NULL IDENTITY,
    [RegistrationNumber] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [Model] nvarchar(max) NOT NULL,
    [YearOfManufacture] int NULL,
    [LastMaintenanceDate] datetime2 NULL,
    [NextMaintenanceDate] datetime2 NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Aircraft] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(150) NOT NULL,
    [LicenseNumber] nvarchar(50) NULL,
    [MedicalClass] nvarchar(20) NULL,
    [MedicalExpiry] datetime2 NULL,
    [Rank] nvarchar(50) NULL,
    [TotalFlightHours] int NOT NULL,
    [ProfileImageUrl] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [InvitationToken] nvarchar(max) NULL,
    [InvitationTokenExpiry] datetime2 NULL,
    [RefreshToken] nvarchar(max) NULL,
    [RefreshTokenExpiry] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [Checklists] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Checklists] PRIMARY KEY ([Id])
);

CREATE TABLE [NotamCaches] (
    [Id] int NOT NULL IDENTITY,
    [AirportCode] nvarchar(max) NOT NULL,
    [NotamText] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NULL,
    [EffectiveFrom] datetime2 NULL,
    [EffectiveTo] datetime2 NULL,
    [FetchedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_NotamCaches] PRIMARY KEY ([Id])
);

CREATE TABLE [WeatherCaches] (
    [Id] int NOT NULL IDENTITY,
    [StationCode] nvarchar(max) NOT NULL,
    [MetarRaw] nvarchar(max) NULL,
    [TafRaw] nvarchar(max) NULL,
    [ParsedData] nvarchar(max) NULL,
    [FetchedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    CONSTRAINT [PK_WeatherCaches] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] nvarchar(450) NULL,
    [Action] nvarchar(max) NOT NULL,
    [EntityType] nvarchar(max) NULL,
    [EntityId] nvarchar(max) NULL,
    [OldValues] nvarchar(max) NULL,
    [NewValues] nvarchar(max) NULL,
    [IpAddress] nvarchar(max) NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);

CREATE TABLE [DecideSessions] (
    [Id] int NOT NULL IDENTITY,
    [PilotId] nvarchar(450) NOT NULL,
    [Scenario] nvarchar(max) NULL,
    [Status] nvarchar(30) NOT NULL,
    [FinalRiskScore] int NULL,
    [StartedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    [IsSynced] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_DecideSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DecideSessions_AspNetUsers_PilotId] FOREIGN KEY ([PilotId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ImSafeAssessments] (
    [Id] int NOT NULL IDENTITY,
    [PilotId] nvarchar(450) NOT NULL,
    [IllnessLevel] int NOT NULL,
    [IllnessNotes] nvarchar(max) NULL,
    [MedicationLevel] int NOT NULL,
    [MedicationNotes] nvarchar(max) NULL,
    [StressLevel] int NOT NULL,
    [StressNotes] nvarchar(max) NULL,
    [AlcoholLevel] int NOT NULL,
    [HoursSinceLastDrink] float NULL,
    [FatigueLevel] int NOT NULL,
    [HoursSlept] float NULL,
    [EmotionLevel] int NOT NULL,
    [EmotionNotes] nvarchar(max) NULL,
    [DataSource] nvarchar(20) NOT NULL,
    [OverallRiskScore] int NOT NULL,
    [Result] nvarchar(20) NOT NULL,
    [AssessedAt] datetime2 NOT NULL,
    [IsSynced] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_ImSafeAssessments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImSafeAssessments_AspNetUsers_PilotId] FOREIGN KEY ([PilotId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [KneeboardNotes] (
    [Id] int NOT NULL IDENTITY,
    [PilotId] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Tags] nvarchar(max) NULL,
    [IsSynced] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_KneeboardNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_KneeboardNotes_AspNetUsers_PilotId] FOREIGN KEY ([PilotId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PaveAssessments] (
    [Id] int NOT NULL IDENTITY,
    [PilotId] nvarchar(450) NOT NULL,
    [AircraftId] int NULL,
    [PilotReadiness] nvarchar(max) NULL,
    [PilotRiskLevel] int NOT NULL,
    [AircraftCondition] nvarchar(max) NULL,
    [AircraftRiskLevel] int NOT NULL,
    [WeatherSummary] nvarchar(max) NULL,
    [MetarData] nvarchar(max) NULL,
    [TafData] nvarchar(max) NULL,
    [EnvironmentRiskLevel] int NOT NULL,
    [ExternalPressures] nvarchar(max) NULL,
    [ExternalRiskLevel] int NOT NULL,
    [OverallRiskScore] int NOT NULL,
    [Result] nvarchar(20) NOT NULL,
    [AssessedAt] datetime2 NOT NULL,
    [IsSynced] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_PaveAssessments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaveAssessments_Aircraft_AircraftId] FOREIGN KEY ([AircraftId]) REFERENCES [Aircraft] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_PaveAssessments_AspNetUsers_PilotId] FOREIGN KEY ([PilotId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [SmartWatchReadings] (
    [Id] int NOT NULL IDENTITY,
    [PilotId] nvarchar(450) NOT NULL,
    [HeartRate] int NULL,
    [HeartRateVariability] int NULL,
    [SleepHours] float NULL,
    [SleepQuality] int NULL,
    [StressIndex] int NULL,
    [SpO2] int NULL,
    [SkinTemperature] float NULL,
    [Steps] int NULL,
    [DeviceName] nvarchar(max) NULL,
    [RawData] nvarchar(max) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [IsSynced] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_SmartWatchReadings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SmartWatchReadings_AspNetUsers_PilotId] FOREIGN KEY ([PilotId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ChecklistItems] (
    [Id] int NOT NULL IDENTITY,
    [ChecklistId] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [SortOrder] int NOT NULL,
    [IsCritical] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_ChecklistItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChecklistItems_Checklists_ChecklistId] FOREIGN KEY ([ChecklistId]) REFERENCES [Checklists] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DecideSteps] (
    [Id] int NOT NULL IDENTITY,
    [SessionId] int NOT NULL,
    [StepType] nvarchar(30) NOT NULL,
    [StepOrder] int NOT NULL,
    [Input] nvarchar(max) NULL,
    [Notes] nvarchar(max) NULL,
    [SuggestedActions] nvarchar(max) NULL,
    [SelectedAction] nvarchar(max) NULL,
    [CompletedAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_DecideSteps] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DecideSteps_DecideSessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [DecideSessions] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);

CREATE INDEX [IX_ChecklistItems_ChecklistId] ON [ChecklistItems] ([ChecklistId]);

CREATE INDEX [IX_DecideSessions_PilotId] ON [DecideSessions] ([PilotId]);

CREATE INDEX [IX_DecideSteps_SessionId] ON [DecideSteps] ([SessionId]);

CREATE INDEX [IX_ImSafeAssessments_PilotId] ON [ImSafeAssessments] ([PilotId]);

CREATE INDEX [IX_KneeboardNotes_PilotId] ON [KneeboardNotes] ([PilotId]);

CREATE INDEX [IX_PaveAssessments_AircraftId] ON [PaveAssessments] ([AircraftId]);

CREATE INDEX [IX_PaveAssessments_PilotId] ON [PaveAssessments] ([PilotId]);

CREATE INDEX [IX_SmartWatchReadings_PilotId] ON [SmartWatchReadings] ([PilotId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260318101516_InitialCreate', N'9.0.14');

COMMIT;
GO

