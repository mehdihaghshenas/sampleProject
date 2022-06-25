using MAction.BaseMongoRepository;

//using AutoMapperBuilder.Extensions.DependencyInjection;

//using PR.Service.HRModels.LoanPaymentModel;

namespace MAction.BaseMongoRepository
{
    public partial class ServiceRegistration
    {
        public class TempMongoDependencyProvider : IMongoDependencyProvider
        {
            public string DatabaseName { get; set; } = "";
        }

    }
}