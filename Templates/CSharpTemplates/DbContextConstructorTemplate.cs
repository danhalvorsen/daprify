namespace CLI.Templates
{
    public class DbContextConstructorTemplate() : HandlebarTemplate(TEMPLATE_STR)
    {
        private const string TEMPLATE_STR =
@"
        public {{Class}}()
        {
            try
            {
                _ = Database.EnsureCreated();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public {{Class}}(DbContextOptions<{{Class}}> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(""{{Service}}DB"");
        }
";

        protected override string TemplateString => TEMPLATE_STR;

        public string Render(string serviceName, string className)
        {
            var data = new
            {
                Service = serviceName,
                Class = className
            };
            return _template(data);
        }
    }
}