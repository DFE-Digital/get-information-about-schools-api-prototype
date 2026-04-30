Write-Host "Generating dynamic SQL seed file..."

# IMPORTANT: inside the container, /seed is the mounted folder
$seedPath = "/seed/002-seed.sql"

# Ensure output folder exists
$folder = Split-Path $seedPath
if (!(Test-Path $folder)) {
    New-Item -ItemType Directory -Path $folder | Out-Null
}

# Escape SQL string literals safely
function Escape-SqlLiteral {
    param([string]$value)
    if ($null -eq $value) { return $null }
    return $value -replace '''', ''''''
}

$sql = @()
$sql += "-- AUTO-GENERATED SEED FILE"
$sql += "-- Generated: $(Get-Date -Format o)"
$sql += ""

# ============================
# Helper functions for realism
# ============================

function Get-RandomSchoolName {
    $prefixes = @(
        "St. Mary's", "St. John's", "Riverside", "Oakwood", "Hillcrest",
        "Greenfield", "Kingswood", "Elm Grove", "Highfield", "Maple Ridge",
        "Brookside", "Westfield", "Northgate", "Southview", "Eastbrook"
    )

    $suffixes = @(
        "Primary School", "Academy", "Community School", "Preparatory School",
        "Junior School", "Infant School", "College", "School"
    )

    return "$($prefixes | Get-Random) $($suffixes | Get-Random)"
}

function Get-RandomStreet {
    $names = @(
        "High Street", "Station Road", "Church Lane", "Victoria Road",
        "Park Avenue", "London Road", "Main Street", "Mill Lane",
        "The Crescent", "The Green", "School Road", "New Road"
    )
    return $names | Get-Random
}

function Get-RandomTown {
    $towns = @(
        "Birmingham", "Manchester", "Leeds", "Sheffield", "Bristol",
        "Liverpool", "Nottingham", "Leicester", "Portsmouth", "Derby",
        "Reading", "Plymouth", "Norwich", "Oxford", "Cambridge"
    )
    return $towns | Get-Random
}

function Get-RandomPostcode {
    $areas = @(
        "AB","AL","B","BA","BB","BD","BH","BL","BN","BR","BS","BT","CA","CB","CF","CM","CO","CR","CT","CV","CW","DA","DD","DE","DG","DH","DL","DN","DT","DY","E","EC","EH","EN","EX","FK","FY","G","GL","GU","HA","HD","HG","HP","HR","HS","HU","HX","IG","IP","IV","KA","KT","KW","KY","L","LA","LD","LE","LL","LN","LS","LU","M","ME","MK","ML","N","NE","NG","NN","NP","NR","NW","OL","OX","PA","PE","PH","PL","PO","PR","RG","RH","RM","S","SA","SE","SG","SK","SL","SM","SN","SO","SP","SR","SS","ST","SW","SY","TA","TD","TF","TN","TQ","TR","TS","TW","UB","W","WA","WC","WD","WF","WN","WR","WS","WV","YO","ZE"
    )

    $area = $areas | Get-Random
    $district = Get-Random -InputObject @(1..9)
    $sector = Get-Random -InputObject @(1..9)
    $unit = -join ((65..90 | Get-Random -Count 2) | ForEach-Object {[char]$_})

    return "$area$district $sector$unit"
}

# ============================
# Extensions
# ============================
$sql += @"
-- Enable pg_trgm extension for fuzzy search
CREATE EXTENSION IF NOT EXISTS pg_trgm;
"@

# ============================
# Lookup tables
# ============================
$sql += @"
-- Lookup table seed data

INSERT INTO EstablishmentType (name) VALUES
('Academy'),
('Community school'),
('Voluntary aided school'),
('British schools overseas')
ON CONFLICT DO NOTHING;

INSERT INTO EducationPhase (name) VALUES
('Primary'),
('Secondary'),
('Special')
ON CONFLICT DO NOTHING;

INSERT INTO EstablishmentStatus (name) VALUES
('Closed'),
('Open')
ON CONFLICT DO NOTHING;
"@

# ============================
# Establishment Group Types
# ============================
$sql += @"
INSERT INTO EstablishmentGroupType (code, name) VALUES
('MAT', 'Multi-Academy Trust'),
('FED', 'Federation'),
('INT', 'International Group')
ON CONFLICT DO NOTHING;
"@

# ============================
# Establishments
# ============================
$establishmentCount = 10000

$sql += "-- Establishment seed data"
$sql += "INSERT INTO Establishment (URN, EstablishmentName, EstablishmentTypeId, EducationPhaseId, SchoolWebsite, TelephoneNum, Street, Town, Postcode, EstablishmentStatusId) VALUES"

$estRows = @()

for ($i = 1; $i -le $establishmentCount; $i++) {

    $urn = 100000 + $i  # Always 6 digits

    $name = Escape-SqlLiteral (Get-RandomSchoolName)
    $street = Escape-SqlLiteral (Get-RandomStreet)
    $town = Escape-SqlLiteral (Get-RandomTown)
    $postcode = Escape-SqlLiteral (Get-RandomPostcode)

    # Random type (1–4)
    $typeId = Get-Random -Minimum 1 -Maximum 5

    # Random phase (1–3)
    $phaseId = Get-Random -Minimum 1 -Maximum 4

    # Weighted status: 80% Open (2), 20% Closed (1)
    $statusId = if ((Get-Random -Minimum 1 -Maximum 100) -le 80) { 2 } else { 1 }

    $estRows += "($urn, '$name', $typeId, $phaseId, NULL, NULL, '$street', '$town', '$postcode', $statusId)"
}

$sql += ($estRows -join ",`n") + ";"

# ============================
# Establishment Groups
# ============================
$groupCount = 10000

$sql += "-- Establishment Group seed data"
$sql += "INSERT INTO EstablishmentGroup (id, name, type_code) VALUES"

$groupRows = @()

for ($i = 1; $i -le $groupCount; $i++) {
    $gid = 10000 + $i
    $gname = Escape-SqlLiteral "Group $i"

    $type = switch ($i % 3) {
        0 { "MAT" }
        1 { "FED" }
        2 { "INT" }
    }

    $groupRows += "($gid, '$gname', '$type')"
}

$sql += ($groupRows -join ",`n") + ";"

# ============================
# GroupLink
# ============================
$sql += "-- GroupLink seed data"
$sql += "INSERT INTO GroupLink (group_id, urn) VALUES"

$linkRows = @()

for ($i = 1; $i -le $groupCount; $i++) {
    $gid = 10000 + $i
    $urn = 100000 + $i
    $linkRows += "($gid, $urn)"
}

$sql += ($linkRows -join ",`n") + ";"

# ============================
# Indexes
# ============================
$sql += @"
-- Indexes required for search

CREATE INDEX IF NOT EXISTS idx_establishment_urn
    ON Establishment (URN);

CREATE INDEX IF NOT EXISTS idx_establishment_name_trgm
    ON Establishment USING gin (EstablishmentName gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_establishment_town_trgm
    ON Establishment USING gin (Town gin_trgm_ops);
"@

# ============================
# Write file
# ============================
$sql -join "`n" | Set-Content -Path $seedPath -Encoding UTF8

Write-Host "Seed file generated at $seedPath"
