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
    LOGGING__LOGLEVEL__DEFAULT               = "Information"
    LOGGING__LOGLEVEL__MICROSOFT__ASPNETCORE = "Warning"

    # Allowed hosts
    ALLOWEDHOSTS = "*"

    # CSV Mappings - Establishment
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__0 = "Identifier.Urn"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__1 = "BasicDetails.Name"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__2 = "BasicDetails.EstablishmentType"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__3 = "BasicDetails.PhaseOfEducation"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__4 = "BasicDetails.Status"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__5 = "Address.Street"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__6 = "Address.Town"
    CSVMAPPINGS__ESTABLISHMENT__COLUMNS__7 = "Address.Postcode"

    CSVMAPPINGS__ESTABLISHMENT__HEADERS__0 = "URN"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__1 = "EstablishmentName"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__2 = "EstablishmentType"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__3 = "PhaseOfEducation"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__4 = "StatusCode"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__5 = "Address_Street"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__6 = "Address_Town"
    CSVMAPPINGS__ESTABLISHMENT__HEADERS__7 = "Address_Postcode"

    # CSV Mappings - EstablishmentGroup
    CSVMAPPINGS__ESTABLISHMENTGROUP__COLUMNS__0 = "Identifier.UID"
    CSVMAPPINGS__ESTABLISHMENTGROUP__COLUMNS__1 = "BasicDetails.Name"
    CSVMAPPINGS__ESTABLISHMENTGROUP__COLUMNS__2 = "BasicDetails.GroupType"
    CSVMAPPINGS__ESTABLISHMENTGROUP__COLUMNS__3 = "GroupEstablishments[].URN"
    CSVMAPPINGS__ESTABLISHMENTGROUP__COLUMNS__4 = "GroupEstablishments[].Name"

    CSVMAPPINGS__ESTABLISHMENTGROUP__HEADERS__0 = "UID"
    CSVMAPPINGS__ESTABLISHMENTGROUP__HEADERS__1 = "GroupName"
    CSVMAPPINGS__ESTABLISHMENTGROUP__HEADERS__2 = "GroupType"
    CSVMAPPINGS__ESTABLISHMENTGROUP__HEADERS__3 = "URN"
    CSVMAPPINGS__ESTABLISHMENTGROUP__HEADERS__4 = "EstablishmentName"

    # Default Required Fields - Establishments
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__0  = "URN"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__1  = "EstablishmentName"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__2  = "EstablishmentType"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__3  = "EducationPhase"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__4  = "WebsiteAddress"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__5  = "EducationPhase"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__6  = "TelephoneNumber"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__7  = "Street"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__8  = "Town"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__9  = "Postcode"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTS__10 = "EstablishmentStatus"

    # Default Required Fields - EstablishmentGroups
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTGROUPS__0 = "UID"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTGROUPS__1 = "GroupName"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTGROUPS__2 = "GroupTypeName"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTGROUPS__3 = "EstablishmentURN"
    DEFAULTREQUIREDFIELDS__REQUIREDFIELDS__ESTABLISHMENTGROUPS__4 = "EstablishmentName"
  }

  # -----------------------------
  # SECRET APP SETTINGS
  # -----------------------------
  secret_variables = {
    CONNECTIONSTRINGS__EDUBASE = "keyvault:CONNECTIONSTRINGS--EDUBASE"
  }
}
