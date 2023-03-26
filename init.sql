/*  Generated by ChatGPT. I am actually impressed.. */
CREATE TABLE public.AspNetUsers (
    Id varchar(255) NOT NULL,
    UserName varchar(256) NULL,
    NormalizedUserName varchar(256) NULL,
    Email varchar(256) NULL,
    NormalizedEmail varchar(256) NULL,
    EmailConfirmed boolean NOT NULL,
    PasswordHash text NULL,
    SecurityStamp text NULL,
    ConcurrencyStamp text NULL,
    PhoneNumber text NULL,
    PhoneNumberConfirmed boolean NOT NULL,
    TwoFactorEnabled boolean NOT NULL,
    LockoutEnd timestamptz NULL,
    LockoutEnabled boolean NOT NULL,
    AccessFailedCount int4 NOT NULL,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);


CREATE TABLE public.AspNetRoles
(
    Id character varying(128) NOT NULL,
    Name character varying(256) NOT NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);

CREATE TABLE public.AspNetUserRoles
(
    UserId character varying(128) NOT NULL,
    RoleId character varying(128) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId)
        REFERENCES public.AspNetRoles (Id) MATCH SIMPLE
        ON UPDATE CASCADE ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId)
        REFERENCES public.AspNetUsers (Id) MATCH SIMPLE
        ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.AspNetUserClaims
(
    Id serial NOT NULL,
    UserId character varying(128) NOT NULL,
    ClaimType character varying(256),
    ClaimValue character varying(256),
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId)
        REFERENCES public.AspNetUsers (Id) MATCH SIMPLE
        ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE public.AspNetUserLogins
(
    LoginProvider character varying(128) NOT NULL,
    ProviderKey character varying(128) NOT NULL,
    UserId character varying(128) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId)
        REFERENCES public.AspNetUsers (Id) MATCH SIMPLE
        ON UPDATE CASCADE ON DELETE CASCADE
);







CREATE TABLE IF NOT EXISTS departments (
    department_id SERIAL PRIMARY KEY,
    long_name VARCHAR(256) NOT NULL,
    short_name VARCHAR(16) NOT NULL,
    UNIQUE(long_name), UNIQUE(short_name)
);

CREATE TABLE IF NOT EXISTS department_relations (
    department_id INTEGER REFERENCES departments(department_id) NOT NULL,
    user_id VARCHAR(255) REFERENCES aspnetusers(id) NOT NULL,
    PRIMARY KEY(department_id, user_id)
);

CREATE TABLE IF NOT EXISTS subjects (
    subject_id SERIAL PRIMARY KEY,
    "name" VARCHAR(256) NOT NULL,
    short_name VARCHAR(16) NOT NULL,
    description TEXT NOT NULL,
    department_id INTEGER REFERENCES departments(department_id),
    UNIQUE("name"), UNIQUE(short_name)
);

CREATE TABLE IF NOT EXISTS semesters (
    semester_id SERIAL PRIMARY KEY,
    starts_at DATE NOT NULL,
    ends_at DATE NOT NULL,
    season INTEGER NOT NULL,
    CHECK(starts_at < ends_at)
);

CREATE TABLE IF NOT EXISTS semester_events (
    semester_event_id SERIAL PRIMARY KEY,
    semester_id INTEGER REFERENCES semesters(semester_id),
    event_starts TIMESTAMP NOT NULL,
    event_ends TIMESTAMP NOT NULL,
    event_type INTEGER NOT NULL DEFAULT 0,
    description TEXT NOT NULL,
    CHECK (event_ends > event_starts)
);

CREATE TABLE IF NOT EXISTS timetable_events (
    timetable_event_id SERIAL PRIMARY KEY,
    subject_id INTEGER REFERENCES subjects(subject_id) NOT NULL,
    event_type INTEGER NOT NULL,
    owner_id VARCHAR(255) REFERENCES aspnetusers(id) NOT NULL
);

CREATE TABLE IF NOT EXISTS timetable_event_times (
    timetable_event_time_id SERIAL PRIMARY KEY,
    timetable_event_id INTEGER REFERENCES timetable_events(timetable_event_id) NOT NULL,
    starts_at TIMESTAMP NOT NULL,
    ends_at TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS subject_relations (
    subject_relation_id SERIAL PRIMARY KEY,
    subject_id INTEGER REFERENCES subjects(subject_id) NOT NULL,
    user_id VARCHAR(255) REFERENCES aspnetusers(id) NOT NULL,
    relation_type INTEGER NOT NULL,
    UNIQUE(subject_id, user_id, relation_type)
);