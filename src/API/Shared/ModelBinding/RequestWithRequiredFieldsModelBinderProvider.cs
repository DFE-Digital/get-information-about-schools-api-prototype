using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding
{
    /// <summary>
    /// Provides a model binder for properties decorated with
    /// <see cref="RequestWithRequiredFieldsAttribute"/>. This binder injects
    /// default required fields when the client does not supply explicit values.
    /// </summary>
    public class RequestWithRequiredFieldsModelBinderProvider : IModelBinderProvider
    {
        private readonly DefaultRequiredFields _requiredFields;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RequestWithRequiredFieldsModelBinderProvider"/> class.
        /// </summary>
        /// <param name="requiredFields">
        /// The configured default required fields, injected via <see cref="IOptions{TOptions}"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requiredFields"/> is <c>null</c>.
        /// </exception>
        public RequestWithRequiredFieldsModelBinderProvider(DefaultRequiredFields requiredFields)
        {
            _requiredFields = requiredFields ??
                throw new ArgumentNullException(nameof(requiredFields));
        }

        /// <summary>
        /// Returns a model binder for properties decorated with
        /// <see cref="RequestWithRequiredFieldsAttribute"/>, or <c>null</c> if
        /// the provider does not apply to the current binding context.
        /// </summary>
        /// <param name="context">The model binder provider context.</param>
        /// <returns>
        /// An instance of <see cref="IModelBinder"/> when applicable; otherwise <c>null</c>.
        /// </returns>
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Must be a property with a container and name
            if (context.Metadata.MetadataKind != ModelMetadataKind.Property ||
                context.Metadata.ContainerType == null ||
                context.Metadata.PropertyName == null)
            {
                return null;
            }

            Type containerType = context.Metadata.ContainerType;
            string propertyName = context.Metadata.PropertyName;

            // Retrieve the property info safely
            PropertyInfo? propertyInfo = containerType.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propertyInfo == null){
                return null;
            }

            // Check for the attribute
            RequestWithRequiredFieldsAttribute? attribute =
                propertyInfo.GetCustomAttribute<RequestWithRequiredFieldsAttribute>();

            if (attribute == null){
                return null;
            }

            // Resolve default fields safely
            DefaultRequiredFields options = _requiredFields;

            if (!options.RequiredFields.TryGetValue(attribute.DefaultFieldsKey, out string[]? defaultFields))
            {
                defaultFields = [];
            }

            return new RequestWithRequiredFieldsModelBinder(defaultFields);
        }
    }
}
