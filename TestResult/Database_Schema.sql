DROP TABLE TestError;
DROP TABLE TestCaseRun;
DROP TABLE TestSuiteRun;
DROP TABLE ​AppRun;

CREATE TABLE ​AppRun (
	AppRunId INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY,
	AppArea VARCHAR(MAX) NOT NULL,
	BuildDate DATETIME NOT NULL,
	ServerName VARCHAR(MAX) NOT NULL,
	StartTime DATETIME NOT NULL,
	EndTime DATETIME NOT NULL,
	Alias VARCHAR(MAX) NOT NULL,
	DbType VARCHAR(MAX) NOT NULL,
	Version VARCHAR(MAX) NOT NULL
);

CREATE TABLE TestSuiteRun (
	SuiteRunId INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY,
	AppRunId INTEGER FOREIGN KEY REFERENCES ​AppRun(AppRunId),
	Name VARCHAR(MAX) NOT NULL,
	Duration FLOAT NOT NULL
);

CREATE TABLE TestCaseRun (
	CaseRunId INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY,
	SuiteRunId INTEGER FOREIGN KEY REFERENCES TestSuiteRun(SuiteRunId),
	Name VARCHAR(MAX) NOT NULL, 
	Duration FLOAT NOT NULL
);

CREATE TABLE TestError (
	ErrorId INTEGER NOT NULL IDENTITY(1,1) PRIMARY KEY,
	CaseRunId INTEGER FOREIGN KEY REFERENCES TestCaseRun(CaseRunId),
	Message VARCHAR(MAX) NOT NULL
);