using MAction.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MAction.BaseClasses.Helpers;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;

namespace MAction.SampleOnion.Domain.Entity.SAL
{
    public class SaleCompany : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        //[BsonRepresentation(BsonType.String)]
        public LanguageEnum Language { get; set; }
    }
}
