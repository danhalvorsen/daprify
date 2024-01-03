using Daprify.Settings;

namespace Daprify.Validation
{
    public interface IOptionValidatorFactory
    {
        OptionValidator CreateValidator(ISettings settings);
    }

    public class OptionValidatorFactory(MyPathValidator myPathValidator) : IOptionValidatorFactory
    {
        private readonly MyPathValidator _myPathValidator = myPathValidator;

        public OptionValidator CreateValidator(ISettings settings)
        {
            return new OptionValidator(_myPathValidator, settings);
        }
    }
}