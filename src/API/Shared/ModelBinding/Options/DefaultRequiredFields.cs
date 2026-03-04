namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Options
{
    /// <summary>
    /// Represents the configured default required fields for request models
    /// that use <c>RequestWithRequiredFieldsAttribute</c>. The dictionary maps
    /// a logical key (defined by the attribute) to the set of field names that
    /// should be used when the client does not supply explicit values.
    /// </summary>
    public class DefaultRequiredFields
    {
        /// <summary>
        /// Gets or sets the mapping of default field sets. The dictionary key
        /// corresponds to the <c>DefaultFieldsKey</c> specified on the attribute,
        /// and the value is the array of field names that should be applied when
        /// no explicit query-string values are provided by the client.
        /// </summary>
        public Dictionary<string, string[]> RequiredFields { get; set; } = new();
    }
}