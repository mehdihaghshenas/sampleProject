using MAction.BaseClasses;
using System;
using System.Linq.Expressions;

namespace MAction.BaseServices
{
    public interface ISelectWithModelMapper<TEntitiy, TResult> where TEntitiy : BaseEntity
    {
        TResult MapEntityToOutput(TEntitiy entitiy);
    }
    public interface ISelectWithModelExpression<TEntitiy, TResult> where TEntitiy : BaseEntity
    {
        Expression<Func<TEntitiy, TResult>> SelectExpression();
    }

    public interface IInputModelCustomMapper<TEntity, InputModel>
    {
        TEntity MapInputToEnity(InputModel inputModel);
    }
}
