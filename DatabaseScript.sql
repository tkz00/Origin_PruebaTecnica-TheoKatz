USE [Origin_Test]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 21/4/2023 18:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Balance_Operation]    Script Date: 21/4/2023 18:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Balance_Operation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[created] [datetime] NOT NULL,
	[code] [varchar](16) NOT NULL,
	[card_id] [int] NOT NULL,
 CONSTRAINT [PK_Balance_Operation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Credit_Card]    Script Date: 21/4/2023 18:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Credit_Card](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL,
	[card_number] [varchar](16) NOT NULL,
	[pin] [varchar](4) NOT NULL,
	[expiration_date] [date] NOT NULL,
	[balance] [decimal](18, 2) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[failedLoginAttempts] [int] NOT NULL,
	[LastFailedLoginTime] [datetime] NULL,
 CONSTRAINT [PK_Credit_Card] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Withdraw_Operation]    Script Date: 21/4/2023 18:43:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Withdraw_Operation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[card_id] [int] NOT NULL,
	[created] [datetime] NOT NULL,
	[code] [varchar](16) NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Withdraw_Operation] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Balance_Operation]  WITH CHECK ADD  CONSTRAINT [FK_Balance_Operation_Credit_Card] FOREIGN KEY([card_id])
REFERENCES [dbo].[Credit_Card] ([id])
GO
ALTER TABLE [dbo].[Balance_Operation] CHECK CONSTRAINT [FK_Balance_Operation_Credit_Card]
GO
ALTER TABLE [dbo].[Withdraw_Operation]  WITH CHECK ADD  CONSTRAINT [FK_Withdraw_Operation_Credit_Card] FOREIGN KEY([card_id])
REFERENCES [dbo].[Credit_Card] ([id])
GO
ALTER TABLE [dbo].[Withdraw_Operation] CHECK CONSTRAINT [FK_Withdraw_Operation_Credit_Card]
GO
