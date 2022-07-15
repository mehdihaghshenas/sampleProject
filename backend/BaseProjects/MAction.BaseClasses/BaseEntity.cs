
using System;

using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace MAction.BaseClasses
{
    public abstract class BaseEntity : IBaseEntity
    {
        public string GetPrimaryKeyPropertyName()
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            return props.Name;
        }
        public object GetPrimaryKeyValue()
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            return props.GetValue(this);
        }
        public Type GetPrimaryKeyType()
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            return props.PropertyType;
        }
        public void SetPrimaryKeyValue(object value)
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            props.SetValue(this, value);
        }

        public int GetPrimaryKeyIntValue()
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            return (int)props.GetValue(this);
        }
        public void SetPrimaryKeyValue(int value)
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            props.SetValue(this, value);
        }
        public void SetPrimaryKeyValue(Guid value)
        {
            var props = this.GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0).First();
            props.SetValue(this, value);
        }

    }

    public abstract class BaseEntityWithCreationInfo : BaseEntity, IEntityCreationInfo
    {
        //[Key]
        //public int Id { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public string TimeZone { get; set; }
        public string UserCreationId { get; set; }
        //[DefaultValue(false)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public bool IsDeleted { get; set; }

    }

    public interface IEntityCreationInfo
    {
        public DateTimeOffset CreateAt { get; set; }
        public string TimeZone { get; set; }
        public string UserCreationId { get; set; }
    }
}
