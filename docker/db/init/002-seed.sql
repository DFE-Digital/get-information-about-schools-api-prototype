-- ============================
-- Lookup table seed data
-- ============================

INSERT INTO EstablishmentType (name) VALUES
('Voluntary aided school'),
('British schools overseas')
ON CONFLICT DO NOTHING;

INSERT INTO EducationPhase (name) VALUES
('Primary'),
('Not applicable')
ON CONFLICT DO NOTHING;

INSERT INTO EstablishmentStatus (name) VALUES
('Closed'),
('Open')
ON CONFLICT DO NOTHING;

-- ============================
-- Establishment seed data
-- ============================

INSERT INTO Establishment (
    URN,
    EstablishmentName,
    EstablishmentTypeId,
    EducationPhaseId,
    SchoolWebsite,
    TelephoneNum,
    Street,
    Town,
    Postcode,
    EstablishmentStatusId
)
VALUES
(
    100000,
    'Sir John Cass''s Foundation Primary Schools',
    (SELECT id FROM EstablishmentType WHERE name = 'Voluntary aided school'),
    (SELECT id FROM EducationPhase WHERE name = 'Primary'),
    NULL,
    NULL,
    'obfuscated',
    'obfuscated',
    'EC3A 5DE',
    (SELECT id FROM EstablishmentStatus WHERE name = 'Closed')
),
(
    130892,
    'Dubai British School',
    (SELECT id FROM EstablishmentType WHERE name = 'British schools overseas'),
    (SELECT id FROM EducationPhase WHERE name = 'Not applicable'),
    NULL,
    NULL,
    'obfuscated',
    'obfuscated',
    NULL,
    (SELECT id FROM EstablishmentStatus WHERE name = 'Open')
),
(
    132270,
    'British School In the Netherlands',
    (SELECT id FROM EstablishmentType WHERE name = 'British schools overseas'),
    (SELECT id FROM EducationPhase WHERE name = 'Not applicable'),
    NULL,
    NULL,
    'obfuscated',
    'obfuscated',
    NULL,
    (SELECT id FROM EstablishmentStatus WHERE name = 'Open')
);

-- ============================
-- Establishment Group Type seed data
-- ============================

INSERT INTO EstablishmentGroupType (code, name) VALUES
('MAT', 'Multi-Academy Trust'),
('FED', 'Federation'),
('INT', 'International Group')
ON CONFLICT DO NOTHING;

-- ============================
-- Establishment Group seed data
-- ============================

INSERT INTO EstablishmentGroup (id, name, type_code) VALUES
(10001, 'Cass Foundation Trust', 'MAT'),
(20002, 'Dubai Education Partnership', 'INT'),
(30003, 'Netherlands British Schools Group', 'INT')
ON CONFLICT DO NOTHING;

-- ============================
-- GroupLink seed data
-- ============================

INSERT INTO GroupLink (group_id, urn) VALUES
(10001, 100000),   -- Cass Foundation Trust - Sir John Cass Primary
(20002, 130892),   -- Dubai Education Partnership - Dubai British School
(30003, 132270)    -- Netherlands British Schools Group - British School in the Netherlands
ON CONFLICT DO NOTHING;
