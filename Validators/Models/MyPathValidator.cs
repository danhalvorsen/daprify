using Daprify.Models;
using Daprify.Services;
using FluentValidation;
using Serilog;

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
                    else
                    { Log.Verbose("Validation success: Path {Path} exists", fullPath); }
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
            Log.Verbose("Validating if path {Path} exists", path);
            return Directory.Exists(path.ToString()) || File.Exists(path.ToString());
        }
    }
}
