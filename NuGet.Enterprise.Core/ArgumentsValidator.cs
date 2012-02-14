namespace Ninemsn.PackageManager.NuGet
{
    using FluentValidation;

    using global::NuGet.Enterprise.Core.Properties;

    public class ArgumentsValidator : AbstractValidator<Arguments>
    {
         public ArgumentsValidator()
         {
             this.RuleFor(argument => argument.Install).Must((argument, install) => install != argument.Uninstall)
                 .WithMessage(Resources.InvalidInstallUninstallFlag);
             this.RuleFor(argument => argument.Package).NotEmpty().WithMessage(Resources.PackageNotSpecified);
         }
    }
}