
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[countries]') AND type in (N'U'))
EXECUTE('
CREATE TABLE [dbo].[countries](
	[id] [bigint] NULL,
	[Country] [nvarchar](100) NULL,
	[A2] [nvarchar](10) NULL,
	[A3] [nvarchar](10) NULL,
	[UN] [nvarchar](10) NULL,
	[dial] [nvarchar](20) NULL
) ON [PRIMARY]
')


GO


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[flags]') AND type in (N'U'))
EXECUTE('
CREATE TABLE [dbo].[flags](
	[flag] [nvarchar](10) NULL,
	[country] [nvarchar](10) NULL,
	[country_id] [bigint] NULL,
	[width] [int] NULL,
	[height] [int] NULL,
	[b64] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
')


GO


IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[geoip].[geoip_locations_temp]') AND type in (N'U'))
EXECUTE('
CREATE TABLE [geoip].[geoip_locations_temp](
	[geoname_id] [bigint] NOT NULL,
	[locale_code] [varchar](2) NOT NULL,
	[continent_code] [varchar](2) NULL,
	[continent_name] [varchar](15) NULL,
	[country_iso_code] [varchar](2) NULL,
	[country_name] [varchar](50) NULL,
 CONSTRAINT [PK_geoip_locations_temp] PRIMARY KEY CLUSTERED 
(
	[geoname_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
')


GO




IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[geoip].[FK_geoip_locations_temp]') AND parent_object_id = OBJECT_ID(N'[geoip].[geoip_blocks_temp]'))
ALTER TABLE [geoip].[geoip_blocks_temp] DROP CONSTRAINT [FK_geoip_locations_temp]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_geoip_blocks_temp_network]') AND type = 'D')
BEGIN
ALTER TABLE [geoip].[geoip_blocks_temp] DROP CONSTRAINT [DF_geoip_blocks_temp_network]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_lower_geoip_blocks_temp_lower_boundary]') AND type = 'D')
BEGIN
ALTER TABLE [geoip].[geoip_blocks_temp] DROP CONSTRAINT [DF_lower_geoip_blocks_temp_lower_boundary]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_lower_geoip_blocks_temp_upper_boundary]') AND type = 'D')
BEGIN
ALTER TABLE [geoip].[geoip_blocks_temp] DROP CONSTRAINT [DF_lower_geoip_blocks_temp_upper_boundary]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[geoip].[geoip_blocks_temp]') AND type in (N'U'))
DROP TABLE [geoip].[geoip_blocks_temp]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [geoip].[geoip_blocks_temp](
	[network] [varchar](32) NOT NULL,
	[geoname_id] [bigint] NULL,
	[registered_country_geoname_id] [bigint] NULL,
	[represented_country_geoname_id] [bigint] NULL,
	[is_anonymous_proxy] [int] NOT NULL,
	[is_satellite_provider] [int] NOT NULL,
	[lower_boundary] [varchar](39) NOT NULL,
	[upper_boundary] [varchar](39) NOT NULL,
	[lower_boundary_int] [bigint] NULL,
	[upper_boundary_int] [bigint] NULL,
 CONSTRAINT [PK_geoip_blocks_temp] PRIMARY KEY CLUSTERED 
(
	[network] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [geoip].[geoip_blocks_temp]  WITH CHECK ADD  CONSTRAINT [FK_geoip_locations_temp] FOREIGN KEY([geoname_id])
REFERENCES [geoip].[geoip_locations_temp] ([geoname_id])
GO

ALTER TABLE [geoip].[geoip_blocks_temp] CHECK CONSTRAINT [FK_geoip_locations_temp]
GO

ALTER TABLE [geoip].[geoip_blocks_temp] ADD  CONSTRAINT [DF_geoip_blocks_temp_network]  DEFAULT ('') FOR [network]
GO

ALTER TABLE [geoip].[geoip_blocks_temp] ADD  CONSTRAINT [DF_lower_geoip_blocks_temp_lower_boundary]  DEFAULT ('') FOR [lower_boundary]
GO

ALTER TABLE [geoip].[geoip_blocks_temp] ADD  CONSTRAINT [DF_lower_geoip_blocks_temp_upper_boundary]  DEFAULT ('') FOR [upper_boundary]
GO


