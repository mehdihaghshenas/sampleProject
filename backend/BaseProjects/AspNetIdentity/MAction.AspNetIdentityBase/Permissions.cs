using Microsoft.AspNetCore.Authorization;

using System.ComponentModel;

using System.Reflection;


namespace MAction.AspNetIdentity.Base
{
    public abstract class PoliciesBase
    {
    }
    public static class PolicyLoader
    {

        public class CustomClaimTypes
        {
            public const string ClaimType = "Permission.Claims";
        }
        public static Dictionary<string, List<string>> PolicyDefinitions;
        static List<PolicyDto> _allpolicies = new List<PolicyDto>();
        public static void AddPolices(Assembly assembly)
        {
            PolicyDefinitions = new Dictionary<string, List<string>>();
            var types = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(PoliciesBase)));
            foreach (var type in types)
            {
                var prop = type.GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanRead && !x.CanWrite);

                foreach (var propertyInfo in prop)
                {
                    PolicyDefinitions.Add(type.Name + "." + propertyInfo.Name, new List<string>() { type.Name + "." + propertyInfo.Name });
                }
                var tempList = prop.Select(p => new PolicyDto()
                {
                    Description = Attribute.IsDefined(p, typeof(DescriptionAttribute)) ? (Attribute.GetCustomAttribute(p, typeof(DescriptionAttribute)) as DescriptionAttribute).Description : string.Empty,
                    Value = (string)p.GetValue(type)
                }).ToList();

                _allpolicies.AddRange(tempList);
            }

        }
        public static void RegisterPolicies(AuthorizationOptions options)
        {
            foreach (var item in PolicyDefinitions.Keys)
            {
                options.AddPolicy(item, policy => policy.RequireClaim(CustomClaimTypes.ClaimType, PolicyDefinitions[item].ToArray()));
            }
        }
        public static List<string> GetPolicies()
        {

            List<string> list = new List<string>();
            foreach (var item in PolicyDefinitions.Values)
            {
                list.AddRange(item);
            }

            return list;
        }
        public static List<PolicyDto> GetAllPolicies()
        {
            return _allpolicies;
        }
    }
    public class PolicyDto
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }


    public class SecurityPolicies : PoliciesBase
    {
        public static string CanChangeUserPasswordWithOutOldPassword { get; } = $"{nameof(SecurityPolicies)}.{nameof(CanChangeUserPasswordWithOutOldPassword)}";
        public static string AllowToPermitAllPermission { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToPermitAllPermission)}";
        public static string AllowToChangeUserPassword { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToChangeUserPassword)}";
        public static string AllowToManagePermission { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToManagePermission)}";
        public static string AllowToManageUsers { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToManageUsers)}";
        public static string AllowToManageRoles { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToManageRoles)}";
        public static string AllowToViewRoles { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToViewRoles)}";
        public static string AllowToViewUsers { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToViewUsers)}";
        public static string AllowToViewPermission { get; } = $"{nameof(SecurityPolicies)}.{nameof(AllowToViewPermission)}";
    }
}