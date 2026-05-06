Write-Host "Generating dynamic SQL seed file (optimised)..."

# IMPORTANT: inside the container, /seed is the mounted folder
$seedPath = "/seed/002-seed.sql"

# Ensure output folder exists
$folder = Split-Path $seedPath
if (!(Test-Path $folder)) {
    New-Item -ItemType Directory -Path $folder | Out-Null
}

# Escape SQL safely
function Escape-SqlLiteral {
    param([string]$value)
    if ($null -eq $value) { return $null }
    return $value -replace '''', ''''''
}

# ============================
# FAST RANDOM GENERATORS
# ============================

function Get-RandomSchoolName {
    $prefixes = @(
        "St. Mary's","St. John's","St. Peter's","St. Paul's","St. Anne's","St. Andrew's","St. George's",
        "St. David's","St. Catherine's","St. Joseph's","St. Mark's","St. Luke's","St. Thomas'",
        "St. Michael's","St. Francis","St. Augustine's","St. Benedict's","St. Christopher's",

        "Oakwood","Cedar Ridge","Willowbank","Maple Grove","Pinecrest","Birchwood","Hawthorn",
        "Chestnut Hill","Elm Valley","Riverbank","Lakeside","Hillcrest","Greenfield","Meadowbrook",
        "Springfield","Bluebell","Foxhill","Brackenwood","Fernwood","Hazelwood","Wheatfield",

        "Kings Heath","Queens Park","Castle Hill","Stonebridge","Millbrook","Forest Edge",
        "Harbour View","Valley View","Broadwater","Windermere","Thornhill","Ridgeway",
        "Longmeadow","Sunnydale","Northgate","Southview","Eastbrook","Westfield","Brookside",

        "Unity","Horizon","Aspire","Endeavour","Inspire","Pioneer","Discovery","Innovation",
        "Momentum","Progress","Venture","Summit","Beacon","Pathway","Frontier","Evolution",

        "STEM Academy","Performing Arts","Sports Leadership","Technology Institute","Digital Futures",
        "Creative Arts","Engineering Centre","Science Hub","Sports Academy","Music Conservatory"
    )

    $suffixes = @(
        "Primary School","Academy","Community School","Preparatory School","Junior School",
        "Infant School","College","School","High School","Grammar School","Comprehensive School",
        "All-Through School","Specialist School","STEM Academy","Performing Arts School",
        "Sports College","Sixth Form","Church of England School","Roman Catholic School",
        "Independent School","Free School","Community College","Learning Centre","Education Campus"
    )

    return "$($prefixes[(Get-Random -Min 0 -Max $prefixes.Count)]) $($suffixes[(Get-Random -Min 0 -Max $suffixes.Count)])"
}

$streets = @(
    "High Street","Station Road","Church Lane","Victoria Road","Park Avenue","London Road",
    "Main Street","Mill Lane","The Crescent","The Green","School Road","New Road",
    "Bridge Street","Kingsway","Queens Road","Market Street","Grove Road","Manor Road"
)

$towns = @(
    "Birmingham","Manchester","Leeds","Sheffield","Bristol","Liverpool","Nottingham",
    "Leicester","Portsmouth","Derby","Reading","Plymouth","Norwich","Oxford","Cambridge",
    "York","Exeter","Swansea","Cardiff","Newcastle","Sunderland","Coventry","Hull"
)

$postcodeAreas = @(
    "AB","AL","B","BA","BB","BD","BH","BL","BN","BR","BS","BT","CA","CB","CF","CM","CO","CR",
    "CT","CV","CW","DA","DD","DE","DG","DH","DL","DN","DT","DY","E","EC","EH","EN","EX","FK",
    "FY","G","GL","GU","HA","HD","HG","HP","HR","HS","HU","HX","IG","IP","IV","KA","KT","KW",
    "KY","L","LA","LD","LE","LL","LN","LS","LU","M","ME","MK","ML","N","NE","NG","NN","NP",
    "NR","NW","OL","OX","PA","PE","PH","PL","PO","PR","RG","RH","RM","S","SA","SE","SG","SK",
    "SL","SM","SN","SO","SP","SR","SS","ST","SW","SY","TA","TD","TF","TN","TQ","TR","TS","TW",
    "UB","W","WA","WC","WD","WF","WN","WR","WS","WV","YO","ZE"
)

function Get-RandomPostcode {
    $area = $postcodeAreas[(Get-Random -Min 0 -Max $postcodeAreas.Count)]
    $district = Get-Random -Min 1 -Max 10
    $sector = Get-Random -Min 1 -Max 10
    $unit = -join ((65..90 | Get-Random -Count 2) | ForEach-Object {[char]$_})
    return "$area$district $sector$unit"
}

function Get-RandomTelephone {
    switch (Get-Random -Min 1 -Max 5) {
        1 { return "01{0:D3}{1:D6}" -f (Get-Random -Min 100 -Max 999), (Get-Random -Min 100000 -Max 999999) }
        2 { return "02{0:D2}{1:D4}{2:D4}" -f (Get-Random -Min 10 -Max 99), (Get-Random -Min 1000 -Max 9999), (Get-Random -Min 1000 -Max 9999) }
        3 { return "07{0:D3}{1:D6}" -f (Get-Random -Min 100 -Max 999), (Get-Random -Min 100000 -Max 999999) }
        4 { return "44{0:D9}" -f (Get-Random -Min 100000000 -Max 999999999) }
    }
}

function Get-RandomWebsite($name) {
    $slug = $name.ToLower() -replace '[^a-z0-9]+','-'
    return "https://$slug.sch.uk"
}

# ============================
# WRITE DIRECTLY TO FILE
# ============================

$writer = [System.IO.StreamWriter]::new($seedPath, $false, [System.Text.Encoding]::UTF8)

$writer.WriteLine("-- AUTO-GENERATED SEED FILE")
$writer.WriteLine("-- Generated: $(Get-Date -Format o)")
$writer.WriteLine("")

$writer.WriteLine("CREATE EXTENSION IF NOT EXISTS pg_trgm;")
$writer.WriteLine("")

$writer.WriteLine("INSERT INTO EstablishmentType (name) VALUES
('Academy'),
('Community school'),
('Voluntary aided school'),
('British schools overseas')
ON CONFLICT DO NOTHING;")

$writer.WriteLine("INSERT INTO EducationPhase (name) VALUES
('Primary'),
('Secondary'),
('Special')
ON CONFLICT DO NOTHING;")

$writer.WriteLine("INSERT INTO EstablishmentStatus (name) VALUES
('Closed'),
('Open')
ON CONFLICT DO NOTHING;")

$writer.WriteLine("INSERT INTO EstablishmentGroupType (code, name) VALUES
('MAT', 'Multi-Academy Trust'),
('FED', 'Federation'),
('INT', 'International Group')
ON CONFLICT DO NOTHING;")

# ============================
# ESTABLISHMENTS
# ============================

$establishmentCount = 100000

$writer.WriteLine("INSERT INTO Establishment (URN, EstablishmentName, EstablishmentTypeId, EducationPhaseId, SchoolWebsite, TelephoneNum, Street, Town, Postcode, EstablishmentStatusId) VALUES")

for ($i = 1; $i -le $establishmentCount; $i++) {

    if ($i % 1000 -eq 0) {
        Write-Host "Generated $i establishments..."
    }

    $urn = 100000 + $i
    $name = Get-RandomSchoolName
    $escapedName = Escape-SqlLiteral $name

    $street = Escape-SqlLiteral ($streets[(Get-Random -Min 0 -Max $streets.Count)])
    $town = Escape-SqlLiteral ($towns[(Get-Random -Min 0 -Max $towns.Count)])
    $postcode = Escape-SqlLiteral (Get-RandomPostcode)
    $website = Escape-SqlLiteral (Get-RandomWebsite $name)
    $telephone = Escape-SqlLiteral (Get-RandomTelephone)

    $typeId = Get-Random -Min 1 -Max 5
    $phaseId = Get-Random -Min 1 -Max 4
    $statusId = if ((Get-Random -Min 1 -Max 100) -le 80) { 2 } else { 1 }

    $line = "($urn, '$escapedName', $typeId, $phaseId, '$website', '$telephone', '$street', '$town', '$postcode', $statusId)"

    if ($i -lt $establishmentCount) {
        $writer.WriteLine("$line,")
    } else {
        $writer.WriteLine("$line;")
    }
}

# ============================
# GROUPS
# ============================

$groupCount = 100000

$writer.WriteLine("INSERT INTO EstablishmentGroup (id, name, type_code) VALUES")

for ($i = 1; $i -le $groupCount; $i++) {

    if ($i % 1000 -eq 0) {
        Write-Host "Generated $i groups..."
    }

    $gid = 10000 + $i
    $gname = "Group $i"

    $type = switch ($i % 3) {
        0 { "MAT" }
        1 { "FED" }
        2 { "INT" }
    }

    $line = "($gid, '$gname', '$type')"

    if ($i -lt $groupCount) {
        $writer.WriteLine("$line,")
    } else {
        $writer.WriteLine("$line;")
    }
}

# ============================
# GROUP LINKS
# ============================

$writer.WriteLine("INSERT INTO GroupLink (group_id, urn) VALUES")

for ($i = 1; $i -le $groupCount; $i++) {

    if ($i % 1000 -eq 0) {
        Write-Host "Generated $i group links..."
    }

    $gid = 10000 + $i
    $urn = 100000 + $i

    $line = "($gid, $urn)"

    if ($i -lt $groupCount) {
        $writer.WriteLine("$line,")
    } else {
        $writer.WriteLine("$line;")
    }
}

# ============================
# INDEXES
# ============================

$writer.WriteLine("
CREATE INDEX IF NOT EXISTS idx_establishment_urn
    ON Establishment (URN);

CREATE INDEX IF NOT EXISTS idx_establishment_name_trgm
    ON Establishment USING gin (EstablishmentName gin_trgm_ops);

CREATE INDEX IF NOT EXISTS idx_establishment_town_trgm
    ON Establishment USING gin (Town gin_trgm_ops);
")

$writer.Close()

Write-Host "Seed file generated at $seedPath"
