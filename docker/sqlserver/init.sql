-- ============================================================
-- Database aanmaken
-- ============================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ErpDb')
BEGIN
    CREATE DATABASE ErpDb;
END
GO

USE ErpDb;
GO

-- ============================================================
-- Schema aanmaken
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'mdata')
BEGIN
    EXEC('CREATE SCHEMA mdata');
END
GO

-- ============================================================
-- PARTY DOMAIN
-- ============================================================

-- Party types
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_types')
BEGIN
    CREATE TABLE mdata.party_types (
        id      TINYINT         NOT NULL,
        name    NVARCHAR(50)    NOT NULL,
        CONSTRAINT pk_party_types PRIMARY KEY (id)
    );

    INSERT INTO mdata.party_types (id, name) VALUES
        (1, 'Organization'),
        (2, 'Person');
END
GO

-- Parties
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'parties')
BEGIN
    CREATE TABLE mdata.parties (
        id                          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        party_type_id               TINYINT             NOT NULL,
        name                        NVARCHAR(200)       NOT NULL,
        is_active                   BIT                 NOT NULL DEFAULT 1,
        created_at                  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at                  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_parties               PRIMARY KEY (id),
        CONSTRAINT fk_parties_party_type    FOREIGN KEY (party_type_id) REFERENCES mdata.party_types(id)
    );

    CREATE INDEX ix_parties_name        ON mdata.parties (name);
    CREATE INDEX ix_parties_is_active   ON mdata.parties (is_active);
END
GO

-- Address types
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'address_types')
BEGIN
    CREATE TABLE mdata.address_types (
        id      TINYINT         NOT NULL,
        name    NVARCHAR(50)    NOT NULL,
        CONSTRAINT pk_address_types PRIMARY KEY (id)
    );

    INSERT INTO mdata.address_types (id, name) VALUES
        (1, 'Postal'),
        (2, 'Delivery'),
        (3, 'Invoice');
END
GO

-- Party addresses
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_addresses')
BEGIN
    CREATE TABLE mdata.party_addresses (
        id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        party_id                UNIQUEIDENTIFIER    NOT NULL,
        address_type_id         TINYINT             NOT NULL,
        street                  NVARCHAR(200)       NOT NULL,
        house_number            NVARCHAR(10)        NOT NULL,
        house_number_addition   NVARCHAR(10)        NULL,
        postal_code             NVARCHAR(10)        NOT NULL,
        city                    NVARCHAR(100)       NOT NULL,
        country_code            CHAR(2)             NOT NULL DEFAULT 'NL',
        attention               NVARCHAR(100)       NULL,
        is_default              BIT                 NOT NULL DEFAULT 0,
        created_at              DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_party_addresses               PRIMARY KEY (id),
        CONSTRAINT fk_party_addresses_party         FOREIGN KEY (party_id) REFERENCES mdata.parties(id),
        CONSTRAINT fk_party_addresses_type          FOREIGN KEY (address_type_id) REFERENCES mdata.address_types(id)
    );

    CREATE INDEX ix_party_addresses_party_id ON mdata.party_addresses (party_id);
END
GO

-- Contact method types
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'contact_method_types')
BEGIN
    CREATE TABLE mdata.contact_method_types (
        id      TINYINT         NOT NULL,
        name    NVARCHAR(50)    NOT NULL,
        CONSTRAINT pk_contact_method_types PRIMARY KEY (id)
    );

    INSERT INTO mdata.contact_method_types (id, name) VALUES
        (1, 'Phone'),
        (2, 'Email'),
        (3, 'Mobile');
END
GO

-- Party contact methods
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_contact_methods')
BEGIN
    CREATE TABLE mdata.party_contact_methods (
        id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        party_id                UNIQUEIDENTIFIER    NOT NULL,
        contact_method_type_id  TINYINT             NOT NULL,
        value                   NVARCHAR(200)       NOT NULL,
        is_primary              BIT                 NOT NULL DEFAULT 0,
        created_at              DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_party_contact_methods         PRIMARY KEY (id),
        CONSTRAINT fk_party_contact_methods_party   FOREIGN KEY (party_id) REFERENCES mdata.parties(id),
        CONSTRAINT fk_party_contact_methods_type    FOREIGN KEY (contact_method_type_id) REFERENCES mdata.contact_method_types(id)
    );

    CREATE INDEX ix_party_contact_methods_party_id ON mdata.party_contact_methods (party_id);
END
GO

-- Party bank accounts
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_bank_accounts')
BEGIN
    CREATE TABLE mdata.party_bank_accounts (
        id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        party_id        UNIQUEIDENTIFIER    NOT NULL,
        iban            NVARCHAR(34)        NOT NULL,
        bic             NVARCHAR(11)        NULL,
        account_holder  NVARCHAR(200)       NULL,
        is_primary      BIT                 NOT NULL DEFAULT 0,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_party_bank_accounts           PRIMARY KEY (id),
        CONSTRAINT fk_party_bank_accounts_party     FOREIGN KEY (party_id) REFERENCES mdata.parties(id)
    );

    CREATE INDEX ix_party_bank_accounts_party_id ON mdata.party_bank_accounts (party_id);
END
GO

-- Party relationship types
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_relationship_types')
BEGIN
    CREATE TABLE mdata.party_relationship_types (
        id      TINYINT         NOT NULL,
        name    NVARCHAR(50)    NOT NULL,
        CONSTRAINT pk_party_relationship_types PRIMARY KEY (id)
    );

    INSERT INTO mdata.party_relationship_types (id, name) VALUES
        (1, 'ContactPerson'),
        (2, 'Subsidiary');
END
GO

-- Party relationships
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'party_relationships')
BEGIN
    CREATE TABLE mdata.party_relationships (
        id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        from_party_id           UNIQUEIDENTIFIER    NOT NULL,
        to_party_id             UNIQUEIDENTIFIER    NOT NULL,
        relationship_type_id    TINYINT             NOT NULL,
        created_at              DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_party_relationships               PRIMARY KEY (id),
        CONSTRAINT fk_party_relationships_from_party    FOREIGN KEY (from_party_id) REFERENCES mdata.parties(id),
        CONSTRAINT fk_party_relationships_to_party      FOREIGN KEY (to_party_id) REFERENCES mdata.parties(id),
        CONSTRAINT fk_party_relationships_type          FOREIGN KEY (relationship_type_id) REFERENCES mdata.party_relationship_types(id),
        CONSTRAINT uq_party_relationships               UNIQUE (from_party_id, to_party_id, relationship_type_id)
    );
END
GO

-- Sequences voor business nummers
IF NOT EXISTS (SELECT * FROM sys.sequences seq JOIN sys.schemas s ON seq.schema_id = s.schema_id WHERE s.name = 'mdata' AND seq.name = 'seq_debtor_number')
    CREATE SEQUENCE mdata.seq_debtor_number START WITH 1000 INCREMENT BY 1;
GO

IF NOT EXISTS (SELECT * FROM sys.sequences seq JOIN sys.schemas s ON seq.schema_id = s.schema_id WHERE s.name = 'mdata' AND seq.name = 'seq_supplier_number')
    CREATE SEQUENCE mdata.seq_supplier_number START WITH 1000 INCREMENT BY 1;
GO

-- Customer roles
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'customer_roles')
BEGIN
    CREATE TABLE mdata.customer_roles (
        party_id            UNIQUEIDENTIFIER    NOT NULL,
        debtor_number       INT                 NOT NULL DEFAULT NEXT VALUE FOR mdata.seq_debtor_number,
        discount            DECIMAL(5,2)        NOT NULL DEFAULT 0,
        is_vat_shifted      BIT                 NOT NULL DEFAULT 0,
        payment_term_days   SMALLINT            NOT NULL DEFAULT 30,
        credit_limit        DECIMAL(18,2)       NULL,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_customer_roles        PRIMARY KEY (party_id),
        CONSTRAINT fk_customer_roles_party  FOREIGN KEY (party_id) REFERENCES mdata.parties(id),
        CONSTRAINT uq_customer_debtor       UNIQUE (debtor_number)
    );

    CREATE INDEX ix_customer_roles_debtor_number ON mdata.customer_roles (debtor_number);
END
GO

-- Supplier roles
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'supplier_roles')
BEGIN
    CREATE TABLE mdata.supplier_roles (
        party_id            UNIQUEIDENTIFIER    NOT NULL,
        supplier_number     INT                 NOT NULL DEFAULT NEXT VALUE FOR mdata.seq_supplier_number,
        is_vat_shifted      BIT                 NOT NULL DEFAULT 0,
        payment_term_days   SMALLINT            NOT NULL DEFAULT 30,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_supplier_roles        PRIMARY KEY (party_id),
        CONSTRAINT fk_supplier_roles_party  FOREIGN KEY (party_id) REFERENCES mdata.parties(id),
        CONSTRAINT uq_supplier_number       UNIQUE (supplier_number)
    );

    CREATE INDEX ix_supplier_roles_supplier_number ON mdata.supplier_roles (supplier_number);
END
GO

PRINT 'Schema succesvol geladen.';
GO

-- ============================================================
-- PERSON DETAILS
-- Alleen voor parties van type Person
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'person_details')
BEGIN
    CREATE TABLE mdata.person_details (
        party_id        UNIQUEIDENTIFIER    NOT NULL,
        first_name      NVARCHAR(100)       NOT NULL,
        middle_name     NVARCHAR(100)       NULL,       -- tussenvoegsel: 'de', 'van der', etc.
        last_name       NVARCHAR(200)       NOT NULL,
        initials        NVARCHAR(20)        NULL,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_person_details        PRIMARY KEY (party_id),
        CONSTRAINT fk_person_details_party  FOREIGN KEY (party_id) REFERENCES mdata.parties(id)
    );

    CREATE INDEX ix_person_details_last_name ON mdata.person_details (last_name);
END
GO

-- ============================================================
-- ORGANIZATION DETAILS
-- Alleen voor parties van type Organization
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'organization_details')
BEGIN
    CREATE TABLE mdata.organization_details (
        party_id                    UNIQUEIDENTIFIER    NOT NULL,
        vat_number                  NVARCHAR(20)        NULL,
        chamber_of_commerce_number  NVARCHAR(20)        NULL,
        created_at                  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at                  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_organization_details          PRIMARY KEY (party_id),
        CONSTRAINT fk_organization_details_party    FOREIGN KEY (party_id) REFERENCES mdata.parties(id)
    );
END
GO

-- ============================================================
-- AUDIT SCHEMA
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'audit')
BEGIN
    EXEC('CREATE SCHEMA audit');
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'event_log')
BEGIN
    CREATE TABLE audit.event_log (
        id              BIGINT          NOT NULL IDENTITY(1,1),
        aggregate_id    UNIQUEIDENTIFIER NOT NULL,
        aggregate_type  NVARCHAR(100)   NOT NULL,
        event_type      NVARCHAR(200)   NOT NULL,
        payload         NVARCHAR(MAX)   NOT NULL,
        occurred_at     DATETIME2       NOT NULL,
        CONSTRAINT pk_event_log PRIMARY KEY (id)
    );

    CREATE INDEX ix_event_log_aggregate ON audit.event_log (aggregate_id, aggregate_type);
    CREATE INDEX ix_event_log_occurred_at ON audit.event_log (occurred_at);
END
GO

-- ============================================================
-- AUDIT HISTORY EN SNAPSHOTS
-- ============================================================

-- Party history - gematerialiseerde weergave voor UI
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'party_history')
BEGIN
    CREATE TABLE audit.party_history (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        party_id        UNIQUEIDENTIFIER    NOT NULL,
        event_sequence  BIGINT              NOT NULL,   -- verwijzing naar event_log.id
        event_type      NVARCHAR(200)       NOT NULL,
        summary         NVARCHAR(500)       NOT NULL,   -- mensleesbaar: "Naam gewijzigd van X naar Y"
        changed_by      NVARCHAR(200)       NOT NULL DEFAULT 'system',  -- placeholder voor auth
        changed_at      DATETIME2           NOT NULL,
        snapshot        NVARCHAR(MAX)       NOT NULL,   -- JSON volledige staat op dit moment
        CONSTRAINT pk_party_history PRIMARY KEY (id),
        CONSTRAINT fk_party_history_event FOREIGN KEY (event_sequence) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_party_history_party_id  ON audit.party_history (party_id, changed_at DESC);
    CREATE INDEX ix_party_history_changed_at ON audit.party_history (changed_at DESC);
END
GO

-- Party snapshots - voor efficiënt replayen van events
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'party_snapshots')
BEGIN
    CREATE TABLE audit.party_snapshots (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        party_id        UNIQUEIDENTIFIER    NOT NULL,
        at_event_id     BIGINT              NOT NULL,   -- event_log.id waarop snapshot is gebaseerd
        snapshot        NVARCHAR(MAX)       NOT NULL,   -- JSON volledige staat
        trigger_reason  NVARCHAR(50)        NOT NULL,   -- 'event_count', 'state_closed', 'scheduled'
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_party_snapshots PRIMARY KEY (id),
        CONSTRAINT fk_party_snapshots_event FOREIGN KEY (at_event_id) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_party_snapshots_party_id ON audit.party_snapshots (party_id, at_event_id DESC);
END
GO
