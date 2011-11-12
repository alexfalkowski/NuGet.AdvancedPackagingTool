﻿namespace Ninemsn.PackageManager.NuGet.App
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentValidation.Results;

    public class Arguments
    {
        public bool Install { get; set; }

        public bool Uninstall { get; set; }

        public string Package { get; set; }

        public string Destination { get; set; }

        public string Source { get; set; }

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