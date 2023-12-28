using CLI.Models;
using FluentValidation;

namespace CLI.Validation
{
    public class MyPathValidator : AbstractValidator<IEnumerable<MyPath>>
    {
        public MyPathValidator()
        {
            RuleForEach(val => val)
                .Custom((path, context) =>
                {
                    MyPath fullPath = GetPath(path);
                    if (!BeAValidPath(fullPath))
                    {
                        string error = ErrorMessage.NotFoundErrorFunc("path", fullPath);
                        context.AddFailure(error);
                    }
                });
        }

        private static MyPath GetPath(MyPath path)
        {
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            MyPath combinedPath = MyPath.Combine(currentDir, path);

            return MyPath.GetFullPath(combinedPath);
        }

        private static bool BeAValidPath(MyPath path)
        {
            return Directory.Exists(path.ToString());
        }
    }
}
