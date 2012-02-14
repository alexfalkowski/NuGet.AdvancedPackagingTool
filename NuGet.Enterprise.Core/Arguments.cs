namespace NuGet.Enterprise.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using FluentValidation.Results;

    using NuGet;

    public class Arguments
    {
        public bool Install { get; set; }

        public bool Uninstall { get; set; }

        public string Package { get; set; }

        public string Source { get; set; }

        [TypeConverter(typeof(VersionTypeConverter))]
        public SemanticVersion Version { get; set; }

        public IEnumerable<string> Errors
        {
            get
            {
                return this.ValidationResult.Errors.Select(error => error.ErrorMessage);
            }
        }

        public bool IsValid
        {
            get
            {
                return this.ValidationResult.IsValid;
            }
        }

        private ValidationResult ValidationResult
        {
            get
            {
                var validator = new ArgumentsValidator();
                var result = validator.Validate(this);
                return result;
            }
        }
    }
}