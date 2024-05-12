

// // Adds Swagger support for optional parameters with default values
// // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/575
// using System.ComponentModel;
// using System.Reflection;
// using Microsoft.AspNetCore.Mvc.ApiExplorer;
// using Microsoft.AspNetCore.Mvc.Controllers;
// using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
// using Swashbuckle.AspNetCore.SwaggerGen;

// public class UpdateOptionalParamatersWithDefaultValues : IOperationFilter
// {
//     // private readonly MvcJsonOptions _mvcJsonOptions;

//     public UpdateOptionalParamatersWithDefaultValues(
//         // IOptions<MvcJsonOptions> mvcJsonOptions
//         )
//     {
//         // _mvcJsonOptions = mvcJsonOptions.Value;
//     }

//     public void Apply(Operation operation, OperationFilterContext context)
//     {
//         if (operation.Parameters == null || !operation.Parameters.Any())
//         {
//             return;
//         }

//         var parameterValuePairs = context.ApiDescription.ParameterDescriptions
//             .Where(parameter => GetDefaultValueAttribute(parameter) != null || GetParameterInfo(parameter).HasDefaultValue)
//             .ToDictionary(parameter => parameter.Name, GetDefaultValue);

//         foreach (var parameter in operation.Parameters)
//         {
//             if (parameterValuePairs.TryGetValue(parameter.Name, out var defaultValue))
//             {
//                 parameter.Extensions.Add("default", defaultValue);
//                 parameter.Required = false;
//             }
//         }
//     }

//     private DefaultValueAttribute GetDefaultValueAttribute(ApiParameterDescription parameter)
//     {
//         if (!(parameter.ModelMetadata is DefaultModelMetadata metadata) || metadata.Attributes.PropertyAttributes == null)
//         {
//             return null;
//         }

//         return metadata.Attributes.PropertyAttributes
//             .OfType<DefaultValueAttribute>()
//             .FirstOrDefault();
//     }

//     public ParameterInfo GetParameterInfo(ApiParameterDescription parameter)
//     {
//         return ((ControllerParameterDescriptor) parameter.ParameterDescriptor).ParameterInfo;
//     }

//     private object GetDefaultValue(ApiParameterDescription parameter)
//     {
//         var parameterInfo = GetParameterInfo(parameter);

//         if (parameterInfo.HasDefaultValue)
//         {
//             // if (parameter.Type.IsEnum)
//             // {
//             //     var stringEnumConverter = _mvcJsonOptions.SerializerSettings.Converters
//             //         .OfType<StringEnumConverter>()
//             //         .FirstOrDefault();

//             //     if (stringEnumConverter != null)
//             //     {
//             //         var defaultValue = parameterInfo.DefaultValue.ToString();
//             //         return stringEnumConverter.CamelCaseText ? ToCamelCase(defaultValue) : defaultValue;
//             //     }
//             // }

//             return parameterInfo.DefaultValue;
//         }

//         var defaultValueAttribute = GetDefaultValueAttribute(parameter);

//         return defaultValueAttribute.Value;
//     }

//     private string ToCamelCase(string name)
//     {
//         return char.ToLowerInvariant(name[0]) + name.Substring(1);
//     }
// }