using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.BaseClasses.OutpuModels;

public class IdTitleViewModel
{
    public string Id { get; set; }

    public string Title { get; set; }
}

//Task<ICollection<IdTitleViewModel>> GetTitleListByFilter(EntityTitleListFilterAndSelector inputData, Expression<Func<TEntity, bool>> extraWhereCondition = null);

