USE [PPS2]
GO
/****** Object:  Table [dbo].[CorporateCustomer]    Script Date: 9/17/2020 12:28:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorporateCustomer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Customer_Id] [int] NOT NULL,
	[CustomerType] [int] NOT NULL,
	[DateOfBirth] [datetime2](7) NULL,
	[DriversLicenceNo] [nvarchar](15) NULL,
	[CompanyNo] [nvarchar](15) NULL,
	[DateIncorporated] [datetime2](7) NULL,
	[NatureOfBusiness] [nvarchar](50) NULL,
	[GSTNo] [nvarchar](15) NULL,
	[PaidUpCapital] [decimal](18, 4) NULL,
	[EstimatedMonthlyPurchase] [decimal](18, 4) NULL,
	[CreditLimtRequired] [decimal](18, 4) NULL,
	[PrincipalPlaceOfBusiness] [int] NULL,
	[LiquoredFullNameLicensee] [nvarchar](1000) NOT NULL,
	[LiquoredCompanyNo] [nvarchar](15) NOT NULL,
	[LiquoredLicenceNo] [nvarchar](15) NOT NULL,
	[LiquoredLicenceExpiryDate] [datetime2](7) NOT NULL,
	[LiquoredTobaccoLicenceNo] [nvarchar](15) NOT NULL,
	[LiquoredTobaccoLicenceExpiryDate] [datetime2](7) NOT NULL,
	[LiquoredStreet] [nvarchar](max) NOT NULL,
	[LiquoredPhone] [nvarchar](max) NOT NULL,
	[LiquoredSuburb] [nvarchar](max) NOT NULL,
	[LiquoredPostCode] [nvarchar](max) NOT NULL,
	[LiquoredFacsimilieNumber] [nvarchar](15) NOT NULL,
	[LiquoredLandlordName] [nvarchar](max) NOT NULL,
	[LiquoredDateLeasedFrom] [datetime2](7) NOT NULL,
	[LiquoredDateLeasedTo] [datetime2](7) NOT NULL,
	[LiquoredPreviousLicenceDetails] [nvarchar](max) NOT NULL,
	[AccountTerms] [int] NOT NULL,
	[PurchaseOrderRequired] [bit] NOT NULL,
	[AccountsToBeEmailed] [bit] NOT NULL,
	[AccountsEmailAddress] [nvarchar](max) NOT NULL,
	[AccountsContact] [nvarchar](max) NOT NULL,
	[PhoneNo] [nvarchar](max) NOT NULL,
	[Bank] [nvarchar](max) NOT NULL,
	[Branch] [nvarchar](max) NOT NULL,
	[AccountNo] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CorporateCustomer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CorporateCustomerCompanyOwners]    Script Date: 9/17/2020 12:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorporateCustomerCompanyOwners](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CorporateCustomer_Id] [int] NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[DateOfBirth] [datetime2](7) NOT NULL,
	[PrivateAddress] [nvarchar](max) NOT NULL,
	[Postcode] [nvarchar](max) NOT NULL,
	[DriversLicenceNo] [nvarchar](max) NOT NULL,
	[PhoneNo] [nvarchar](max) NOT NULL,
	[MobileNo] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CorporateCustomerCompanyOwners] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CorporateCustomerTradeReferences]    Script Date: 9/17/2020 12:28:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorporateCustomerTradeReferences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CorporateCustomer_Id] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[PhoneFaxEmail] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CorporateCustomerTradeReferences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CorporateCustomer]  WITH CHECK ADD  CONSTRAINT [FK_CorporateCustomer_Customer_Id] FOREIGN KEY([Customer_Id])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[CorporateCustomer] CHECK CONSTRAINT [FK_CorporateCustomer_Customer_Id]
GO
ALTER TABLE [dbo].[CorporateCustomerCompanyOwners]  WITH CHECK ADD  CONSTRAINT [FK_CorporateCustomer_CompanyOwners_Id] FOREIGN KEY([CorporateCustomer_Id])
REFERENCES [dbo].[CorporateCustomer] ([Id])
GO
ALTER TABLE [dbo].[CorporateCustomerCompanyOwners] CHECK CONSTRAINT [FK_CorporateCustomer_CompanyOwners_Id]
GO
ALTER TABLE [dbo].[CorporateCustomerTradeReferences]  WITH CHECK ADD  CONSTRAINT [FK_CorporateCustomer_CorporateCustomer_Id] FOREIGN KEY([CorporateCustomer_Id])
REFERENCES [dbo].[CorporateCustomer] ([Id])
GO
ALTER TABLE [dbo].[CorporateCustomerTradeReferences] CHECK CONSTRAINT [FK_CorporateCustomer_CorporateCustomer_Id]
GO
