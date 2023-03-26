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