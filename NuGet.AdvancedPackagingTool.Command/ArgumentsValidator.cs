namespace NuGet.AdvancedPackagingTool.Command
{
    using System.Diagnostics.CodeAnalysis;

    using FluentValidation;

    using NuGet.AdvancedPackagingTool.Command.Properties;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
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