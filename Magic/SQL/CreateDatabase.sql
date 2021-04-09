USE [magic]
GO
/****** Object:  Table [dbo].[Card]    Script Date: 2021-04-10 12:57:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Card](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](max) NOT NULL,
	[Type] [int] NOT NULL,
	[SubType] [varchar](max) NULL,
	[BlueMana] [int] NULL,
	[GreenMana] [int] NULL,
	[WhiteMana] [int] NULL,
	[BlackMana] [int] NULL,
	[RedMana] [int] NULL,
	[NeutralMana] [int] NULL,
	[Rarity] [int] NOT NULL,
	[Mechanic] [varchar](max) NULL,
	[CodeName] [varchar](max) NULL,
	[Power] [int] NOT NULL,
	[Defense] [int] NOT NULL,
	[EditionId] [int] NOT NULL,
	[Commentary] [varchar](max) NULL,
	[UrlImage] [varchar](max) NULL,
	[IsTreated] [bit] NOT NULL,
 CONSTRAINT [PK_Card] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Edition]    Script Date: 2021-04-10 12:57:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Edition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](max) NOT NULL,
	[Url_Logo] [varchar](max) NULL,
	[Description] [varchar](max) NULL,
	[Subtitle] [varchar](max) NULL,
 CONSTRAINT [PK_Edition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Game]    Script Date: 2021-04-10 12:57:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Game](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Settings] [varchar](max) NOT NULL,
	[Guid] [varchar](max) NOT NULL,
	[Edition] [int] NOT NULL,
	[Date] [date] NOT NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rarity]    Script Date: 2021-04-10 12:57:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rarity](
	[Id] [int] NOT NULL,
	[Label] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Rarity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Type]    Script Date: 2021-04-10 12:57:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Type](
	[Id] [int] NOT NULL,
	[Label] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Card] ADD  CONSTRAINT [DF_Card_IsTreated]  DEFAULT ((0)) FOR [IsTreated]
GO
ALTER TABLE [dbo].[Card]  WITH CHECK ADD  CONSTRAINT [FK_Card_Edition] FOREIGN KEY([EditionId])
REFERENCES [dbo].[Edition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Card] CHECK CONSTRAINT [FK_Card_Edition]
GO
ALTER TABLE [dbo].[Card]  WITH CHECK ADD  CONSTRAINT [FK_Card_Rarity] FOREIGN KEY([Rarity])
REFERENCES [dbo].[Rarity] ([Id])
GO
ALTER TABLE [dbo].[Card] CHECK CONSTRAINT [FK_Card_Rarity]
GO
ALTER TABLE [dbo].[Card]  WITH CHECK ADD  CONSTRAINT [FK_Card_Type] FOREIGN KEY([Type])
REFERENCES [dbo].[Type] ([Id])
GO
ALTER TABLE [dbo].[Card] CHECK CONSTRAINT [FK_Card_Type]
GO
ALTER TABLE [dbo].[Game]  WITH CHECK ADD  CONSTRAINT [FK_Game_Edition] FOREIGN KEY([Edition])
REFERENCES [dbo].[Edition] ([Id])
GO
ALTER TABLE [dbo].[Game] CHECK CONSTRAINT [FK_Game_Edition]
GO
