namespace Ninemsn.PackageManager.NuGet.App
{
    using FluentValidation;

    using Ninemsn.PackageManager.NuGet.Properties;

    public class ArgumentsValidator : AbstractValidator<Arguments>
    {
         public ArgumentsValidator()
         {
             this.RuleFor(argument => argument.Install).Must((argument, install) => install != argument.Uninstall)
                 .WithMessage(Resources.InvalidInstallUninstallFlag);
             this.RuleFor(argument => argument.Package).NotEmpty().WithMessage(Resources.PackageNotSpecified);
             this.RuleFor(argument => argument.Source).NotEmpty().WithMessage(Resources.SourceNotSpecified);
         }
    }
}