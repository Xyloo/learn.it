CREATE DATABASE learnitdb;
GO
USE learnitdb;
GO

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
GO

IF SCHEMA_ID(N'learnitdb') IS NULL EXEC(N'CREATE SCHEMA [learnitdb];');
GO

CREATE TABLE [learnitdb].[Achievements] (
    [AchievementId] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NULL,
    [ImagePath] nvarchar(100) NULL,
    [Description] nvarchar(200) NULL,
    [Predicate] nvarchar(450) NULL,
    CONSTRAINT [PK_achievements_achievement_id] PRIMARY KEY ([AchievementId])
);
GO

CREATE TABLE [learnitdb].[Permissions] (
    [PermissionId] int NOT NULL IDENTITY,
    [Name] nvarchar(45) NOT NULL,
    CONSTRAINT [PK_permissions_permission_id] PRIMARY KEY ([PermissionId])
);
GO

CREATE TABLE [learnitdb].[Users] (
    [UserId] int NOT NULL IDENTITY,
    [Username] nvarchar(450) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [CreateTime] datetime2(0) NOT NULL DEFAULT ((getdate())),
    [LastLogin] datetime2(0) NULL,
    [Avatar] nvarchar(max) NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_users_user_id] PRIMARY KEY ([UserId]),
    CONSTRAINT [users$fk_users_permissions1] FOREIGN KEY ([PermissionId]) REFERENCES [learnitdb].[Permissions] ([PermissionId])
);
GO

CREATE TABLE [learnitdb].[Groups] (
    [GroupId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [CreatorId] int NULL,
    CONSTRAINT [PK_groups_group_id] PRIMARY KEY ([GroupId]),
    CONSTRAINT [groups$fk_groups_users1] FOREIGN KEY ([CreatorId]) REFERENCES [learnitdb].[Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[Logins] (
    [LoginId] int NOT NULL IDENTITY,
    [Timestamp] datetime2(0) NOT NULL DEFAULT ((getdate())),
    [IsSuccessful] bit NOT NULL,
    [UserAgent] nvarchar(255) NOT NULL,
    [IpAddress] nvarchar(45) NOT NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_logins_login_id] PRIMARY KEY ([LoginId]),
    CONSTRAINT [logins$fk_logins_users] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId])
);
GO

CREATE TABLE [learnitdb].[UserAchievements] (
    [AchievementId] int NOT NULL,
    [UserId] int NOT NULL,
    [Timestamp] datetime2(0) NOT NULL DEFAULT ((getdate())),
    CONSTRAINT [PK_user_achievements_user_id] PRIMARY KEY ([UserId], [AchievementId]),
    CONSTRAINT [user_achievements$fk_user_achievements_achievements1] FOREIGN KEY ([AchievementId]) REFERENCES [learnitdb].[Achievements] ([AchievementId]) ON DELETE CASCADE,
    CONSTRAINT [user_achievements$fk_user_achievements_users1] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[UserPreferences] (
    [UserId] int NOT NULL,
    [HighContrastMode] bit NOT NULL,
    [AutoTts] bit NOT NULL,
    CONSTRAINT [PK_user_preferences_user_id] PRIMARY KEY ([UserId]),
    CONSTRAINT [user_preferences$fk_user_preferences_users1] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[UserStats] (
    [UserId] int NOT NULL,
    [TotalSetsMastered] int NOT NULL,
    [TotalLoginDays] int NOT NULL,
    [TotalFlashcardsMastered] int NOT NULL,
    [ConsecutiveLoginDays] int NOT NULL,
    [TotalSetsAdded] int NOT NULL,
    [TotalFlashcardsAdded] int NOT NULL,
    CONSTRAINT [PK_user_stats_user_id] PRIMARY KEY ([UserId]),
    CONSTRAINT [user_stats$fk_user_stats_users1] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[GroupJoinRequest] (
    [GroupId] int NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2(0) NOT NULL,
    [ExpiresAt] datetime2(0) NOT NULL,
    [CreatorId] int NOT NULL,
    CONSTRAINT [PK_group_join_requests_user_id] PRIMARY KEY ([UserId], [GroupId]),
    CONSTRAINT [group_join_requests$fk_group_join_requests_groups1] FOREIGN KEY ([GroupId]) REFERENCES [learnitdb].[Groups] ([GroupId]) ON DELETE CASCADE,
    CONSTRAINT [group_join_requests$fk_group_join_requests_users1] FOREIGN KEY ([CreatorId]) REFERENCES [learnitdb].[Users] ([UserId])
);
GO

CREATE TABLE [learnitdb].[GroupUser] (
    [GroupId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_GroupUser] PRIMARY KEY ([GroupId], [UserId]),
    CONSTRAINT [FK_GroupUser_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [learnitdb].[Groups] ([GroupId]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupUser_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId])
);
GO

CREATE TABLE [learnitdb].[StudySets] (
    [StudySetId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Visibility] int NOT NULL,
    [CreatorId] int NOT NULL,
    [GroupId] int NULL,
    CONSTRAINT [PK_study_sets_study_set_id] PRIMARY KEY ([StudySetId]),
    CONSTRAINT [study_sets$fk_study_sets_groups1] FOREIGN KEY ([GroupId]) REFERENCES [learnitdb].[Groups] ([GroupId]),
    CONSTRAINT [study_sets$fk_study_sets_users1] FOREIGN KEY ([CreatorId]) REFERENCES [learnitdb].[Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[Flashcards] (
    [FlashcardId] int NOT NULL IDENTITY,
    [Term] nvarchar(max) NULL,
    [Definition] nvarchar(max) NULL,
    [IsTermText] bit NOT NULL,
    [StudySetId] int NULL,
    CONSTRAINT [PK_flashcards_flashcard_id] PRIMARY KEY ([FlashcardId]),
    CONSTRAINT [flashcards$fk_flashcards_study_sets1] FOREIGN KEY ([StudySetId]) REFERENCES [learnitdb].[StudySets] ([StudySetId]) ON DELETE CASCADE
);
GO

CREATE TABLE [learnitdb].[Answers] (
    [AnswerId] int NOT NULL IDENTITY,
    [IsCorrect] bit NOT NULL,
    [AnswerTime] int NOT NULL,
    [AnswerTimestamp] datetime2 NOT NULL,
    [FlashcardId] int NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_answers_answer_id] PRIMARY KEY ([AnswerId]),
    CONSTRAINT [answers$fk_answers_flashcards1] FOREIGN KEY ([FlashcardId]) REFERENCES [learnitdb].[Flashcards] ([FlashcardId]) ON DELETE CASCADE,
    CONSTRAINT [answers$fk_answers_users1] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId])
);
GO

CREATE TABLE [learnitdb].[FlashcardUserProgress] (
    [UserId] int NOT NULL,
    [FlashcardId] int NOT NULL,
    [ConsecutiveCorrectAnswers] int NOT NULL,
    [IsMastered] bit NOT NULL,
    [MasteredTimestamp] datetime2(0) NULL,
    [NeedsMoreRepetitions] bit NOT NULL,
    CONSTRAINT [PK_flashcard_user_progress_user_id] PRIMARY KEY ([UserId], [FlashcardId]),
    CONSTRAINT [flashcard_user_progress$fk_flashcard_user_progress_flashcards1] FOREIGN KEY ([FlashcardId]) REFERENCES [learnitdb].[Flashcards] ([FlashcardId]) ON DELETE CASCADE,
    CONSTRAINT [flashcard_user_progress$fk_flashcard_user_progress_users1] FOREIGN KEY ([UserId]) REFERENCES [learnitdb].[Users] ([UserId])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'Name') AND [object_id] = OBJECT_ID(N'[learnitdb].[Permissions]'))
    SET IDENTITY_INSERT [learnitdb].[Permissions] ON;
INSERT INTO [learnitdb].[Permissions] ([PermissionId], [Name])
VALUES (1, N'Admin'),
(2, N'User');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'Name') AND [object_id] = OBJECT_ID(N'[learnitdb].[Permissions]'))
    SET IDENTITY_INSERT [learnitdb].[Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Username', N'Email', N'Password', N'CreateTime', N'LastLogin', N'Avatar', N'PermissionId') AND [object_id] = OBJECT_ID(N'[learnitdb].[Users]'))
    SET IDENTITY_INSERT [learnitdb].[Users] ON;
INSERT INTO [learnitdb].[Users] ([UserId], [Username], [Email], [Password], [CreateTime], [LastLogin], [Avatar], [PermissionId])
VALUES (-1, N'testAdmin', N'test@admin.com', N'AQAAAAIAAYagAAAAEHgdRpl0GvK6IgRIWMN1SJfmJ2yirgRsimXNDeQx0LuSyPTqEQRenosehwnQfQSpGA==', '2024-01-02T20:18:44Z', NULL, NULL, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'Username', N'Email', N'Password', N'CreateTime', N'LastLogin', N'Avatar', N'PermissionId') AND [object_id] = OBJECT_ID(N'[learnitdb].[Users]'))
    SET IDENTITY_INSERT [learnitdb].[Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'TotalSetsMastered', N'TotalLoginDays', N'TotalFlashcardsMastered', N'ConsecutiveLoginDays', N'TotalSetsAdded', N'TotalFlashcardsAdded') AND [object_id] = OBJECT_ID(N'[learnitdb].[UserStats]'))
    SET IDENTITY_INSERT [learnitdb].[UserStats] ON;
INSERT INTO [learnitdb].[UserStats] ([UserId], [TotalSetsMastered], [TotalLoginDays], [TotalFlashcardsMastered], [ConsecutiveLoginDays], [TotalSetsAdded], [TotalFlashcardsAdded])
VALUES (-1, 0, 0, 0, 0, 0, 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'TotalSetsMastered', N'TotalLoginDays', N'TotalFlashcardsMastered', N'ConsecutiveLoginDays', N'TotalSetsAdded', N'TotalFlashcardsAdded') AND [object_id] = OBJECT_ID(N'[learnitdb].[UserStats]'))
    SET IDENTITY_INSERT [learnitdb].[UserStats] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'HighContrastMode', N'AutoTts') AND [object_id] = OBJECT_ID(N'[learnitdb].[UserPreferences]'))
    SET IDENTITY_INSERT [learnitdb].[UserPreferences] ON;
INSERT INTO [learnitdb].[UserPreferences] ([UserId], [HighContrastMode], [AutoTts])
VALUES (-1, CAST(0 AS bit), CAST(0 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'HighContrastMode', N'AutoTts') AND [object_id] = OBJECT_ID(N'[learnitdb].[UserPreferences]'))
    SET IDENTITY_INSERT [learnitdb].[UserPreferences] OFF;
GO

CREATE UNIQUE INDEX [predicate_UNIQUE] ON [learnitdb].[Achievements] ([Predicate]) WHERE [Predicate] IS NOT NULL;
GO

CREATE INDEX [fk_answers_flashcards1_idx] ON [learnitdb].[Answers] ([FlashcardId]);
GO

CREATE INDEX [fk_answers_users1_idx] ON [learnitdb].[Answers] ([UserId]);
GO

CREATE INDEX [fk_flashcards_study_sets1_idx] ON [learnitdb].[Flashcards] ([StudySetId]);
GO

CREATE INDEX [fk_flashcard_user_progress_flashcards1_idx] ON [learnitdb].[FlashcardUserProgress] ([FlashcardId]);
GO

CREATE INDEX [IX_GroupJoinRequest_CreatorId] ON [learnitdb].[GroupJoinRequest] ([CreatorId]);
GO

CREATE INDEX [IX_GroupJoinRequest_GroupId] ON [learnitdb].[GroupJoinRequest] ([GroupId]);
GO

CREATE INDEX [fk_groups_users1_idx] ON [learnitdb].[Groups] ([CreatorId]);
GO

CREATE INDEX [IX_GroupUser_UserId] ON [learnitdb].[GroupUser] ([UserId]);
GO

CREATE INDEX [fk_logins_users_idx] ON [learnitdb].[Logins] ([UserId]);
GO

CREATE INDEX [fk_study_sets_groups1_idx] ON [learnitdb].[StudySets] ([GroupId]);
GO

CREATE INDEX [fk_study_sets_users1_idx] ON [learnitdb].[StudySets] ([CreatorId]);
GO

CREATE INDEX [fk_user_achievements_achievements1] ON [learnitdb].[UserAchievements] ([AchievementId]);
GO

CREATE INDEX [fk_user_achievements_users1_idx] ON [learnitdb].[UserAchievements] ([UserId]);
GO

CREATE INDEX [fk_user_preferences_users1_idx] ON [learnitdb].[UserPreferences] ([UserId]);
GO

CREATE INDEX [fk_users_permissions1_idx] ON [learnitdb].[Users] ([PermissionId]);
GO

CREATE UNIQUE INDEX [users$email_UNIQUE] ON [learnitdb].[Users] ([Email]);
GO

CREATE UNIQUE INDEX [users$username_UNIQUE] ON [learnitdb].[Users] ([Username]);
GO

CREATE INDEX [fk_user_stats_users1_idx] ON [learnitdb].[UserStats] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240102193744_Initial', N'7.0.13');
GO

COMMIT;
GO

