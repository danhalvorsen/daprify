using CLI.Models;

namespace CLI.Services
{
    public interface IService
    {
        public void Generate(OptionDictionary options);
    }
}