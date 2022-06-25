using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseMongoRepository
{
    public interface IMongoDependencyProvider
    {
        string DatabaseName { get; set; }
    }

}
