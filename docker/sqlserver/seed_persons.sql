USE ErpDb;
GO

-- Personen zonder speciale tekens in de SQL zelf
-- Speciale tekens worden opgeslagen als NVARCHAR via N'' prefix
INSERT INTO mdata.parties (party_type_id, name, is_active) VALUES
(2, N'Jan de Vries', 1),
(2, N'Petra van den Berg', 1),
(2, N'Mohammed El-Masri', 1),
(2, N'IJsbrand van der Meer', 1),
(2, N'François Dubois', 1),
(2, N'René Müller', 1),
(2, N'Ève de Groot', 1),
(2, N'Joël van de Laar', 1),
(2, N'Søren Andersen', 1),
(2, N'Ángel García', 1),
(2, N'Li Wu', 1),
(2, N'Bartholomeus Winterbottom', 1),
(2, N'Maria-José Hernández', 1),
(2, N'Seán O''Brien', 1),
(2, N'D''Artagnan de Boer', 1),
(2, N'Kim Jansen', 1),
(2, N'Robin van Dijk', 1),
(2, N'Roos Bakker', 1),
(2, N'McDonald de Wit', 1),
(2, N'Anna-Lien Vermeersch', 1),
(2, N'Fatima Çelik', 1),
(2, N'Mehmet Yılmaz', 1),
(2, N'Agnieszka Kowalski', 1),
(2, N'Piotr Wróblewski', 1),
(2, N'Günter von der Heide', 1),
(2, N'Sigríður Björnsdóttir', 1),
(2, N'Jean-Pierre De Smedt', 1),
(2, N'Nguyên Thị Hương', 1),
(2, N'Weronika Żółtowska', 1),
(2, N'Sofía Martínez', 1),
(2, N'Hans-Jürgen Schröder', 1),
(2, N'Björk Guðmundsdóttir', 1),
(2, N'Alžběta Nováková', 1),
(2, N'Miloš Formanović', 1),
(2, N'Leïla Benali', 1),
(2, N'Jürgen van der Straaten', 1),
(2, N'Céline Dupont-Bernard', 1),
(2, N'Władysław Szczepanski', 1),
(2, N'Ingrid Åkesson', 1),
(2, N'Pieter-Jan De Cock', 1),
(2, N'Özlem Demir', 1),
(2, N'Şükrü Arslan', 1),
(2, N'María de los Ángeles Ruiz', 1),
(2, N'Jan-Willem van ''t Hof', 1),
(2, N'IJda van IJzendoorn', 1),
(2, N'Zoë van der Hoeven', 1),
(2, N'Hervé-Louis d''Entremont', 1),
(2, N'Anästasia Bäuerle', 1),
(2, N'A.', 1),
(2, N' ', 1);
GO

-- Person details
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Jan', N'de', N'Vries', N'J.' FROM mdata.parties p WHERE p.name = N'Jan de Vries';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Petra', N'van den', N'Berg', N'P.' FROM mdata.parties p WHERE p.name = N'Petra van den Berg';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Mohammed', NULL, N'El-Masri', N'M.' FROM mdata.parties p WHERE p.name = N'Mohammed El-Masri';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'IJsbrand', N'van der', N'Meer', N'IJ.' FROM mdata.parties p WHERE p.name = N'IJsbrand van der Meer';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'François', NULL, N'Dubois', N'F.' FROM mdata.parties p WHERE p.name = N'François Dubois';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'René', NULL, N'Müller', N'R.' FROM mdata.parties p WHERE p.name = N'René Müller';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Ève', N'de', N'Groot', N'È.' FROM mdata.parties p WHERE p.name = N'Ève de Groot';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Joël', N'van de', N'Laar', N'J.' FROM mdata.parties p WHERE p.name = N'Joël van de Laar';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Søren', NULL, N'Andersen', N'S.' FROM mdata.parties p WHERE p.name = N'Søren Andersen';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Ángel', NULL, N'García', N'Á.' FROM mdata.parties p WHERE p.name = N'Ángel García';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Li', NULL, N'Wu', N'L.' FROM mdata.parties p WHERE p.name = N'Li Wu';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Bartholomeus', NULL, N'Winterbottom', N'B.' FROM mdata.parties p WHERE p.name = N'Bartholomeus Winterbottom';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Maria-José', NULL, N'Hernández', N'M.J.' FROM mdata.parties p WHERE p.name = N'Maria-José Hernández';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Seán', NULL, N'O''Brien', N'S.' FROM mdata.parties p WHERE p.name = N'Seán O''Brien';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'D''Artagnan', N'de', N'Boer', N'D.' FROM mdata.parties p WHERE p.name = N'D''Artagnan de Boer';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Kim', NULL, N'Jansen', N'K.' FROM mdata.parties p WHERE p.name = N'Kim Jansen';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Robin', N'van', N'Dijk', N'R.' FROM mdata.parties p WHERE p.name = N'Robin van Dijk';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Roos', NULL, N'Bakker', N'R.' FROM mdata.parties p WHERE p.name = N'Roos Bakker';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'McDonald', N'de', N'Wit', N'M.' FROM mdata.parties p WHERE p.name = N'McDonald de Wit';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Anna-Lien', NULL, N'Vermeersch', N'A.L.' FROM mdata.parties p WHERE p.name = N'Anna-Lien Vermeersch';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Fatima', NULL, N'Çelik', N'F.' FROM mdata.parties p WHERE p.name = N'Fatima Çelik';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Mehmet', NULL, N'Yılmaz', N'M.' FROM mdata.parties p WHERE p.name = N'Mehmet Yılmaz';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Agnieszka', NULL, N'Kowalski', N'A.' FROM mdata.parties p WHERE p.name = N'Agnieszka Kowalski';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Piotr', NULL, N'Wróblewski', N'P.' FROM mdata.parties p WHERE p.name = N'Piotr Wróblewski';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Günter', N'von der', N'Heide', N'G.' FROM mdata.parties p WHERE p.name = N'Günter von der Heide';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Jean-Pierre', N'De', N'Smedt', N'J.P.' FROM mdata.parties p WHERE p.name = N'Jean-Pierre De Smedt';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Sofía', NULL, N'Martínez', N'S.' FROM mdata.parties p WHERE p.name = N'Sofía Martínez';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Hans-Jürgen', NULL, N'Schröder', N'H.J.' FROM mdata.parties p WHERE p.name = N'Hans-Jürgen Schröder';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Leïla', NULL, N'Benali', N'L.' FROM mdata.parties p WHERE p.name = N'Leïla Benali';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Jürgen', N'van der', N'Straaten', N'J.' FROM mdata.parties p WHERE p.name = N'Jürgen van der Straaten';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Céline', NULL, N'Dupont-Bernard', N'C.' FROM mdata.parties p WHERE p.name = N'Céline Dupont-Bernard';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Ingrid', NULL, N'Åkesson', N'I.' FROM mdata.parties p WHERE p.name = N'Ingrid Åkesson';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Pieter-Jan', N'De', N'Cock', N'P.J.' FROM mdata.parties p WHERE p.name = N'Pieter-Jan De Cock';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Özlem', NULL, N'Demir', N'Ö.' FROM mdata.parties p WHERE p.name = N'Özlem Demir';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'María', N'de los', N'Ángeles Ruiz', N'M.' FROM mdata.parties p WHERE p.name = N'María de los Ángeles Ruiz';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Jan-Willem', N'van ''t', N'Hof', N'J.W.' FROM mdata.parties p WHERE p.name = N'Jan-Willem van ''t Hof';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'IJda', N'van', N'IJzendoorn', N'IJ.' FROM mdata.parties p WHERE p.name = N'IJda van IJzendoorn';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Zoë', N'van der', N'Hoeven', N'Z.' FROM mdata.parties p WHERE p.name = N'Zoë van der Hoeven';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Hervé-Louis', N'd''', N'Entremont', N'H.L.' FROM mdata.parties p WHERE p.name = N'Hervé-Louis d''Entremont';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, N'Anästasia', NULL, N'Bäuerle', N'A.' FROM mdata.parties p WHERE p.name = N'Anästasia Bäuerle';
GO

-- Adressen voor personen
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 1, 'Gewonestraat', CAST(ROW_NUMBER() OVER (ORDER BY p.name) AS NVARCHAR), '1234 AB', 'Amsterdam', 'NL', 1
FROM mdata.parties p WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';
GO

-- Contactmethodes voor personen
INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 3, '06' + RIGHT('00000000' + CAST(ABS(CHECKSUM(p.name)) % 90000000 + 10000000 AS NVARCHAR), 8), 1
FROM mdata.parties p WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';
GO

-- Party relationships
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Bout & Moer Holding B.V.' AND per.name = N'Jan de Vries';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'De Roestvrije Droom B.V.' AND per.name = N'Petra van den Berg';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Mueller Stahlbau GmbH' AND per.name = N'Günter von der Heide';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Schnell und Gunstig Stahl GmbH' AND per.name = N'Hans-Jürgen Schröder';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'De Smedt Metaalwerken NV' AND per.name = N'Jean-Pierre De Smedt';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Vlak & Glad Metaalbewerking B.V.' AND per.name = N'Mohammed El-Masri';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Niemand & Zonen B.V.' AND per.name = N'IJsbrand van der Meer';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Twee Petten Op B.V.' AND per.name = N'Kim Jansen';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Twee Petten Op B.V.' AND per.name = N'Robin van Dijk';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Heen En Weer Handel B.V.' AND per.name = N'Sofía Martínez';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Koop En Verkoop Zo B.V.' AND per.name = N'Sofía Martínez';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Altijd Op Voorraad B.V.' AND per.name = N'Agnieszka Kowalski';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Levertijd Onbekend B.V.' AND per.name = N'Leïla Benali';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Van Alles Wat Metaal B.V.' AND per.name = N'Jan-Willem van ''t Hof';
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Wij Doen Alles B.V.' AND per.name = N'IJda van IJzendoorn';
GO

PRINT 'Personen seed klaar.';
GO

-- ============================================================
-- HISTORY EN SNAPSHOTS voor geseedde personen
-- ============================================================

INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
SELECT
    p.id,
    'Party',
    'PartyCreated',
    (SELECT p.id AS PartyId, p.name AS Name, p.party_type_id AS PartyType,
        0 AS IsCustomer, 0 AS IsSupplier,
        SYSUTCDATETIME() AS OccurredAt
     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
    SYSUTCDATETIME()
FROM mdata.parties p
WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';
GO

INSERT INTO audit.party_history (party_id, event_sequence, event_type, summary, changed_by, changed_at, snapshot)
SELECT
    p.id,
    e.id,
    'PartyCreated',
    'Party aangemaakt: ' + p.name,
    'seed',
    SYSUTCDATETIME(),
    e.payload
FROM mdata.parties p
JOIN audit.event_log e ON e.aggregate_id = p.id AND e.event_type = 'PartyCreated'
WHERE p.party_type_id = 2;
GO

INSERT INTO audit.party_snapshots (party_id, at_event_id, snapshot, trigger_reason)
SELECT
    p.id,
    e.id,
    e.payload,
    'state_closed'
FROM mdata.parties p
JOIN audit.event_log e ON e.aggregate_id = p.id AND e.event_type = 'PartyCreated'
WHERE p.party_type_id = 2;
GO

PRINT 'Personen history en snapshots klaar.';
GO
