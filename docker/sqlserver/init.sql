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
        id              BIGINT              NOT NULL IDENTITY(1,1),
        aggregate_id    UNIQUEIDENTIFIER    NOT NULL,
        aggregate_type  NVARCHAR(100)       NOT NULL,
        event_type      NVARCHAR(200)       NOT NULL,
        payload         NVARCHAR(MAX)       NOT NULL,
        occurred_at     DATETIME2           NOT NULL,
        message_id      UNIQUEIDENTIFIER    NULL,
        CONSTRAINT pk_event_log PRIMARY KEY (id)
    );

    CREATE INDEX ix_event_log_aggregate ON audit.event_log (aggregate_id, aggregate_type);
    CREATE INDEX ix_event_log_occurred_at ON audit.event_log (occurred_at);
    CREATE UNIQUE INDEX uix_event_log_message_id ON audit.event_log (message_id) WHERE message_id IS NOT NULL;
END
GO

-- Migration: add message_id to existing event_log tables
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('audit.event_log') AND name = 'message_id')
    ALTER TABLE audit.event_log ADD message_id UNIQUEIDENTIFIER NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'uix_event_log_message_id' AND object_id = OBJECT_ID('audit.event_log'))
    CREATE UNIQUE INDEX uix_event_log_message_id ON audit.event_log (message_id) WHERE message_id IS NOT NULL;
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

-- ============================================================
-- ARTICLES DOMAIN
-- ============================================================

-- Article categories (user-configurable)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'article_categories')
BEGIN
    CREATE TABLE mdata.article_categories (
        id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        name        NVARCHAR(100)       NOT NULL,
        sort_order  INT                 NOT NULL DEFAULT 0,
        is_active   BIT                 NOT NULL DEFAULT 1,
        created_at  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at  DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_article_categories        PRIMARY KEY (id),
        CONSTRAINT uq_article_categories_name   UNIQUE (name)
    );

    INSERT INTO mdata.article_categories (name, sort_order) VALUES
        ('Koolstofstaal',   0),
        ('RVS',             1),
        ('Non-Ferro',       2),
        ('Aluminium',       3),
        ('Kunststof',       4),
        ('Diversen',        5),
        ('Gereedschapstaal',6),
        ('Gietstuk/deel',   7),
        ('Titaan',          8);
END
GO

-- Units of measure (user-configurable)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'units_of_measure')
BEGIN
    CREATE TABLE mdata.units_of_measure (
        id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        name            NVARCHAR(100)       NOT NULL,
        abbreviation    NVARCHAR(10)        NOT NULL,
        is_active       BIT                 NOT NULL DEFAULT 1,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_units_of_measure      PRIMARY KEY (id),
        CONSTRAINT uq_units_of_measure_name UNIQUE (name)
    );

    INSERT INTO mdata.units_of_measure (name, abbreviation) VALUES
        ('Kilogram',        'kg'),
        ('Meter',           'm'),
        ('Stuk',            'st'),
        ('Uur',             'uur'),
        ('Vierkante meter', 'm²');
END
GO

-- Article number sequence
IF NOT EXISTS (SELECT * FROM sys.sequences seq JOIN sys.schemas s ON seq.schema_id = s.schema_id WHERE s.name = 'mdata' AND seq.name = 'seq_article_number')
    CREATE SEQUENCE mdata.seq_article_number START WITH 1000 INCREMENT BY 1;
GO

-- Articles
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'articles')
BEGIN
    CREATE TABLE mdata.articles (
        id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        article_number      INT                 NOT NULL DEFAULT NEXT VALUE FOR mdata.seq_article_number,
        code                NVARCHAR(50)        NOT NULL,
        name                NVARCHAR(200)       NOT NULL,
        description         NVARCHAR(MAX)       NULL,
        article_type        NVARCHAR(50)        NOT NULL DEFAULT 'raw_material',
        category_id         UNIQUEIDENTIFIER    NULL,
        unit_of_measure_id  UNIQUEIDENTIFIER    NULL,
        purchase_price      DECIMAL(18,4)       NULL,
        is_active           BIT                 NOT NULL DEFAULT 1,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_articles          PRIMARY KEY (id),
        CONSTRAINT uq_articles_number   UNIQUE (article_number),
        CONSTRAINT uq_articles_code     UNIQUE (code),
        CONSTRAINT chk_articles_type    CHECK (article_type IN ('raw_material','manufactured','bought_out','service')),
        CONSTRAINT fk_articles_category FOREIGN KEY (category_id)        REFERENCES mdata.article_categories(id),
        CONSTRAINT fk_articles_uom      FOREIGN KEY (unit_of_measure_id) REFERENCES mdata.units_of_measure(id)
    );

    CREATE INDEX ix_articles_name         ON mdata.articles (name);
    CREATE INDEX ix_articles_is_active    ON mdata.articles (is_active);
    CREATE INDEX ix_articles_category_id  ON mdata.articles (category_id);
    CREATE INDEX ix_articles_article_type ON mdata.articles (article_type);
END
GO

-- Bill of Materials
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'bill_of_materials')
BEGIN
    CREATE TABLE mdata.bill_of_materials (
        id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        parent_article_id   UNIQUEIDENTIFIER    NOT NULL,
        child_article_id    UNIQUEIDENTIFIER    NOT NULL,
        quantity            DECIMAL(18,4)       NOT NULL,
        unit_of_measure_id  UNIQUEIDENTIFIER    NULL,
        sort_order          INT                 NOT NULL DEFAULT 0,
        is_active           BIT                 NOT NULL DEFAULT 1,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_bill_of_materials             PRIMARY KEY (id),
        CONSTRAINT uq_bom_parent_child              UNIQUE (parent_article_id, child_article_id),
        CONSTRAINT chk_bom_no_self_reference        CHECK (parent_article_id <> child_article_id),
        CONSTRAINT fk_bom_parent_article            FOREIGN KEY (parent_article_id) REFERENCES mdata.articles(id),
        CONSTRAINT fk_bom_child_article             FOREIGN KEY (child_article_id)  REFERENCES mdata.articles(id),
        CONSTRAINT fk_bom_uom                       FOREIGN KEY (unit_of_measure_id) REFERENCES mdata.units_of_measure(id)
    );

    CREATE INDEX ix_bom_parent_article_id ON mdata.bill_of_materials (parent_article_id);
    CREATE INDEX ix_bom_child_article_id  ON mdata.bill_of_materials (child_article_id);
END
GO

-- Article history
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'article_history')
BEGIN
    CREATE TABLE audit.article_history (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        article_id      UNIQUEIDENTIFIER    NOT NULL,
        event_sequence  BIGINT              NOT NULL,
        event_type      NVARCHAR(200)       NOT NULL,
        summary         NVARCHAR(500)       NOT NULL,
        changed_by      NVARCHAR(200)       NOT NULL DEFAULT 'system',
        changed_at      DATETIME2           NOT NULL,
        snapshot        NVARCHAR(MAX)       NOT NULL,
        CONSTRAINT pk_article_history       PRIMARY KEY (id),
        CONSTRAINT fk_article_history_event FOREIGN KEY (event_sequence) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_article_history_article_id  ON audit.article_history (article_id, changed_at DESC);
    CREATE INDEX ix_article_history_changed_at  ON audit.article_history (changed_at DESC);
END
GO

-- Article snapshots
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'article_snapshots')
BEGIN
    CREATE TABLE audit.article_snapshots (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        article_id      UNIQUEIDENTIFIER    NOT NULL,
        at_event_id     BIGINT              NOT NULL,
        snapshot        NVARCHAR(MAX)       NOT NULL,
        trigger_reason  NVARCHAR(50)        NOT NULL,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_article_snapshots         PRIMARY KEY (id),
        CONSTRAINT fk_article_snapshots_event   FOREIGN KEY (at_event_id) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_article_snapshots_article_id ON audit.article_snapshots (article_id, at_event_id DESC);
END
GO

PRINT 'Articles schema succesvol geladen.';
GO

-- ============================================================
-- ROUTING TEMPLATE — REFERENCE DATA
-- ============================================================

-- Machine types (reference data — never mutated by the app)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'machine_types')
BEGIN
    CREATE TABLE mdata.machine_types (
        id        UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        name      NVARCHAR(100)       NOT NULL,
        is_active BIT                 NOT NULL DEFAULT 1,
        CONSTRAINT pk_machine_types     PRIMARY KEY (id),
        CONSTRAINT uq_machine_types_name UNIQUE (name)
    );

    INSERT INTO mdata.machine_types (name) VALUES
        ('Lathe'),
        ('Milling Center'),
        ('Grinder'),
        ('Special machine'),
        ('Inspection station');
END
GO

-- Operation types (reference data — never mutated by the app)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'operation_types')
BEGIN
    CREATE TABLE mdata.operation_types (
        id               UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        name             NVARCHAR(100)       NOT NULL,
        is_subcontracted BIT                 NOT NULL DEFAULT 0,
        machine_type_id  UNIQUEIDENTIFIER    NULL,
        is_active        BIT                 NOT NULL DEFAULT 1,
        CONSTRAINT pk_operation_types           PRIMARY KEY (id),
        CONSTRAINT uq_operation_types_name      UNIQUE (name),
        CONSTRAINT fk_operation_types_machine   FOREIGN KEY (machine_type_id) REFERENCES mdata.machine_types(id)
    );

    INSERT INTO mdata.operation_types (name, is_subcontracted, machine_type_id)
    SELECT op.name, op.is_subcontracted, mt.id
    FROM (VALUES
        ('CNC Turning',              CAST(0 AS BIT), 'Lathe'),
        ('CNC Milling',              CAST(0 AS BIT), 'Milling Center'),
        ('Grinding',                 CAST(0 AS BIT), 'Grinder'),
        ('Honing',                   CAST(0 AS BIT), 'Special machine'),
        ('Broaching',                CAST(0 AS BIT), 'Special machine'),
        ('Intermediate inspection',  CAST(0 AS BIT), 'Inspection station'),
        ('Final inspection',         CAST(0 AS BIT), 'Inspection station'),
        ('3D Measuring',             CAST(0 AS BIT), 'Inspection station'),
        ('Sawing',                   CAST(0 AS BIT), NULL),
        ('Deburring',                CAST(0 AS BIT), NULL),
        ('Welding',                  CAST(0 AS BIT), NULL),
        ('Barrel finishing',         CAST(0 AS BIT), NULL),
        ('Marking',                  CAST(0 AS BIT), NULL),
        ('Material issue',           CAST(0 AS BIT), NULL),
        ('Final finishing',          CAST(0 AS BIT), NULL),
        ('Assembly',                 CAST(0 AS BIT), NULL),
        ('Heat treatment',           CAST(1 AS BIT), NULL),
        ('Surface treatment',        CAST(1 AS BIT), NULL),
        ('Stellite / carbide coating', CAST(1 AS BIT), NULL),
        ('Measurement report',       CAST(1 AS BIT), NULL)
    ) AS op(name, is_subcontracted, machine_type_name)
    LEFT JOIN mdata.machine_types mt ON mt.name = op.machine_type_name;
END
GO

-- Article operations (routing template)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'article_operations')
BEGIN
    CREATE TABLE mdata.article_operations (
        id                   UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        article_id           UNIQUEIDENTIFIER    NOT NULL,
        sequence_number      INT                 NOT NULL,
        operation_type_id    UNIQUEIDENTIFIER    NOT NULL,
        operation_type_name  NVARCHAR(100)       NOT NULL,
        is_subcontracted     BIT                 NOT NULL DEFAULT 0,
        estimated_minutes    DECIMAL(8,2)        NULL,
        notes                NVARCHAR(500)       NULL,
        is_conditional       BIT                 NOT NULL DEFAULT 0,
        is_active            BIT                 NOT NULL DEFAULT 1,
        created_at           DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_article_operations            PRIMARY KEY (id),
        CONSTRAINT fk_article_operations_article    FOREIGN KEY (article_id)        REFERENCES mdata.articles(id),
        CONSTRAINT fk_article_operations_op_type    FOREIGN KEY (operation_type_id) REFERENCES mdata.operation_types(id)
    );

    CREATE INDEX ix_article_operations_article_id ON mdata.article_operations (article_id);
END
GO

-- Migration: revision column on articles
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('mdata.articles') AND name = 'revision')
    ALTER TABLE mdata.articles ADD revision NVARCHAR(10) NULL;
GO

PRINT 'Routing template schema succesvol geladen.';
GO

-- ============================================================
-- ORDERS DOMAIN
-- ============================================================

-- Order number sequence
IF NOT EXISTS (SELECT * FROM sys.sequences seq JOIN sys.schemas s ON seq.schema_id = s.schema_id WHERE s.name = 'mdata' AND seq.name = 'seq_order_number')
    CREATE SEQUENCE mdata.seq_order_number AS INT START WITH 1000 INCREMENT BY 1;
GO

-- Production orders
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'production_orders')
BEGIN
    CREATE TABLE mdata.production_orders (
        id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        order_number        INT                 NOT NULL DEFAULT NEXT VALUE FOR mdata.seq_order_number,
        article_id          UNIQUEIDENTIFIER    NOT NULL,
        article_code        NVARCHAR(50)        NOT NULL,
        article_name        NVARCHAR(200)       NOT NULL,
        article_revision    NVARCHAR(10)        NULL,
        customer_id         UNIQUEIDENTIFIER    NULL,
        customer_name       NVARCHAR(200)       NULL,
        quantity            DECIMAL(12,4)       NOT NULL,
        unit_of_measure     NVARCHAR(20)        NOT NULL,
        status              NVARCHAR(20)        NOT NULL DEFAULT 'draft',
        due_date            DATE                NULL,
        notes               NVARCHAR(1000)      NULL,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_production_orders             PRIMARY KEY (id),
        CONSTRAINT uq_production_orders_number      UNIQUE (order_number),
        CONSTRAINT chk_production_orders_status     CHECK (status IN ('draft','released','inprogress','done','cancelled')),
        CONSTRAINT fk_production_orders_article     FOREIGN KEY (article_id)  REFERENCES mdata.articles(id),
        CONSTRAINT fk_production_orders_customer    FOREIGN KEY (customer_id) REFERENCES mdata.parties(id)
    );

    CREATE INDEX ix_production_orders_status     ON mdata.production_orders (status);
    CREATE INDEX ix_production_orders_article_id ON mdata.production_orders (article_id);
    CREATE INDEX ix_production_orders_created_at ON mdata.production_orders (created_at DESC);
END
GO

-- Order BOM snapshot lines
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'order_bom_lines')
BEGIN
    CREATE TABLE mdata.order_bom_lines (
        id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        order_id        UNIQUEIDENTIFIER    NOT NULL,
        component_id    UNIQUEIDENTIFIER    NOT NULL,
        component_code  NVARCHAR(50)        NOT NULL,
        component_name  NVARCHAR(200)       NOT NULL,
        quantity        DECIMAL(12,4)       NOT NULL,
        unit_of_measure NVARCHAR(20)        NOT NULL,
        notes           NVARCHAR(500)       NULL,
        CONSTRAINT pk_order_bom_lines           PRIMARY KEY (id),
        CONSTRAINT fk_order_bom_lines_order     FOREIGN KEY (order_id) REFERENCES mdata.production_orders(id)
    );

    CREATE INDEX ix_order_bom_lines_order_id ON mdata.order_bom_lines (order_id);
END
GO

-- Order operation snapshot lines
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'order_operations')
BEGIN
    CREATE TABLE mdata.order_operations (
        id                   UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        order_id             UNIQUEIDENTIFIER    NOT NULL,
        sequence_number      INT                 NOT NULL,
        operation_type_id    UNIQUEIDENTIFIER    NOT NULL,
        operation_type_name  NVARCHAR(100)       NOT NULL,
        is_subcontracted     BIT                 NOT NULL DEFAULT 0,
        estimated_minutes    DECIMAL(8,2)        NULL,
        notes                NVARCHAR(500)       NULL,
        is_conditional       BIT                 NOT NULL DEFAULT 0,
        CONSTRAINT pk_order_operations          PRIMARY KEY (id),
        CONSTRAINT fk_order_operations_order    FOREIGN KEY (order_id) REFERENCES mdata.production_orders(id)
    );

    CREATE INDEX ix_order_operations_order_id ON mdata.order_operations (order_id);
END
GO

-- Order history (materialized for UI)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'order_history')
BEGIN
    CREATE TABLE audit.order_history (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        order_id        UNIQUEIDENTIFIER    NOT NULL,
        event_sequence  BIGINT              NOT NULL,
        event_type      NVARCHAR(200)       NOT NULL,
        summary         NVARCHAR(500)       NOT NULL,
        changed_by      NVARCHAR(200)       NOT NULL DEFAULT 'system',
        changed_at      DATETIME2           NOT NULL,
        snapshot        NVARCHAR(MAX)       NULL,
        CONSTRAINT pk_order_history         PRIMARY KEY (id),
        CONSTRAINT fk_order_history_event   FOREIGN KEY (event_sequence) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_order_history_order_id   ON audit.order_history (order_id, changed_at DESC);
    CREATE INDEX ix_order_history_changed_at ON audit.order_history (changed_at DESC);
END
GO

-- Order snapshots
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'order_snapshots')
BEGIN
    CREATE TABLE audit.order_snapshots (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        order_id        UNIQUEIDENTIFIER    NOT NULL,
        at_event_id     BIGINT              NOT NULL,
        snapshot        NVARCHAR(MAX)       NOT NULL,
        trigger_reason  NVARCHAR(100)       NOT NULL,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_order_snapshots           PRIMARY KEY (id),
        CONSTRAINT fk_order_snapshots_event     FOREIGN KEY (at_event_id) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_order_snapshots_order_id ON audit.order_snapshots (order_id, at_event_id DESC);
END
GO

PRINT 'Orders schema succesvol geladen.';
GO

-- ============================================================
-- QUOTES DOMAIN
-- ============================================================

-- Quote number sequence
IF NOT EXISTS (SELECT * FROM sys.sequences seq JOIN sys.schemas s ON seq.schema_id = s.schema_id WHERE s.name = 'mdata' AND seq.name = 'seq_quote_number')
    CREATE SEQUENCE mdata.seq_quote_number AS INT START WITH 1000 INCREMENT BY 1;
GO

-- Quotes
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'quotes')
BEGIN
    CREATE TABLE mdata.quotes (
        id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        quote_number        INT                 NOT NULL DEFAULT NEXT VALUE FOR mdata.seq_quote_number,
        customer_id         UNIQUEIDENTIFIER    NULL,
        customer_name       NVARCHAR(200)       NULL,
        date                DATE                NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
        reference           NVARCHAR(200)       NULL,
        contact_person      NVARCHAR(200)       NULL,
        delivery_time       NVARCHAR(100)       NULL,
        hourly_rate         DECIMAL(10,2)       NOT NULL DEFAULT 72,
        material_margin     DECIMAL(5,2)        NOT NULL DEFAULT 115,
        standard_margin     DECIMAL(5,2)        NOT NULL DEFAULT 11,
        setup_time          DECIMAL(8,2)        NOT NULL DEFAULT 1,
        status              NVARCHAR(20)        NOT NULL DEFAULT 'draft',
        remarks             NVARCHAR(MAX)       NULL,
        created_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_at          DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_quotes                PRIMARY KEY (id),
        CONSTRAINT uq_quotes_number         UNIQUE (quote_number),
        CONSTRAINT chk_quotes_status        CHECK (status IN ('draft','sent','accepted','rejected')),
        CONSTRAINT fk_quotes_customer       FOREIGN KEY (customer_id) REFERENCES mdata.parties(id)
    );

    CREATE INDEX ix_quotes_status      ON mdata.quotes (status);
    CREATE INDEX ix_quotes_customer_id ON mdata.quotes (customer_id);
    CREATE INDEX ix_quotes_created_at  ON mdata.quotes (created_at DESC);
END
GO

-- Quote lines
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'mdata' AND t.name = 'quote_lines')
BEGIN
    CREATE TABLE mdata.quote_lines (
        id                      UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWSEQUENTIALID(),
        quote_id                UNIQUEIDENTIFIER    NOT NULL,
        sort_order              INT                 NOT NULL DEFAULT 0,
        part_name               NVARCHAR(200)       NOT NULL,
        part_number             NVARCHAR(100)       NOT NULL,
        quantity                DECIMAL(12,4)       NOT NULL,
        article_id              UNIQUEIDENTIFIER    NULL,
        -- Material (denormalized — material catalog deferred to later iteration)
        material_type           NVARCHAR(100)       NULL,
        material_code           NVARCHAR(50)        NULL,
        material_code2          NVARCHAR(100)       NULL,
        material_geometry       NVARCHAR(50)        NULL,
        material_size_mm        DECIMAL(10,3)       NULL,
        material_length_mm      DECIMAL(10,3)       NULL,
        material_quantity       DECIMAL(12,4)       NULL,
        material_price          DECIMAL(18,4)       NULL,
        material_source         NVARCHAR(20)        NOT NULL DEFAULT 'inclusive',
        -- Operations
        operation_count         INT                 NOT NULL DEFAULT 0,
        operation_time_minutes  DECIMAL(10,2)       NOT NULL DEFAULT 0,
        -- Subcontracting
        subcontracting_count    INT                 NOT NULL DEFAULT 0,
        subcontracting_price    DECIMAL(18,4)       NOT NULL DEFAULT 0,
        -- Pricing
        total_price_per_unit    DECIMAL(18,4)       NULL,
        is_manual_price         BIT                 NOT NULL DEFAULT 0,
        manual_price            DECIMAL(18,4)       NULL,
        is_accepted             BIT                 NOT NULL DEFAULT 0,
        remarks                 NVARCHAR(1000)      NULL,
        CONSTRAINT pk_quote_lines               PRIMARY KEY (id),
        CONSTRAINT chk_quote_lines_source       CHECK (material_source IN ('inclusive','customer')),
        CONSTRAINT fk_quote_lines_quote         FOREIGN KEY (quote_id)    REFERENCES mdata.quotes(id),
        CONSTRAINT fk_quote_lines_article       FOREIGN KEY (article_id)  REFERENCES mdata.articles(id)
    );

    CREATE INDEX ix_quote_lines_quote_id   ON mdata.quote_lines (quote_id);
    CREATE INDEX ix_quote_lines_article_id ON mdata.quote_lines (article_id) WHERE article_id IS NOT NULL;
END
GO

-- Quote history (materialized for UI)
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'quote_history')
BEGIN
    CREATE TABLE audit.quote_history (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        quote_id        UNIQUEIDENTIFIER    NOT NULL,
        event_sequence  BIGINT              NOT NULL,
        event_type      NVARCHAR(200)       NOT NULL,
        summary         NVARCHAR(500)       NOT NULL,
        changed_by      NVARCHAR(200)       NOT NULL DEFAULT 'system',
        changed_at      DATETIME2           NOT NULL,
        snapshot        NVARCHAR(MAX)       NULL,
        CONSTRAINT pk_quote_history         PRIMARY KEY (id),
        CONSTRAINT fk_quote_history_event   FOREIGN KEY (event_sequence) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_quote_history_quote_id   ON audit.quote_history (quote_id, changed_at DESC);
    CREATE INDEX ix_quote_history_changed_at ON audit.quote_history (changed_at DESC);
END
GO

-- Quote snapshots
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'audit' AND t.name = 'quote_snapshots')
BEGIN
    CREATE TABLE audit.quote_snapshots (
        id              BIGINT              NOT NULL IDENTITY(1,1),
        quote_id        UNIQUEIDENTIFIER    NOT NULL,
        at_event_id     BIGINT              NOT NULL,
        snapshot        NVARCHAR(MAX)       NOT NULL,
        trigger_reason  NVARCHAR(50)        NOT NULL,
        created_at      DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT pk_quote_snapshots           PRIMARY KEY (id),
        CONSTRAINT fk_quote_snapshots_event     FOREIGN KEY (at_event_id) REFERENCES audit.event_log(id)
    );

    CREATE INDEX ix_quote_snapshots_quote_id ON audit.quote_snapshots (quote_id, at_event_id DESC);
END
GO

-- Migration: add quote_id to production_orders
-- NOTE: CREATE INDEX is in a separate GO batch to avoid SQL Server compile-time column validation failure.
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('mdata.production_orders') AND name = 'quote_id')
BEGIN
    ALTER TABLE mdata.production_orders ADD quote_id UNIQUEIDENTIFIER NULL;
    ALTER TABLE mdata.production_orders ADD CONSTRAINT fk_production_orders_quote
        FOREIGN KEY (quote_id) REFERENCES mdata.quotes(id);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('mdata.production_orders') AND name = 'ix_production_orders_quote_id')
    CREATE INDEX ix_production_orders_quote_id ON mdata.production_orders (quote_id) WHERE quote_id IS NOT NULL;
GO

PRINT 'Quotes schema succesvol geladen.';
GO
