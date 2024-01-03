using Daprify.Models;
using Daprify.Services;
using FluentValidation;

namespace Daprify.Validation
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
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
            MyPath combinedPath = MyPath.Combine(currentDir.ToString(), path.ToString());

            return MyPath.GetFullPath(combinedPath);
        }

        private static bool BeAValidPath(MyPath path)
        {
            return Directory.Exists(path.ToString());
        }
    }
}
