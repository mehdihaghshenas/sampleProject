using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.Helpers;
using Moq;

namespace TestHelpers
{
    public class IBaseRepositoryMock<T,TKey, Repo> where T : class, IBaseEntity where Repo : class, IBaseRepository<T,TKey>
    {
        public Mock<Repo> MockRepository { get; set; }
        List<T> _entities;
        public List<T> Entities { get => _entities; }
        public IBaseRepositoryMock()
        {
            _entities = new List<T>();
#pragma warning disable CS8603 // Possible null reference return.
            MockRepository = new Mock<Repo>();

            MockRepository.Setup(x => x.GetAll()).Returns(() => _entities.AsQueryable());
            MockRepository.Setup(x => x.Get(It.IsAny<TKey>()))
                .Returns<TKey>(x => _entities.FirstOrDefault(ExpressionHelpers.GetIdFilter<T>(x).Compile()));

            MockRepository.Setup(x => x.GetAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
    .Returns<object, CancellationToken>((x, _) => Task.FromResult(_entities.FirstOrDefault(ExpressionHelpers.GetIdFilter<T>(x).Compile())));

            MockRepository.Setup(x => x.InsertWithSaveChange(It.IsAny<T>()))
                .Returns<T>(x =>
                {
                    _entities.Add(x);
                    return x;
                });

            MockRepository.Setup(x => x.InsertWithSaveChangeAsync(It.IsAny<T>(), CancellationToken.None))
                .Returns<T, CancellationToken>((x, _) =>
                {
                    _entities.Add(x);
                    return Task.FromResult(x);
                });

            MockRepository.Setup(x => x.RemoveWithSaveChangeAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<object, CancellationToken>((x, _) =>
                {
                    var func = ExpressionHelpers.GetIdFilter<T>(x).Compile();
                    var entity = _entities.FirstOrDefault(func);
                    if (entity == null)
                        throw new NotFoundException("Enity dos not found");

                    _entities.Remove(entity);
                    return Task.FromResult(1);
                });

            MockRepository.Setup(x => x.UpdateWithSaveChange(It.IsAny<T>())).Callback<T>(x =>
            {
                var func = ExpressionHelpers.GetIdFilter<T>(x.GetPrimaryKeyValue()).Compile();
                _entities.Remove(_entities.First(func));
                _entities.Add(x);
            });

            MockRepository.Setup(x => x.UpdateWithSaveChangeAsync(It.IsAny<T>(), CancellationToken.None))
                .Returns<T, CancellationToken>((x, _) =>
                {
                    object keyValue = x.GetPrimaryKeyValue();
                    var func = ExpressionHelpers.GetIdFilter<T>(keyValue).Compile();
                    var entity = _entities.FirstOrDefault(func);
                    if (entity == null)
                        throw new NotFoundException("Enity dos not found");
                    _entities.Remove(entity);
                    _entities.Add(x);
                    return Task.FromResult(1);
                });
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}