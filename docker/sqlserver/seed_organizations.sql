USE ErpDb;
GO

INSERT INTO mdata.parties (party_type_id, name, is_active) VALUES
(1, 'Bout & Moer Holding B.V.', 1),
(1, 'De Roestvrije Droom B.V.', 1),
(1, 'Vlak & Glad Metaalbewerking B.V.', 1),
(1, 'Staal & Weinig Constructie B.V.', 1),
(1, 'Gebroeders Ergens B.V.', 1),
(1, 'Niemand & Zonen B.V.', 1),
(1, 'Iemand Anders Techniek B.V.', 1),
(1, 'Van Hier Naar Daar Metaal B.V.', 1),
(1, 'Zo Gezegd Zo Gesneden B.V.', 1),
(1, 'Bijna Klaar Industrie B.V.', 1),
(1, 'Wel En Niet B.V.', 1),
(1, 'Haast Klaar Fabricage B.V.', 1),
(1, 'Altijd Wat Machinefabriek B.V.', 1),
(1, 'Ooit Eens B.V.', 1),
(1, 'Morgen Beter Staalwerken B.V.', 1),
(1, 'Nu Meteen Plaatbewerking B.V.', 1),
(1, 'Zo Maar Even B.V.', 1),
(1, 'Prima Hoor Techniek B.V.', 1),
(1, 'Dat Komt Wel Goed B.V.', 1),
(1, 'Eigenlijk Niet B.V.', 1),
(1, 'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.', 1),
(1, '3M Staalwerken B.V.', 1),
(1, 'Van Dam Metaal B.V.', 1),
(1, 'Van Dam Metaal & Staal B.V.', 1),
(1, 'Mueller Stahlbau GmbH', 1),
(1, 'De Smedt Metaalwerken NV', 1),
(1, 'Nauwkeurig Op De Millimeter B.V.', 1),
(1, 'Tolerantie Nul Komma Nul B.V.', 1),
(1, 'Scherp Geprijsd Staal B.V.', 1),
(1, 'Precies Goed Zo B.V.', 1),
(1, 'Recht Toe Recht Aan Techniek B.V.', 1),
(1, 'Geen Gezeur Gewoon Doen B.V.', 1),
(1, 'Lassen Voor Dummies B.V.', 1),
(1, 'Vonken Vliegen Lasbedrijf B.V.', 1),
(1, 'Buigen Maar Niet Breken B.V.', 1),
(1, 'Zwaar Werk Lichte Prijzen B.V.', 1),
(1, 'Altijd Op Voorraad B.V.', 1),
(1, 'Nergens Meer B.V.', 1),
(1, 'Levertijd Onbekend B.V.', 1),
(1, 'Volgende Week Zeker Groothandel B.V.', 1),
(1, 'Dat Hadden We Niet Meer B.V.', 1),
(1, 'Misschien Volgende Maand Import B.V.', 1),
(1, 'Wacht Even B.V.', 1),
(1, 'Schnell und Gunstig Stahl GmbH', 1),
(1, 'Heen En Weer Handel B.V.', 1),
(1, 'Koop En Verkoop Zo B.V.', 1),
(1, 'Wij Doen Alles B.V.', 1),
(1, 'Van Alles Wat Metaal B.V.', 1),
(1, 'Twee Petten Op B.V.', 1),
(1, 'Failliet en Weg B.V.', 0);
GO

-- Organization details
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
SELECT p.id, 'DE123456789', 'HRB12345' FROM mdata.parties p WHERE p.name = 'Mueller Stahlbau GmbH';
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
SELECT p.id, 'DE987654321', 'HRB54321' FROM mdata.parties p WHERE p.name = 'Schnell und Gunstig Stahl GmbH';
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
INSERT INTO mdata.organization_details (party_id, vat_number, chamber_of_commerce_number)
SELECT p.id, NULL, '12345708' FROM mdata.parties p WHERE p.name = 'Failliet en Weg B.V.';
GO

-- Customer roles
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'De Roestvrije Droom B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Vlak & Glad Metaalbewerking B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Staal & Weinig Constructie B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Gebroeders Ergens B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Iemand Anders Techniek B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Van Hier Naar Daar Metaal B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Zo Gezegd Zo Gesneden B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Bijna Klaar Industrie B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Wel En Niet B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Haast Klaar Fabricage B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Altijd Wat Machinefabriek B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Ooit Eens B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Morgen Beter Staalwerken B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Nu Meteen Plaatbewerking B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Zo Maar Even B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Prima Hoor Techniek B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Dat Komt Wel Goed B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, NULL FROM mdata.parties p WHERE p.name = 'Eigenlijk Niet B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 60, 50000.00 FROM mdata.parties p WHERE p.name = 'Internationale Groothandel in Bijzondere en Minder Bijzondere Metaalproducten B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = '3M Staalwerken B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Van Dam Metaal B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Van Dam Metaal & Staal B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 1, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Mueller Stahlbau GmbH';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 1, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'De Smedt Metaalwerken NV';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Nauwkeurig Op De Millimeter B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Tolerantie Nul Komma Nul B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Scherp Geprijsd Staal B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Precies Goed Zo B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Recht Toe Recht Aan Techniek B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Geen Gezeur Gewoon Doen B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Lassen Voor Dummies B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Vonken Vliegen Lasbedrijf B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Buigen Maar Niet Breken B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days, credit_limit)
SELECT p.id, 0.00, 0, 30, 10000.00 FROM mdata.parties p WHERE p.name = 'Zwaar Werk Lichte Prijzen B.V.';
-- Hoge korting edge case
UPDATE mdata.customer_roles SET discount = 25.00
FROM mdata.customer_roles cr JOIN mdata.parties p ON p.id = cr.party_id
WHERE p.name = 'Bout & Moer Holding B.V.';
-- Beide petten
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30 FROM mdata.parties p WHERE p.name = 'Heen En Weer Handel B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30 FROM mdata.parties p WHERE p.name = 'Koop En Verkoop Zo B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30 FROM mdata.parties p WHERE p.name = 'Wij Doen Alles B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30 FROM mdata.parties p WHERE p.name = 'Van Alles Wat Metaal B.V.';
INSERT INTO mdata.customer_roles (party_id, discount, is_vat_shifted, payment_term_days)
SELECT p.id, 5.00, 0, 30 FROM mdata.parties p WHERE p.name = 'Twee Petten Op B.V.';
GO

-- Supplier roles
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Altijd Op Voorraad B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Nergens Meer B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Levertijd Onbekend B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Volgende Week Zeker Groothandel B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Dat Hadden We Niet Meer B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Misschien Volgende Maand Import B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Wacht Even B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 1, 45 FROM mdata.parties p WHERE p.name = 'Schnell und Gunstig Stahl GmbH';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Heen En Weer Handel B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Koop En Verkoop Zo B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Wij Doen Alles B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Van Alles Wat Metaal B.V.';
INSERT INTO mdata.supplier_roles (party_id, is_vat_shifted, payment_term_days)
SELECT p.id, 0, 30 FROM mdata.parties p WHERE p.name = 'Twee Petten Op B.V.';
GO

-- Adressen
INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 1, 'Industriepark', CAST(ROW_NUMBER() OVER (ORDER BY p.name) AS NVARCHAR), '5000 AA', 'Tilburg', 'NL', 1
FROM mdata.parties p WHERE p.party_type_id = 1;

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, house_number_addition, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Industrieweg', '45', 'A', '5047 TK', 'Tilburg', 'NL', 1
FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Nergenslaan', '1', '9700 AA', 'Groningen', 'NL', 1
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Ergensdreef', '2', '5600 BB', 'Eindhoven', 'NL', 0
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

INSERT INTO mdata.party_addresses (party_id, address_type_id, street, house_number, postal_code, city, country_code, is_default)
SELECT p.id, 2, 'Verderoplaan', '3', '2500 CC', 'Den Haag', 'NL', 0
FROM mdata.parties p WHERE p.name = 'Niemand & Zonen B.V.';

UPDATE mdata.party_addresses SET street = 'Stahlstrasse', house_number = '17', postal_code = '40001', city = 'Dusseldorf', country_code = 'DE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'Mueller Stahlbau GmbH';

UPDATE mdata.party_addresses SET street = 'Schnellweg', house_number = '42', postal_code = '50001', city = 'Koln', country_code = 'DE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'Schnell und Gunstig Stahl GmbH';

UPDATE mdata.party_addresses SET street = 'Metaalstraat', house_number = '8', postal_code = '2000', city = 'Antwerpen', country_code = 'BE'
FROM mdata.party_addresses pa JOIN mdata.parties p ON p.id = pa.party_id
WHERE p.name = 'De Smedt Metaalwerken NV';
GO

-- Contactgegevens
INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 1, '013' + RIGHT('0000000' + CAST(ABS(CHECKSUM(p.name)) % 9000000 + 1000000 AS NVARCHAR), 7), 1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;

INSERT INTO mdata.party_contact_methods (party_id, contact_method_type_id, value, is_primary)
SELECT p.id, 2, LOWER(REPLACE(REPLACE(LEFT(p.name, 30), ' ', ''), '.', '')) + '@testbedrijf.nl', 1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;
GO

-- Bankrekeningen
INSERT INTO mdata.party_bank_accounts (party_id, iban, bic, account_holder, is_primary)
SELECT p.id,
    'NL' + RIGHT('00' + CAST(ABS(CHECKSUM(p.name)) % 100 AS NVARCHAR), 2)
        + 'TEST' + RIGHT('0000000000' + CAST(ABS(CHECKSUM(REVERSE(p.name))) % 10000000000 AS NVARCHAR), 10),
    'TESTNL2A', p.name, 1
FROM mdata.parties p WHERE p.party_type_id = 1 AND p.is_active = 1;

INSERT INTO mdata.party_bank_accounts (party_id, iban, bic, account_holder, is_primary)
SELECT p.id, 'NL99TEST9999999999', NULL, 'Bout & Moer Holding B.V. - Spaarrekening', 0
FROM mdata.parties p WHERE p.name = 'Bout & Moer Holding B.V.';
GO

PRINT 'Organisaties seed klaar.';
GO

-- ============================================================
-- HISTORY EN SNAPSHOTS voor geseedde organisaties
-- ============================================================

INSERT INTO audit.event_log (aggregate_id, aggregate_type, event_type, payload, occurred_at)
SELECT
    p.id,
    'Party',
    'PartyCreated',
    (SELECT p.id AS PartyId, p.name AS Name, p.party_type_id AS PartyType,
        CASE WHEN cr.party_id IS NOT NULL THEN 1 ELSE 0 END AS IsCustomer,
        CASE WHEN sr.party_id IS NOT NULL THEN 1 ELSE 0 END AS IsSupplier,
        SYSUTCDATETIME() AS OccurredAt
     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
    SYSUTCDATETIME()
FROM mdata.parties p
LEFT JOIN mdata.customer_roles cr ON cr.party_id = p.id
LEFT JOIN mdata.supplier_roles sr ON sr.party_id = p.id
WHERE p.party_type_id = 1;
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
WHERE p.party_type_id = 1;
GO

INSERT INTO audit.party_snapshots (party_id, at_event_id, snapshot, trigger_reason)
SELECT
    p.id,
    e.id,
    e.payload,
    'state_closed'
FROM mdata.parties p
JOIN audit.event_log e ON e.aggregate_id = p.id AND e.event_type = 'PartyCreated'
WHERE p.party_type_id = 1;
GO

PRINT 'Organisaties history en snapshots klaar.';
GO
