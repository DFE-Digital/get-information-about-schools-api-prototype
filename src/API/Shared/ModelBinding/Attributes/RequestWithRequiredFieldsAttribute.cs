namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes
{
    /// <summary>
    /// Identifies a property or action parameter whose value should be bound
    /// using <see cref="RequestWithRequiredFieldsModelBinder"/>. The attribute
    /// specifies a logical key used to look up the default set of required
    /// fields when the client does not provide explicit values in the query
    /// string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class RequestWithRequiredFieldsAttribute : Attribute
    {
        /// <summary>
        /// Gets the configuration key used to retrieve the default required
        /// fields from <see cref="DefaultRequiredFields"/> when no explicit
        /// query-string values are supplied by the client.
        /// </summary>
        public string DefaultFieldsKey { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RequestWithRequiredFieldsAttribute"/> class.
        /// </summary>
        /// <param name="defaultFieldsKey">
        /// The key used to select the appropriate default field set from the
        /// <see cref="DefaultRequiredFields.RequiredFields"/> dictionary.
        /// </param>
        public RequestWithRequiredFieldsAttribute(string defaultFieldsKey)
        {
            DefaultFieldsKey = defaultFieldsKey;
        }
    }
}
