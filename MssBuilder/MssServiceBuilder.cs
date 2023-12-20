using Mss;
using MssBuilder.Projects;

namespace MssBuilder
{
    public class MssServiceBuilder
    {
        public MssWebApiProject Build(MssService service)
        {
            MssWebApiProject project = new(service.Name, service.Name);

            return project;
        }
    }
}