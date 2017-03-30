/*Include the drop statements to reset the database */

--drop table Alarms, AlarmTypes, Jars, JarTypes, Orders, Customers
--drop view JarOrderAmount, ProductionInfo

--go

CREATE TABLE [dbo].[AlarmTypes] (
    [AlarmId]     INT        IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NULL,
    [Priority]    INT        NOT NULL,
    CONSTRAINT [XPKAlarmType] PRIMARY KEY CLUSTERED ([AlarmId] ASC)
)

go

CREATE TABLE [dbo].[Alarms] (
    [OrderId] INT      NULL,
    [Time]    DATETIME DEFAULT (getdate()) NOT NULL,
    [AlarmId] INT      NOT NULL,
    CONSTRAINT [XPKAlarm] PRIMARY KEY CLUSTERED ([AlarmId] ASC, [Time] ASC),
    CONSTRAINT [R_10] FOREIGN KEY ([AlarmId]) REFERENCES [dbo].[AlarmTypes] ([AlarmId])
)

go

CREATE TABLE [dbo].[Customers] (
    [CustomerId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50) NOT NULL,
    [Address]    VARCHAR (50) NULL,
    CONSTRAINT [XPKCustomer] PRIMARY KEY CLUSTERED ([CustomerId] ASC)
)

go

CREATE TABLE [dbo].[JarTypes] (
    [TypeId]    INT          IDENTITY (1, 1) NOT NULL,
    [Type]      VARCHAR (50) NOT NULL,
    [Weight]    FLOAT (53)   DEFAULT ((0)) NOT NULL,
    [CostPrMl]  FLOAT (53)   DEFAULT ((0)) NOT NULL,
    [MaxWeight] FLOAT (53)   DEFAULT ((0)) NOT NULL,
    CONSTRAINT [XPKJarType] PRIMARY KEY CLUSTERED ([TypeId] ASC)
)

go

CREATE TABLE [dbo].[Orders] (
    [OrderId]        INT      IDENTITY (1, 1) NOT NULL,
    [Status]         INT      NOT NULL,
    [OrderedTime]    DATETIME DEFAULT (getdate()) NOT NULL,
    [CustomerId]     INT      NOT NULL,
    [DisgardedJars]  INT      NULL,
    [ProductionTime] INT      NULL,
    CONSTRAINT [XPKOrder] PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT [R_3] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([CustomerId])
)

go

CREATE TABLE [dbo].[Jars] (
    [JarId]           INT          IDENTITY (1, 1) NOT NULL,
    [Amount]          VARCHAR (18) NOT NULL,
    [TypeId]          INT          NOT NULL,
    [OrderId]         INT          NOT NULL,
    [Status]          INT          DEFAULT ((0)) NOT NULL,
    [AmountDelivered] VARCHAR (10) NULL,
    CONSTRAINT [XPKJar] PRIMARY KEY CLUSTERED ([JarId] ASC),
    CONSTRAINT [R_1] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[JarTypes] ([TypeId]),
    CONSTRAINT [R_2] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([OrderId])
)

go

CREATE VIEW JarOrderAmount (OrderId, JarType, Amount)
AS SELECT o.OrderId, jt.Type, COUNT(jt.TypeId)
FROM Orders o
INNER JOIN Jars j
ON o.OrderId = j.OrderId 
INNER JOIN JarTypes jt
ON j.TypeId = jt.TypeId 
GROUP BY jt.Type, o.OrderId

go

CREATE VIEW ProductionInfo (NumOfOrders, InProgress, Finished, OrderId)
AS SELECT COUNT(Jars.Status),
COUNT(CASE WHEN Jars.Status = 1 THEN 1 END),
COUNT(CASE WHEN Jars.Status = 2 THEN 1 END),
OrderId
FROM Jars 
GROUP BY OrderId

go

INSERT INTO AlarmTypes(Description,Priority) VALUES
( 'Loss of Air Pressure', 1),
( 'Water level High', 1),
( 'Transparent Jar Empty', 2),
( 'Tall Jar Empty', 2),
( 'Black Jar Empty', 2),
( 'Red Jar Empty', 2),
( 'Water level Low', 2),
( 'Cap empty', 2),
( 'Tray full', 2)

go

INSERT INTO JarTypes(Type,Weight,CostPrMl,MaxWeight) VALUES
('Høyt glass',24,6,40),
('Gjennomsiktig glass',16,4,12),
('Svart glass',15,4,12),
('Rødt glass',15,4,12)


;