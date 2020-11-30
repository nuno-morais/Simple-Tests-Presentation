CREATE DATABASE [billstracker-db]
GO

USE [billstracker-db];
GO

CREATE TABLE Bills (
	Id INT NOT NULL IDENTITY,
	Description TEXT NOT NULL,
	Price INT NOT NULL,
	PRIMARY KEY (Id)
);
GO
