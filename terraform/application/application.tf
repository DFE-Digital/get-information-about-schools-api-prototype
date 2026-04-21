module "application_configuration" {
  source = "./vendor/modules/aks//aks/application_configuration"

  namespace              = var.namespace
  environment            = var.environment
  azure_resource_prefix  = var.azure_resource_prefix
  service_short          = var.service_short
  config_short           = var.config_short
  secret_key_vault_short = "app"

  is_rails_application = false

  # -----------------------------
  # NON‑SECRET APP SETTINGS
  # -----------------------------
  config_variables = {
    # Logging
    Logging__LogLevel__Default              = "Information"
    Logging__LogLevel__Microsoft.AspNetCore = "Warning"

    # Allowed hosts
    AllowedHosts = "*"

    # CsvMappings - Establishment
    CsvMappings__Establishment__Columns__0 = "Identifier.Urn"
    CsvMappings__Establishment__Columns__1 = "BasicDetails.Name"
    CsvMappings__Establishment__Columns__2 = "BasicDetails.EstablishmentType"
    CsvMappings__Establishment__Columns__3 = "BasicDetails.PhaseOfEducation"
    CsvMappings__Establishment__Columns__4 = "BasicDetails.Status"
    CsvMappings__Establishment__Columns__5 = "Address.Street"
    CsvMappings__Establishment__Columns__6 = "Address.Town"
    CsvMappings__Establishment__Columns__7 = "Address.Postcode"

    CsvMappings__Establishment__Headers__0 = "URN"
    CsvMappings__Establishment__Headers__1 = "EstablishmentName"
    CsvMappings__Establishment__Headers__2 = "EstablishmentType"
    CsvMappings__Establishment__Headers__3 = "PhaseOfEducation"
    CsvMappings__Establishment__Headers__4 = "StatusCode"
    CsvMappings__Establishment__Headers__5 = "Address_Street"
    CsvMappings__Establishment__Headers__6 = "Address_Town"
    CsvMappings__Establishment__Headers__7 = "Address_Postcode"

    # CsvMappings - EstablishmentGroup
    CsvMappings__EstablishmentGroup__Columns__0 = "Identifier.UID"
    CsvMappings__EstablishmentGroup__Columns__1 = "BasicDetails.Name"
    CsvMappings__EstablishmentGroup__Columns__2 = "BasicDetails.GroupType"
    CsvMappings__EstablishmentGroup__Columns__3 = "GroupEstablishments[].URN"
    CsvMappings__EstablishmentGroup__Columns__4 = "GroupEstablishments[].Name"

    CsvMappings__EstablishmentGroup__Headers__0 = "UID"
    CsvMappings__EstablishmentGroup__Headers__1 = "GroupName"
    CsvMappings__EstablishmentGroup__Headers__2 = "GroupType"
    CsvMappings__EstablishmentGroup__Headers__3 = "URN"
    CsvMappings__EstablishmentGroup__Headers__4 = "EstablishmentName"

    # DefaultRequiredFields - Establishments
    DefaultRequiredFields__RequiredFields__Establishments__0 = "URN"
    DefaultRequiredFields__RequiredFields__Establishments__1 = "EstablishmentName"
    DefaultRequiredFields__RequiredFields__Establishments__2 = "EstablishmentType"
    DefaultRequiredFields__RequiredFields__Establishments__3 = "EducationPhase"
    DefaultRequiredFields__RequiredFields__Establishments__4 = "WebsiteAddress"
    DefaultRequiredFields__RequiredFields__Establishments__5 = "TelephoneNumber"
    DefaultRequiredFields__RequiredFields__Establishments__6 = "Street"
    DefaultRequiredFields__RequiredFields__Establishments__7 = "Town"
    DefaultRequiredFields__RequiredFields__Establishments__8 = "Postcode"
    DefaultRequiredFields__RequiredFields__Establishments__9 = "EstablishmentStatus"

    # DefaultRequiredFields - EstablishmentGroups
    DefaultRequiredFields__RequiredFields__EstablishmentGroups__0 = "UID"
    DefaultRequiredFields__RequiredFields__EstablishmentGroups__1 = "GroupName"
    DefaultRequiredFields__RequiredFields__EstablishmentGroups__2 = "GroupTypeName"
    DefaultRequiredFields__RequiredFields__EstablishmentGroups__3 = "EstablishmentURN"
    DefaultRequiredFields__RequiredFields__EstablishmentGroups__4 = "EstablishmentName"

    # ValidationPatterns
    ValidationPatterns__Patterns__Street    = "^(UNDEFINED|[\\p{L}\\p{N}'(\\)][\\p{L}\\p{N}\\s\\.,'\\/\\-\\–—\\&\\(\\):]{1,99})$"
    ValidationPatterns__Patterns__Town      = "^(UNDEFINED|[\\p{L}\\p{N}][\\p{L}\\p{N}\\s\\.,'\\/\\-\\(\\)\\&]{0,49})$"
    ValidationPatterns__Patterns__Postcode  = "^(UNDEFINED|(GIR 0AA|[A-Za-z]{1,2}[0-9][0-9A-Za-z]?\\s?[0-9][A-Za-z]{2}|[A-Za-z0-9][A-Za-z0-9\\s\\-\\/]{1,11}))$"
    ValidationPatterns__Patterns__Website   = "^(UNDEFINED|(?:https?:\\/\\/|www\\.)?[A-Za-z0-9.-]+\\.[A-Za-z]{2,}(?:\\/\\S*)?)$"
    ValidationPatterns__Patterns__Telephone = "^(UNDEFINED|(?:\\d{7,14}|0\\d{6,13}|44\\d{5,12}))$"
  }

  # -----------------------------
  # SECRET APP SETTINGS
  # -----------------------------
  secret_variables = {
    CONNECTIONSTRINGS__EDUBASE = "keyvault:CONNECTIONSTRINGS--EDUBASE"
  }
}
