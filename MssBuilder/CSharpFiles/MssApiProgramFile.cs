using System.Text;

namespace MssBuilder
{
    public class MssApiProgramFile(string nameSpace) : MssCSharpFile("Program.cs", "")
    {
        public readonly string NameSpace = nameSpace;

        private void AddEndpoint(StringBuilder builder, string indent, string endpoint, string description, int statusCode)
        {
            builder.AppendLine(indent + "app.Map(" + endpoint + ", builder => builder.Run(async context =>");
            builder.AppendLine("{");
            string newIndent = indent + "    ";
            builder.AppendLine(newIndent + "context.Response.StatusCode = " + statusCode + ";");
            builder.AppendLine(newIndent + "await context.Response.WriteAsync(\"" + description + "\");");
            builder.AppendLine(indent + "}));");
        }

        private void AddStartupEndpoints(StringBuilder builder, string indent)
        {
            AddEndpoint(builder, indent, "Program.ROOT + Program.QUERY", NameSpace + " does not have query functionality yet!", 400);
            AddEndpoint(builder, indent, "Program.ROOT + Program.CMD", NameSpace + " does not have cmd functionality yet!", 400);
        }

        private void AddStartupConfigure(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "public virtual void Configure(IApplicationBuilder app)");
            builder.AppendLine(indent + "{");
            AddStartupEndpoints(builder, indent + "    ");
            builder.AppendLine(indent + "}");
        }

        private void AddStartupConfigureServices(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "public virtual void ConfigureServices(IServiceCollection services)");
            builder.AppendLine(indent + "{");
            builder.AppendLine(indent + "}");
        }

        private void AddStartupClass(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "public class Startup");
            builder.AppendLine(indent + "{");
            string newIndent = indent + "    ";
            builder.AppendLine(newIndent + "public Startup()");
            builder.AppendLine(newIndent + "{");
            builder.AppendLine(newIndent + "}");
            builder.AppendLine("");
            AddStartupConfigureServices(builder, newIndent);
            builder.AppendLine("");
            AddStartupConfigure(builder, newIndent);
            builder.AppendLine(indent + "}");
        }

        private void AddHostBuilderMethod(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "private static IHostBuilder CreateHostBuilder(string[] args)");
            builder.AppendLine(indent + "{");
            string newIndent = indent + "    ";
            builder.AppendLine(newIndent + "return Host.CreateDefaultBuilder(args)");
            newIndent += "    ";
            builder.AppendLine(newIndent + ".ConfigureWebHostDefaults(webBuilder =>");
            builder.AppendLine(newIndent + "{");
            string newestIndent = newIndent + "    ";
            builder.AppendLine(newestIndent + "webBuilder.UseStartup<Startup>();");
            builder.AppendLine(newIndent + "});");
            builder.AppendLine(indent + "}");
        }

        private void AddMainMethod(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "public static void Main(string[] args)");
            builder.AppendLine(indent + "{");
            string newIndent = indent + "    ";
            builder.AppendLine(newIndent + "var app = CreateHostBuilder(args).Build();");
            builder.AppendLine(newIndent + "app.Run();");
            builder.AppendLine(indent + "}");
        }

        private void AddProgramFields(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + @"public const string ROOT = ""/"";");
            builder.AppendLine(indent + @"public const string QUERY = ""query"";");
            builder.AppendLine(indent + @"public const string CMD = ""cmd"";");
        }

        private void AddProgramClass(StringBuilder builder, string indent)
        {
            builder.AppendLine(indent + "internal sealed class Program");
            builder.AppendLine(indent + "{");
            AddProgramFields(builder, indent + "    ");
            builder.AppendLine("");
            AddHostBuilderMethod(builder, indent + "    ");
            builder.AppendLine("");
            AddMainMethod(builder, indent + "    ");
            builder.AppendLine(indent + "}");
        }

        public override void Write(string path)
        {
            StringBuilder builder = new();

            // builder.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            // builder.AppendLine("using Microsoft.Extensions.Hosting;");
            // builder.AppendLine("using Microsoft.AspNetCore.Hosting;");
            // builder.AppendLine("");

            builder.AppendLine("namespace " + NameSpace);
            builder.AppendLine("{");
            AddStartupClass(builder, "    ");
            builder.AppendLine("");
            AddProgramClass(builder, "    ");
            builder.AppendLine("}");

            _content = builder.ToString();

            base.Write(path);
        }
    }
}