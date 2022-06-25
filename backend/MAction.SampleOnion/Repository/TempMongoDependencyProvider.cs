using MAction.BaseMongoRepository;


namespace MAction.SampleOnion.Repository
{
    public partial class ServiceRegistration
    {
        public class TempMongoDependencyProvider : IMongoDependencyProvider
        {
            public string DatabaseName { get; set; } = "";
        }

    }
}