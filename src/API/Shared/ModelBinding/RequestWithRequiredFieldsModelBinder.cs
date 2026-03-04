using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding
{
    /// <summary>
    /// A model binder that extracts repeated query-string values for the
    /// <c>requiredEstablishmentFields</c> parameter. If no values are provided,
    /// the binder falls back to a predefined set of default fields.
    /// </summary>
    public class RequestWithRequiredFieldsModelBinder : IModelBinder
    {
        /// <summary>
        /// The query-string key used to extract repeated field values.
        /// </summary>
        private const string QueryKey = "requiredFields";

        /// <summary>
        /// The default fields to use when no explicit values are provided
        /// in the query string.
        /// </summary>
        private readonly string[] _defaultFields;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RequestWithRequiredFieldsModelBinder"/> class.
        /// </summary>
        /// <param name="defaultFields">
        /// The default field names to use when the query string does not
        /// contain any values for <c>requiredEstablishmentFields</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="defaultFields"/> is <c>null</c>.
        /// </exception>
        public RequestWithRequiredFieldsModelBinder(string[] defaultFields)
        {
            _defaultFields = defaultFields ?? throw new ArgumentNullException(nameof(defaultFields));
        }

        /// <summary>
        /// Attempts to bind the model by extracting repeated values from the
        /// query string. If values are present for <c>requiredEstablishmentFields</c>,
        /// they are returned; otherwise, the default fields are used.
        /// </summary>
        /// <param name="bindingContext">
        /// The context for model binding, including the HTTP request and
        /// metadata about the model being bound.
        /// </param>
        /// <returns>
        /// A completed task representing the asynchronous bind operation.
        /// </returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            try
            {
                IQueryCollection query = bindingContext.HttpContext.Request.Query;

                string[] repeatedValues =
                    [.. query[QueryKey]
                        .Where(value => value != null)
                        .Select(value => value!)];

                string[] result =
                    repeatedValues.Length > 0
                        ? repeatedValues
                        : _defaultFields;

                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
