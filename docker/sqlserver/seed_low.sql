-- ============================================================
-- SEED DATA - LOW ENVIRONMENT
-- Tongue-in-cheek organisatienamen, edge case contactpersonen
-- ============================================================

USE ErpDb;
GO

-- ============================================================
-- PERSONEN (50) - Edge case testset voor namen
-- ============================================================

INSERT INTO mdata.parties (party_type_id, name, is_active) VALUES
-- Standaard Nederlandse namen met tussenvoegsels
(2, 'Jan de Vries',                         1),
(2, 'Petra van den Berg',                   1),
(2, 'Mohammed El-Masri',                    1),
(2, 'IJsbrand van der Meer',                1),  -- begint met IJ
(2, 'François Dubois',                      1),  -- cedille
(2, 'René Müller',                          1),  -- accent + umlaut
(2, 'Ève de Groot',                         1),  -- accent grave
(2, 'Joël van de Laar',                     1),  -- trema
(2, 'Søren Andersen',                       1),  -- Scandinavisch
(2, 'Ángel García',                         1),  -- tilde
(2, 'Li Wu',                                1),  -- zeer korte naam
(2, 'Bartholomeus Winterbottom',            1),  -- zeer lange naam
(2, 'Maria-José Hernández',                 1),  -- koppelteken voornaam + accent
(2, 'Seán O''Brien',                        1),  -- apostrof, Iers
(2, 'D''Artagnan de Boer',                  1),  -- apostrof aan begin
(2, 'Kim Jansen',                           1),  -- geslachtsneutraal
(2, 'Robin van Dijk',                       1),  -- geslachtsneutraal
(2, 'Roos Bakker',                          1),  -- geslachtsneutraal
(2, 'McDonald de Wit',                      1),  -- binnenhoofdletter
(2, 'Anna-Lien Vermeersch',                 1),  -- koppelteken voornaam, Vlaams
(2, 'Fatima Çelik',                         1),  -- cedille, Turks
(2, 'Mehmet Yılmaz',                        1),  -- puntloze i, Turks
(2, 'Agnieszka Kowalski',                   1),  -- Oost-Europees
(2, 'Piotr Wróblewski',                     1),  -- Poolse diakriet
(2, 'Günter von der Heide',                 1),  -- umlaut + Duits tussenvoegsel
(2, 'Sigríður Björnsdóttir',                1),  -- IJslands, meerdere diacritica
(2, 'Jean-Pierre De Smedt',                 1),  -- koppelteken + Belgisch hoofdletter tussenvoegsel
(2, 'Nguyễn Thị Hương',                     1),  -- Vietnamese tonen
(2, 'Weronika Żółtowska',                   1),  -- Poolse diakritische tekens
(2, 'Sofía Martínez',                       1),  -- accent aigu
(2, 'Hans-Jürgen Schröder',                 1),  -- koppelteken + umlaut, Duits
(2, 'Björk Guðmundsdóttir',                 1),  -- IJslands, eth teken
(2, 'Alžběta Nováková',                     1),  -- haček, Tsjechisch
(2, 'Miloš Formanović',                     1),  -- haček, Servisch
(2, 'Leïla Benali',                         1),  -- trema midden in naam
(2, 'Jürgen van der Straaten',              1),  -- umlaut + NL tussenvoegsel
(2, 'Céline Dupont-Bernard',                1),  -- accent + koppelteken achternaam
(2, 'Władysław Szczepański',                1),  -- Poolse l-streep
(2, 'Ingrid Åkesson',                       1),  -- Zweeds ringteken
(2, 'Pieter-Jan De Cock',                   1),  -- koppelteken + Belgisch
(2, 'Özlem Demir',                          1),  -- umlaut aan begin, Turks
(2, 'Şükrü Arslan',                         1),  -- cedille aan begin, Turks
(2, 'María de los Ángeles Ruiz',            1),  -- Spaanse meerdelige naam
(2, 'Jan-Willem van ''t Hof',              1),  -- apostrof in tussenvoegsel
(2, 'IJda van IJzendoorn',                  1),  -- IJ tweemaal
(2, 'Zoë van der Hoeven',                   1),  -- trema op e
(2, 'Hervé-Louis d''Entremont',             1),  -- apostrof + accent, Frans
(2, 'Anästasia Bäuerle',                    1),  -- twee umlauts
(2, 'A.',                                   1),  -- alleen initiaal, geen achternaam
(2, ' ',                                    1);  -- lege naam / whitespace edge case
GO

-- ============================================================
-- ORGANISATIES (50)
-- Verdeling: ~30 klanten, ~15 leveranciers, ~10 beide, ~5 inactief
-- ============================================================

INSERT INTO mdata.parties (party_type_id, name, is_active) VALUES
-- Klanten
(1, 'Bout & Moer Holding B.V.',                                                                     1, 'NL001234567B01', '12345671'),
(1, 'De Roestvrije Droom B.V.',                                                                     1, 'NL002345678B01', '12345672'),
(1, 'Vlak & Glad Metaalbewerking B.V.',                                                             1, 'NL003456789B01', '12345673'),
(1, 'Staal & Weinig Constructie B.V.',                                                              1, 'NL004567890B01', '12345674'),
(1, 'Gebroeders Ergens B.V.',                                                                       1, 'NL005678901B01', '12345675'),
(1, 'Niemand & Zonen B.V.',                                                                         1, 'NL006789012B01', '12345676'),
(1, 'Iemand Anders Techniek B.V.',                                                                  1, 'NL007890123B01', '12345677'),
(1, 'Van Hier Naar Daar Metaal B.V.',                                                               1, 'NL008901234B01', '12345678'),
(1, 'Zo Gezegd Zo Gesneden B.V.',                                                                   1, 'NL009012345B01', '12345679'),
(1, 'Bijna Klaar Industrie B.V.',                                                                   1, 'NL010123456B01', '12345680'),
(1, 'Wel En Niet B.V.',                                                                             1, 'NL011234567B01', '12345681'),
(1, 'Haast Klaar Fabricage B.V.',                                                                   1, 'NL012345678B01', '12345682'),
(1, 'Altijd Wat Machinefabriek B.V.',                                                               1, 'NL013456789B01', '12345683'),
(1, 'Ooit Eens B.V.',                                                                               1, 'NL014567890B01', '12345684'),
(1, 'Morgen Beter Staalwerken B.V.',                                                                1, 'NL015678901B01', '12345685'),
(1, 'Nu Meteen Plaatbewerking B.V.',                                                                1, 'NL016789012B01', '12345686'),
(1, 'Zo Maar Even B.V.',                                                                            1, 'NL017890123B01', '12345687'),
(1, 'Prima Hoor Techniek B.V.',                                                                     1, 'NL018901234B01', '12345688'),
(1, 'Dat Komt Wel Goed B.V.',                                                                       1, 'NL019012345B01', '12345689'),
(1, 'Eigenlijk Niet B.V.',                                                                          1, 'NL020123456B01', '12345690'),
-- Lange naam edge case
(1, 'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.',          1, 'NL021234567B01', '12345691'),
-- Naam begint met cijfer
(1, '3M Staalwerken B.V.',                                                                          1, 'NL022345678B01', '12345692'),
-- Bijna identieke namen
(1, 'Van Dam Metaal B.V.',                                                                          1, 'NL023456789B01', '12345693'),
(1, 'Van Dam Metaal & Staal B.V.',                                                                  1, 'NL024567890B01', '12345694'),
-- Buitenlandse klanten
(1, 'Müller Stahlbau GmbH',                                                                         1, 'DE123456789',    'HRB12345'),
(1, 'De Smedt Metaalwerken NV',                                                                     1, 'BE0123456789',   '0123456789'),
-- Meer klanten
(1, 'Nauwkeurig Op De Millimeter B.V.',                                                             1, 'NL039012345B01', '12345710'),
(1, 'Tolerantie Nul Komma Nul B.V.',                                                                1, 'NL040123456B01', '12345711'),
(1, 'Scherp Geprijsd Staal B.V.',                                                                   1, 'NL041234567B01', '12345712'),
(1, 'Precies Goed Zo B.V.',                                                                         1, 'NL042345678B01', '12345713'),
(1, 'Recht Toe Recht Aan Techniek B.V.',                                                            1, 'NL043456789B01', '12345714'),
(1, 'Geen Gezeur Gewoon Doen B.V.',                                                                 1, 'NL044567890B01', '12345715'),
(1, 'Lassen Voor Dummies B.V.',                                                                     1, 'NL045678901B01', '12345716'),
(1, 'Vonken Vliegen Lasbedrijf B.V.',                                                               1, 'NL046789012B01', '12345717'),
(1, 'Buigen Maar Niet Breken B.V.',                                                                 1, 'NL047890123B01', '12345718'),
(1, 'Zwaar Werk Lichte Prijzen B.V.',                                                               1, 'NL048901234B01', '12345719'),
-- Leveranciers
(1, 'Altijd Op Voorraad B.V.',                                                                      1, 'NL025678901B01', '12345695'),
(1, 'Nergens Meer B.V.',                                                                            1, 'NL026789012B01', '12345696'),
(1, 'Levertijd Onbekend B.V.',                                                                      1, 'NL027890123B01', '12345697'),
(1, 'Volgende Week Zeker Groothandel B.V.',                                                         1, 'NL028901234B01', '12345698'),
(1, 'Dat Hadden We Niet Meer B.V.',                                                                 1, 'NL029012345B01', '12345699'),
(1, 'Misschien Volgende Maand Import B.V.',                                                         1, 'NL030123456B01', '12345700'),
(1, 'Wacht Even B.V.',                                                                              1, 'NL031234567B01', '12345701'),
(1, 'Schnell & Günstig Stahl GmbH',                                                                 1, 'DE987654321',    'HRB54321'),
-- Zowel klant als leverancier
(1, 'Heen En Weer Handel B.V.',                                                                     1, 'NL032345678B01', '12345702'),
(1, 'Koop En Verkoop Zo B.V.',                                                                      1, 'NL033456789B01', '12345703'),
(1, 'Wij Doen Alles B.V.',                                                                          1, 'NL034567890B01', '12345704'),
(1, 'Van Alles Wat Metaal B.V.',                                                                    1, 'NL035678901B01', '12345705'),
(1, 'Twee Petten Op B.V.',                                                                          1, 'NL036789012B01', '12345706'),
-- Inactief
(1, 'Failliet & Weg B.V.',                                                                          0, 'NL038901234B01', '12345708'),
GO

-- ============================================================
-- ADRESSEN
-- ============================================================

-- Nederlandse organisaties - bulk adressen
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 1, 'Industriepark', CAST(ROW_NUMBER() OVER (ORDER BY p.name) AS NVARCHAR), '5000 AA', 'Tilburg', 'NL', 1
FROM mdata.parties p WHERE p.party_type_id = 1;

-- Bout & Moer - apart afleveradres (andere straat)
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, house_number_addition, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Industrieweg', '45', 'A', '5047 TK', 'Tilburg', 'NL', 1
FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';

-- Vlak & Glad - huisnummer met toevoeging + t.a.v.
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, house_number_addition, postal_code, city, country_code, attention, is_default)
SELECT p.id, 2, 'Slijpsteenstraat', '99', '3-bis', '1234 ZZ', 'Amsterdam', 'NL', 'Jan de Vries', 1
FROM mdata.parties p WHERE p.name = 'Vlak & Glad Metaalbewerking B.V.';

-- Niemand & Zonen - drie afleveradressen
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Nergenslaan', '1', '9700 AA', 'Groningen', 'NL', 1
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Ergensdreef', '2', '5600 BB', 'Eindhoven', 'NL', 0
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Verderoplaan', '3', '2500 CC', 'Den Haag', 'NL', 0
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

-- Duits bedrijf
UPDATE mdata.party_addresses SET street = 'Stahlstraße', house_number = '17', postal_code = '40001', city = 'Düsseldorf', country_code = 'DE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'Müller Stahlbau GmbH';

UPDATE mdata.party_addresses SET street = 'Schnellweg', house_number = '42', postal_code = '50001', city = 'Köln', country_code = 'DE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'Schnell & Günstig Stahl GmbH';

-- Belgisch bedrijf
UPDATE mdata.party_addresses SET street = 'Metaalstraat', house_number = '8', postal_code = '2000', city = 'Antwerpen', country_code = 'BE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'De Smedt Metaalwerken NV';

-- Personen adressen
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 1, 'Gewonestraat', CAST(ROW_NUMBER() OVER (ORDER BY p.name) AS NVARCHAR), '1234 AB', 'Amsterdam', 'NL', 1
FROM mdata.parties p WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';
GO

-- ============================================================
-- CONTACTGEGEVENS
-- ============================================================

INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 1, '013' + RIGHT('0000000' + CAST(ABS(CHECKSUM(p.name)) % 9000000 + 1000000 AS NVARCHAR), 7), 1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;

INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 2, LOWER(REPLACE(REPLACE(LEFT(p.name, 30), ' ', ''), '.', '')) + '@testbedrijf.nl', 1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;

INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 3, '06' + RIGHT('00000000' + CAST(ABS(CHECKSUM(p.name)) % 90000000 + 10000000 AS NVARCHAR), 8), 1
FROM mdata.parties p WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';

INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 2, LOWER(REPLACE(LEFT(p.name, 30), ' ', '.')) + '@testpersoon.nl', 1
FROM mdata.parties p WHERE p.party_type_id = 2 AND LTRIM(RTRIM(p.name)) != '';
GO

-- ============================================================
-- BANKREKENINGEN
-- ============================================================

INSERT INTO mdata.party_bank_accounts (party_id, iban, bic, account_holder, is_primary)
SELECT
    p.id,
    'NL' + RIGHT('00' + CAST(ABS(CHECKSUM(p.name)) % 100 AS NVARCHAR), 2)
        + 'TEST' + RIGHT('0000000000' + CAST(ABS(CHECKSUM(REVERSE(p.name))) % 10000000000 AS NVARCHAR), 10),
    'TESTNL2A',
    p.name,
    1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;

-- Bout & Moer heeft twee bankrekeningen
INSERT INTO mdata.party_bank_accounts (party_id, iban, bic, account_holder, is_primary)
SELECT p.id, 'NL99TEST9999999999', NULL, 'Bout & Moer Holding B.V. - Spaarrekening', 0
FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';
GO

-- ============================================================
-- CUSTOMER ROLES
-- ============================================================

INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00
FROM mdata.parties p
WHERE p.name IN (
    'Bout & Moer Holding B.V.', 'De Roestvrije Droom B.V.', 'Vlak & Glad Metaalbewerking B.V.',
    'Staal & Weinig Constructie B.V.', 'Gebroeders Ergens B.V.', 'Niemand & Zonen B.V.',
    'Iemand Anders Techniek B.V.', 'Van Hier Naar Daar Metaal B.V.', 'Zo Gezegd Zo Gesneden B.V.',
    'Bijna Klaar Industrie B.V.', 'Wel En Niet B.V.', 'Haast Klaar Fabricage B.V.',
    'Altijd Wat Machinefabriek B.V.', 'Ooit Eens B.V.', 'Morgen Beter Staalwerken B.V.',
    'Nu Meteen Plaatbewerking B.V.', 'Zo Maar Even B.V.', 'Prima Hoor Techniek B.V.',
    'Dat Komt Wel Goed B.V.', 'Eigenlijk Niet B.V.',
    'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.',
    '3M Staalwerken B.V.', 'Van Dam Metaal B.V.', 'Van Dam Metaal & Staal B.V.',
    'Müller Stahlbau GmbH', 'De Smedt Metaalwerken NV',
    'Nauwkeurig Op De Millimeter B.V.', 'Tolerantie Nul Komma Nul B.V.',
    'Scherp Geprijsd Staal B.V.', 'Precies Goed Zo B.V.',
    'Recht Toe Recht Aan Techniek B.V.', 'Geen Gezeur Gewoon Doen B.V.',
    'Lassen Voor Dummies B.V.', 'Vonken Vliegen Lasbedrijf B.V.',
    'Buigen Maar Niet Breken B.V.', 'Zwaar Werk Lichte Prijzen B.V.'
);

-- Hoge korting
UPDATE mdata.customer_roles SET discount = 25.00
FROM mdata.customer_roles cr JOIN mdata.parties p ON p.id = cr.party_id
WHERE p.name = 'Bout & Moer Holding B.V.';

-- Geen kredietlimiet
UPDATE mdata.customer_roles SET credit_limit = NULL
FROM mdata.customer_roles cr JOIN mdata.parties p ON p.id = cr.party_id
WHERE p.name = 'Eigenlijk Niet B.V.';

-- BTW verlegd buitenlandse klanten
UPDATE mdata.customer_roles SET is_vat_shifted = 1
FROM mdata.customer_roles cr JOIN mdata.parties p ON p.id = cr.party_id
WHERE p.name IN ('Müller Stahlbau GmbH', 'De Smedt Metaalwerken NV');

-- Afwijkende betalingstermijn
UPDATE mdata.customer_roles SET payment_term_days = 60
FROM mdata.customer_roles cr JOIN mdata.parties p ON p.id = cr.party_id
WHERE p.name = 'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.';

-- Beide petten: ook klant
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30
FROM mdata.parties p
WHERE p.name IN (
    'Heen En Weer Handel B.V.', 'Koop En Verkoop Zo B.V.',
    'Wij Doen Alles B.V.', 'Van Alles Wat Metaal B.V.', 'Twee Petten Op B.V.'
);
GO

-- ============================================================
-- SUPPLIER ROLES
-- ============================================================

INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30
FROM mdata.parties p
WHERE p.name IN (
    'Altijd Op Voorraad B.V.', 'Nergens Meer B.V.', 'Levertijd Onbekend B.V.',
    'Volgende Week Zeker Groothandel B.V.', 'Dat Hadden We Niet Meer B.V.',
    'Misschien Volgende Maand Import B.V.', 'Wacht Even B.V.'
);

INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 1, 45
FROM mdata.parties p WHERE p.name = 'Schnell & Günstig Stahl GmbH';

INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30
FROM mdata.parties p
WHERE p.name IN (
    'Heen En Weer Handel B.V.', 'Koop En Verkoop Zo B.V.',
    'Wij Doen Alles B.V.', 'Van Alles Wat Metaal B.V.', 'Twee Petten Op B.V.'
);
GO

-- ============================================================
-- PARTY RELATIONSHIPS - contactpersonen koppelen
-- ============================================================

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Bout & Moer Holding B.V.'              AND per.name = 'Jan de Vries';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'De Roestvrije Droom B.V.'              AND per.name = 'Petra van den Berg';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Müller Stahlbau GmbH'                  AND per.name = 'Günter von der Heide';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Schnell & Günstig Stahl GmbH'          AND per.name = 'Hans-Jürgen Schröder';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'De Smedt Metaalwerken NV'              AND per.name = 'Jean-Pierre De Smedt';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Vlak & Glad Metaalbewerking B.V.'      AND per.name = 'Mohammed El-Masri';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Niemand & Zonen B.V.'                  AND per.name = 'IJsbrand van der Meer';

-- Organisatie met twee contactpersonen (edge case)
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Twee Petten Op B.V.'                   AND per.name = 'Kim Jansen';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Twee Petten Op B.V.'                   AND per.name = 'Robin van Dijk';

-- Contactpersoon bij twee organisaties (edge case)
INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Heen En Weer Handel B.V.'              AND per.name = 'Sofía Martínez';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Koop En Verkoop Zo B.V.'               AND per.name = 'Sofía Martínez';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Altijd Op Voorraad B.V.'               AND per.name = 'Agnieszka Kowalski';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Levertijd Onbekend B.V.'               AND per.name = 'Leïla Benali';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Lassen Voor Dummies B.V.'              AND per.name = 'Seán O''Brien';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Van Alles Wat Metaal B.V.'             AND per.name = 'Jan-Willem van ''t Hof';

INSERT INTO mdata.party_relationships (from_party_id, to_party_id, relationship_type_id)
SELECT org.id, per.id, 1 FROM mdata.parties org, mdata.parties per
WHERE org.name = 'Wij Doen Alles B.V.'                   AND per.name = 'IJda van IJzendoorn';

PRINT 'Seed data (low) succesvol geladen.';
GO

-- ============================================================
-- PERSON DETAILS
-- Voornaam, tussenvoegsel, achternaam en initialen per persoon
-- ============================================================

INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Jan',           'de',          'Vries',            'J.'         FROM mdata.parties p WHERE p.name = 'Jan de Vries';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Petra',         'van den',     'Berg',             'P.'         FROM mdata.parties p WHERE p.name = 'Petra van den Berg';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Mohammed',      NULL,          'El-Masri',         'M.'         FROM mdata.parties p WHERE p.name = 'Mohammed El-Masri';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'IJsbrand',      'van der',     'Meer',             'IJ.'        FROM mdata.parties p WHERE p.name = 'IJsbrand van der Meer';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'François',      NULL,          'Dubois',           'F.'         FROM mdata.parties p WHERE p.name = 'François Dubois';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'René',          NULL,          'Müller',           'R.'         FROM mdata.parties p WHERE p.name = 'René Müller';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Ève',           'de',          'Groot',            'È.'         FROM mdata.parties p WHERE p.name = 'Ève de Groot';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Joël',          'van de',      'Laar',             'J.'         FROM mdata.parties p WHERE p.name = 'Joël van de Laar';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Søren',         NULL,          'Andersen',         'S.'         FROM mdata.parties p WHERE p.name = 'Søren Andersen';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Ángel',         NULL,          'García',           'Á.'         FROM mdata.parties p WHERE p.name = 'Ángel García';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Li',            NULL,          'Wu',               'L.'         FROM mdata.parties p WHERE p.name = 'Li Wu';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Bartholomeus',  NULL,          'Winterbottom',     'B.'         FROM mdata.parties p WHERE p.name = 'Bartholomeus Winterbottom';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Maria-José',    NULL,          'Hernández',        'M.J.'       FROM mdata.parties p WHERE p.name = 'Maria-José Hernández';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Seán',          NULL,          'O''Brien',         'S.'         FROM mdata.parties p WHERE p.name = 'Seán O''Brien';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'D''Artagnan',   'de',          'Boer',             'D.''A.'     FROM mdata.parties p WHERE p.name = 'D''Artagnan de Boer';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Kim',           NULL,          'Jansen',           'K.'         FROM mdata.parties p WHERE p.name = 'Kim Jansen';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Robin',         'van',         'Dijk',             'R.'         FROM mdata.parties p WHERE p.name = 'Robin van Dijk';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Roos',          NULL,          'Bakker',           'R.'         FROM mdata.parties p WHERE p.name = 'Roos Bakker';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'McDonald',      'de',          'Wit',              'M.'         FROM mdata.parties p WHERE p.name = 'McDonald de Wit';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Anna-Lien',     NULL,          'Vermeersch',       'A.L.'       FROM mdata.parties p WHERE p.name = 'Anna-Lien Vermeersch';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Fatima',        NULL,          'Çelik',            'F.'         FROM mdata.parties p WHERE p.name = 'Fatima Çelik';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Mehmet',        NULL,          'Yılmaz',           'M.'         FROM mdata.parties p WHERE p.name = 'Mehmet Yılmaz';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Agnieszka',     NULL,          'Kowalski',         'A.'         FROM mdata.parties p WHERE p.name = 'Agnieszka Kowalski';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Piotr',         NULL,          'Wróblewski',       'P.'         FROM mdata.parties p WHERE p.name = 'Piotr Wróblewski';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Günter',        'von der',     'Heide',            'G.'         FROM mdata.parties p WHERE p.name = 'Günter von der Heide';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Sigríður',      NULL,          'Björnsdóttir',     'S.'         FROM mdata.parties p WHERE p.name = 'Sigríður Björnsdóttir';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Jean-Pierre',   'De',          'Smedt',            'J.P.'       FROM mdata.parties p WHERE p.name = 'Jean-Pierre De Smedt';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Nguyễn',        'Thị',         'Hương',            'N.T.'       FROM mdata.parties p WHERE p.name = 'Nguyễn Thị Hương';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Weronika',      NULL,          'Żółtowska',        'W.'         FROM mdata.parties p WHERE p.name = 'Weronika Żółtowska';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Sofía',         NULL,          'Martínez',         'S.'         FROM mdata.parties p WHERE p.name = 'Sofía Martínez';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Hans-Jürgen',   NULL,          'Schröder',         'H.J.'       FROM mdata.parties p WHERE p.name = 'Hans-Jürgen Schröder';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Björk',         NULL,          'Guðmundsdóttir',   'B.'         FROM mdata.parties p WHERE p.name = 'Björk Guðmundsdóttir';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Alžběta',       NULL,          'Nováková',         'A.'         FROM mdata.parties p WHERE p.name = 'Alžběta Nováková';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Miloš',         NULL,          'Formanović',       'M.'         FROM mdata.parties p WHERE p.name = 'Miloš Formanović';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Leïla',         NULL,          'Benali',           'L.'         FROM mdata.parties p WHERE p.name = 'Leïla Benali';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Jürgen',        'van der',     'Straaten',         'J.'         FROM mdata.parties p WHERE p.name = 'Jürgen van der Straaten';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Céline',        NULL,          'Dupont-Bernard',   'C.'         FROM mdata.parties p WHERE p.name = 'Céline Dupont-Bernard';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Władysław',     NULL,          'Szczepański',      'W.'         FROM mdata.parties p WHERE p.name = 'Władysław Szczepański';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Ingrid',        NULL,          'Åkesson',          'I.'         FROM mdata.parties p WHERE p.name = 'Ingrid Åkesson';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Pieter-Jan',    'De',          'Cock',             'P.J.'       FROM mdata.parties p WHERE p.name = 'Pieter-Jan De Cock';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Özlem',         NULL,          'Demir',            'Ö.'         FROM mdata.parties p WHERE p.name = 'Özlem Demir';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Şükrü',         NULL,          'Arslan',           'Ş.'         FROM mdata.parties p WHERE p.name = 'Şükrü Arslan';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'María',         'de los',      'Ángeles Ruiz',     'M.'         FROM mdata.parties p WHERE p.name = 'María de los Ángeles Ruiz';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Jan-Willem',    'van ''t',     'Hof',              'J.W.'       FROM mdata.parties p WHERE p.name = 'Jan-Willem van ''t Hof';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'IJda',          'van',         'IJzendoorn',       'IJ.'        FROM mdata.parties p WHERE p.name = 'IJda van IJzendoorn';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Zoë',           'van der',     'Hoeven',           'Z.'         FROM mdata.parties p WHERE p.name = 'Zoë van der Hoeven';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Hervé-Louis',   'd''',         'Entremont',        'H.L.'       FROM mdata.parties p WHERE p.name = 'Hervé-Louis d''Entremont';
INSERT INTO mdata.person_details (party_id, first_name, middle_name, last_name, initials)
SELECT p.id, 'Anästasia',     NULL,          'Bäuerle',          'A.'         FROM mdata.parties p WHERE p.name = 'Anästasia Bäuerle';
-- Edge cases: alleen initiaal en lege naam krijgen geen person_details
GO

-- ============================================================
-- ORGANIZATION DETAILS
-- BTW-nummer en KVK per organisatie
-- ============================================================

INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL001234567B01', '12345671' FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL002345678B01', '12345672' FROM mdata.parties p WHERE p.name = 'De Roestvrije Droom B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL003456789B01', '12345673' FROM mdata.parties p WHERE p.name = 'Vlak & Glad Metaalbewerking B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL004567890B01', '12345674' FROM mdata.parties p WHERE p.name = 'Staal & Weinig Constructie B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL005678901B01', '12345675' FROM mdata.parties p WHERE p.name = 'Gebroeders Ergens B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL006789012B01', '12345676' FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL007890123B01', '12345677' FROM mdata.parties p WHERE p.name = 'Iemand Anders Techniek B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL008901234B01', '12345678' FROM mdata.parties p WHERE p.name = 'Van Hier Naar Daar Metaal B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL009012345B01', '12345679' FROM mdata.parties p WHERE p.name = 'Zo Gezegd Zo Gesneden B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL010123456B01', '12345680' FROM mdata.parties p WHERE p.name = 'Bijna Klaar Industrie B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL011234567B01', '12345681' FROM mdata.parties p WHERE p.name = 'Wel En Niet B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL012345678B01', '12345682' FROM mdata.parties p WHERE p.name = 'Haast Klaar Fabricage B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL013456789B01', '12345683' FROM mdata.parties p WHERE p.name = 'Altijd Wat Machinefabriek B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL014567890B01', '12345684' FROM mdata.parties p WHERE p.name = 'Ooit Eens B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL015678901B01', '12345685' FROM mdata.parties p WHERE p.name = 'Morgen Beter Staalwerken B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL016789012B01', '12345686' FROM mdata.parties p WHERE p.name = 'Nu Meteen Plaatbewerking B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL017890123B01', '12345687' FROM mdata.parties p WHERE p.name = 'Zo Maar Even B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL018901234B01', '12345688' FROM mdata.parties p WHERE p.name = 'Prima Hoor Techniek B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL019012345B01', '12345689' FROM mdata.parties p WHERE p.name = 'Dat Komt Wel Goed B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL020123456B01', '12345690' FROM mdata.parties p WHERE p.name = 'Eigenlijk Niet B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL021234567B01', '12345691' FROM mdata.parties p WHERE p.name = 'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL022345678B01', '12345692' FROM mdata.parties p WHERE p.name = '3M Staalwerken B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL023456789B01', '12345693' FROM mdata.parties p WHERE p.name = 'Van Dam Metaal B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL024567890B01', '12345694' FROM mdata.parties p WHERE p.name = 'Van Dam Metaal & Staal B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'DE123456789', 'HRB12345' FROM mdata.parties p WHERE p.name = 'Müller Stahlbau GmbH';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'BE0123456789', '0123456789' FROM mdata.parties p WHERE p.name = 'De Smedt Metaalwerken NV';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL039012345B01', '12345710' FROM mdata.parties p WHERE p.name = 'Nauwkeurig Op De Millimeter B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL040123456B01', '12345711' FROM mdata.parties p WHERE p.name = 'Tolerantie Nul Komma Nul B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL041234567B01', '12345712' FROM mdata.parties p WHERE p.name = 'Scherp Geprijsd Staal B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL042345678B01', '12345713' FROM mdata.parties p WHERE p.name = 'Precies Goed Zo B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL043456789B01', '12345714' FROM mdata.parties p WHERE p.name = 'Recht Toe Recht Aan Techniek B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL044567890B01', '12345715' FROM mdata.parties p WHERE p.name = 'Geen Gezeur Gewoon Doen B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL045678901B01', '12345716' FROM mdata.parties p WHERE p.name = 'Lassen Voor Dummies B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL046789012B01', '12345717' FROM mdata.parties p WHERE p.name = 'Vonken Vliegen Lasbedrijf B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL047890123B01', '12345718' FROM mdata.parties p WHERE p.name = 'Buigen Maar Niet Breken B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL048901234B01', '12345719' FROM mdata.parties p WHERE p.name = 'Zwaar Werk Lichte Prijzen B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL025678901B01', '12345695' FROM mdata.parties p WHERE p.name = 'Altijd Op Voorraad B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL026789012B01', '12345696' FROM mdata.parties p WHERE p.name = 'Nergens Meer B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL027890123B01', '12345697' FROM mdata.parties p WHERE p.name = 'Levertijd Onbekend B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL028901234B01', '12345698' FROM mdata.parties p WHERE p.name = 'Volgende Week Zeker Groothandel B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL029012345B01', '12345699' FROM mdata.parties p WHERE p.name = 'Dat Hadden We Niet Meer B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL030123456B01', '12345700' FROM mdata.parties p WHERE p.name = 'Misschien Volgende Maand Import B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL031234567B01', '12345701' FROM mdata.parties p WHERE p.name = 'Wacht Even B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'DE987654321', 'HRB54321' FROM mdata.parties p WHERE p.name = 'Schnell & Günstig Stahl GmbH';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL032345678B01', '12345702' FROM mdata.parties p WHERE p.name = 'Heen En Weer Handel B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL033456789B01', '12345703' FROM mdata.parties p WHERE p.name = 'Koop En Verkoop Zo B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL034567890B01', '12345704' FROM mdata.parties p WHERE p.name = 'Wij Doen Alles B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL035678901B01', '12345705' FROM mdata.parties p WHERE p.name = 'Van Alles Wat Metaal B.V.';
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, 'NL036789012B01', '12345706' FROM mdata.parties p WHERE p.name = 'Twee Petten Op B.V.';
-- Inactieve organisatie zonder BTW-nummer (edge case)
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, NULL, '12345708' FROM mdata.parties p WHERE p.name = 'Failliet & Weg B.V.';
GO
