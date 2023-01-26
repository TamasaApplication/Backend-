using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using static AhmadBase.Core.SearchFilter;

namespace AhmadBase.Core
{









    public static class Extention
    {
        public static string TryParse(string value)
        {
            try
            {
                return JToken.Parse(value).ToString();
            }
            catch (System.Exception)
            {

            }
            return null;
        }
        public static bool TryFromObject(object value, out dynamic result)
        {
            try
            {
                var kind = JToken.FromObject(value).First.First.Value<int>();
                switch (kind)
                {
                    default:
                    case (int)JsonValueKind.String:
                        result = value.ToString();
                        break;
                    case (int)JsonValueKind.Number:
                        result = Convert.ToInt32(value);
                        break;
                }
                return true;
            }
            catch (System.Exception)
            {
                result = null;
            }
            return false;
        }

        public static dynamic ConvertTo(this object source, Type typeTo, bool isNullable = false)
        {

            dynamic getValue()
            {
                try
                {

                    var d = typeTo.IsEnum ? Enum.Parse(typeTo, source.ToString()) : null;
                    if (source is IConvertible)
                        d = Convert.ChangeType(source, typeTo);
                    if (TryFromObject(source, out var res))
                        d = res;
                    if (isNullable)
                        return GetNullable((dynamic)d);
                    return d;
                }
                catch (System.Exception)
                {
                    return typeTo.GetDefaultValue();
                }
            }

            var typeName = typeTo.IsEnum ? nameof(Enum) : typeTo.Name;
            if (isNullable && typeTo.GenericTypeArguments.Length > 0)
                typeName = typeTo.GenericTypeArguments[0].Name;

            switch (typeName)
            {
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(Byte):
                case nameof(String):
                case nameof(DateTime):
                case nameof(Boolean):
                case nameof(Enum):
                    {
                        return getValue();
                    }
                default:
                    return (null);
            }
            // return false;
        }

        public static dynamic CreateGenericList(this object data, dynamic defulatValue, bool isNullable = false)
        {
            var arr = Newtonsoft.Json.Linq.JArray.FromObject(data);

            dynamic values = null;
            if (!isNullable)
                values = GetList(defulatValue);
            else
                values = GetNullableList(defulatValue);
            foreach (var item in arr)
            {
                var d = item.ToObject<dynamic>();
                var s = ConvertTo(d, defulatValue.GetType(), isNullable);
                values.Add(s);
            }
            return values;
        }

        public static dynamic GetList<T>(this T value)
        {
            var t = typeof(List<>).MakeGenericType(value.GetType());
            var d = Activator.CreateInstance(t);
            return d;
        }

        public static dynamic GetNullableList<T>(this T value) where T : struct
        {
            var nt = Type.GetType($"System.Nullable`1[{value.GetType()}]");
            var t = typeof(List<>).MakeGenericType(nt);
            var d = Activator.CreateInstance(t);
            return d;
        }

        public static Nullable<T> GetNullable<T>(this T s) where T : struct
        {
            return (Nullable<T>)s;
        }

        public static bool IsList(this object source)
        {
            if (source is null)
                return false;
            var t = JToken.FromObject(source);
            return t is JArray;
        }

        public static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType)
            {
                if (!(Nullable.GetUnderlyingType(t) is null))
                    t = Nullable.GetUnderlyingType(t);

                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }

        public static string ToLatinDigit(this string str)
        {
            Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9',
                ['.'] = '.'
            };
            foreach (var item in str)
            {
                switch (item)
                {
                    case '۰':
                    case '۱':
                    case '۲':
                    case '۳':
                    case '۴':
                    case '۵':
                    case '۶':
                    case '۷':
                    case '۸':
                    case '۹':
                    case '.':
                        str = str.Replace(item, LettersDictionary[item]);
                        break;
                }
            }
            return str;
        }
    }

    public enum FilterOperation
    {
        Equals,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        IsNull,
        IsNotNull,
        Any,
        NotAny,
        In,
        NotIn
    }
    public class Filter
    {
        public string PropertyName { get; set; }
        public FilterOperation Operation { get; set; }
        public object Value { get; set; }

        public Filter()
        {

        }
        public Filter(string propertyName, FilterOperation operation, object value)
        {
            this.PropertyName = propertyName;
            this.Operation = operation;
            this.Value = value;
        }

        public Expression Translate<T>(ParameterExpression param)
        {
            var exp = new FilterTranslator().Translate<T>(this, param);
            return exp;
        }
    }

    public class FilterTranslator
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo ContainsMethod2 = typeof(string).GetMethods()
            .Where(x => x.Name.Equals("Contains")).First();

        private static readonly MethodInfo StartsWithMethod =

            typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

        private static readonly MethodInfo EndsWithMethod =
            typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        private static readonly MethodInfo AnyMethod =
            typeof(Enumerable)
            .GetTypeInfo()
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" && m.GetParameters().Count() == 2);

        public Expression Translate<T>(Filter filter, ParameterExpression param)
        {
            var exp = GetExpression<T>(filter, param);
            return exp;
        }

        /// <summary>
        /// جهت ایجاد ممبر اکسپرشن که معرف کانتینر و متغییر سطح آن میباشد، استفاه میشود
        /// city: CityTitle==تهران
        /// </summary>
        /// <param name="param"></param>
        /// <param name="propertyName"></param>
        /// <returns>city100=> city100.CityTitle</returns>
        private MemberExpression GetProperty(Expression param, string propertyName)
        {
            string[] props = propertyName.Split('.');
            MemberExpression member = Expression.Property(param, props[0]);
            if (props.Length > 1 && member.Type.GetTypeInfo().Namespace.Contains("System.Collections") == false)
            {
                /// در صورت وجود متغییرهای پی در پی که مجموعه نباشند این تابع به صورت بازگشتی استفاده میشود تا به سطح آخر برسیم
                /// city: City100=> City100.Province.Country.CountryTitle
                /// 
                propertyName = propertyName.Replace(props[0] + ".", "");
                return GetProperty(member, propertyName);
            }
            else if (member.Type.GetTypeInfo().Namespace.Contains("System.Collections") == true)
            {
                /// در صورتی که به متغییر برخورد کنیم که به صورت مجموعه باشد در این تابع ادامه نمیدهیم و در تابع دیگری عملیات ادامه می یابد
                /// family: family100.TbFamilyMember.FirstName
                /// 
                return member;
            }
            return member;
        }

        /// <summary>
        /// حذف قسمت هایی از فیلتر که به صورت ممبر اکسپرشن درآمده اند
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        private string PrepareFilter(MemberExpression param, string PropertyName)
        {
            var props = PropertyName.Split('.').ToList();

            var filterToRemove = string.Empty;
            for (int i = 0; i < props.Count; i++)
            {
                if (props[i] != param.Type.GetTypeInfo().GetGenericArguments()[0].Name)
                {
                    filterToRemove += props[i] + ".";
                }
                else
                {
                    filterToRemove += props[i] + ".";
                    break;
                }
            }
            PropertyName = PropertyName.Replace(filterToRemove, "");
            return PropertyName;
        }

        public Type GetEnumType(string enumName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }
            return null;
        }

        private Expression GetExpression<T>(Filter filter, ParameterExpression param)
        {
            Expression retVal = null;
            ConstantExpression constant = null;
            MemberExpression member = GetProperty(param, filter.PropertyName);

            ///در صورتی که در پیمایش به مجموعه برخورد کنیم ادامه ساخت ممبر اکسپرشن متوقف شده و از طریق زیر یک ممبر اکسپرشن جدید ایجاد میگردد که داخل پرانتز انی قرار میگرید
            ///
            if (member.Type.GetTypeInfo().Namespace.Contains("System.Collections") == true &&
                (filter.Operation != FilterOperation.Any && filter.Operation != FilterOperation.NotAny))
            {
                var par = Expression.Parameter(member.Type.GetTypeInfo().GetGenericArguments()[0],
                    member.ToString().Replace('.', '_') +
                    "_" +
                    member.Type.GetTypeInfo().GetGenericArguments()[0].Name);
                filter.PropertyName = PrepareFilter(member, filter.PropertyName);
                var mem = GetProperty(par, filter.PropertyName);
                if (filter.PropertyName.Split('.').ToList().Count <= 2)
                {
                    Type t = Nullable.GetUnderlyingType(Type.GetType(mem.Type.FullName)) ?? Type.GetType(mem.Type.FullName);
                    object safeValue = (filter.Value == null) ? null : Convert.ChangeType(filter.Value, t);
                    constant = Expression.Constant(safeValue);
                }
                var exp = GetExpression<T>(filter, par);
                var funcType = typeof(Func<,>).MakeGenericType(member.Type.GetTypeInfo()
                    .GetGenericArguments()[0], typeof(bool));

                var lam = Expression.Lambda(funcType, exp, par);
                var d = CallAny(member, lam);
                return d;
            }

            Type type = Type.GetType(member.Type.FullName) ?? GetEnumType(member.Type.FullName);
            var undlyType = type;
            bool isNullable = false;
            object value = null;

            if (isNullable = !(Nullable.GetUnderlyingType(type) is null))
                undlyType = Nullable.GetUnderlyingType(type);
            if (!filter.Value.IsList())
            {
                value = filter.Value.ConvertTo(undlyType, isNullable);
                if (value == null)
                    value = undlyType.GetDefaultValue();
            }
            else
            {
                var defaultValue = type.GetDefaultValue();
                value = filter.Value.CreateGenericList(defaultValue, isNullable);
            }

            if (filter.Operation != FilterOperation.IsNull && filter.Operation != FilterOperation.IsNotNull &&
                filter.Operation != FilterOperation.Any && filter.Operation != FilterOperation.NotAny &&
                filter.Operation != FilterOperation.In && filter.Operation != FilterOperation.NotIn)
            {
                constant = Expression.Constant(
                    Convert.ChangeType(value, undlyType), type);
            }
            else if (filter.Operation == FilterOperation.Any || filter.Operation == FilterOperation.NotAny)
            {
                //if (filter.Value == null)
                constant = Expression.Constant(true);
            }
            else if (filter.Operation == FilterOperation.In || filter.Operation == FilterOperation.NotIn)
            {
                constant = Expression.Constant(value, value.GetType());

                //     case nameof (Int64):
                //         {
                //             List<long> ints = new List<long> ();
                //             List<long?> intsNull = new List<long?> ();
                //             if (!isNull)
                //             {
                //                 //ints.AddRange (value.CreateGenericList<long> (value));
                //                 constant = Expression.Constant (ints, ints.GetType ());
                //             }
                //             else
                //             {
                //                 //intsNull.AddRange (value.CreateGenericList<long?> (value));
                //                 constant = Expression.Constant (intsNull, intsNull.GetType ());
                //             }

                //         }
                //         break;

                //     default:
                //         break;
                // }
                //values.AddRange(new List<Int32>(filter.Value))

            }
            else
            {
                constant = Expression.Constant(null);
            }

            MethodInfo inMethod = typeof(List<>).MakeGenericType(type)
                .GetTypeInfo()
                .GetMethods()
                .Where(m => m.Name == "Contains")
                .First();

            switch (filter.Operation)
            {
                case FilterOperation.Equals:
                    retVal = Expression.Equal(member, constant);
                    break;

                case FilterOperation.IsNull:
                    retVal = Expression.Equal(member, constant);
                    break;

                case FilterOperation.IsNotNull:
                    retVal = Expression.NotEqual(member, constant);
                    break;

                case FilterOperation.NotEqual:
                    retVal = Expression.NotEqual(member, constant);
                    break;

                case FilterOperation.GreaterThan:
                    retVal = Expression.GreaterThan(member, constant);
                    break;

                case FilterOperation.GreaterThanOrEqual:
                    retVal = Expression.GreaterThanOrEqual(member, constant);
                    break;

                case FilterOperation.LessThan:
                    retVal = Expression.LessThan(member, constant);
                    break;

                case FilterOperation.LessThanOrEqual:
                    retVal = Expression.LessThanOrEqual(member, constant);
                    break;

                case FilterOperation.Contains:
                    {
                        var methood = member.GetMethodByName("Contains");
                        if (methood != null)
                            retVal = Expression.Call(member, methood, constant);
                    }
                    break;

                case FilterOperation.StartsWith:
                    {
                        var method = member.GetMethodByName("StartsWith");
                        if (method != null)
                            retVal = Expression.Call(member, method, constant);
                    }
                    break;

                case FilterOperation.EndsWith:
                    {
                        var method = member.GetMethodByName("EndsWith");
                        if (method != null)
                            retVal = Expression.Call(member, method, constant);
                    }

                    break;

                case FilterOperation.Any:
                    var genericArguments = member.Type.GetTypeInfo().GetGenericArguments();
                    if (filter.Value == null && genericArguments.Length > 0)
                        retVal = CallAny(member, Expression.Lambda(constant,
                            Expression.Parameter(genericArguments[0], member.ToString().Replace('.', '_'))));
                    else if (genericArguments.Length > 0)
                    {

                        var parameter = Expression.Parameter(genericArguments[0], member.ToString()
                            .Replace('.', '_'));
                        var sf = Newtonsoft.Json.JsonConvert
                            .DeserializeObject<SearchFilter>(filter.Value.ToString());
                        var expr = new SearchFilterTranslator().Translate<T>(sf, parameter);
                        var predicate = Expression.Lambda(expr, parameter);
                        retVal = CallAny(member, predicate);
                    }
                    break;

                case FilterOperation.NotAny:
                    if (filter.Value == null)
                    {
                        retVal = Expression.Not(CallAny(member, Expression.Lambda(constant,
                            Expression.Parameter(member.Type.GetTypeInfo().GetGenericArguments()[0], member.ToString().Replace('.', '_')))));
                    }
                    else
                    {
                        var parameter = Expression.Parameter(member.Type.GetTypeInfo().GetGenericArguments()[0], member.ToString().Replace('.', '_'));
                        var sf = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilter>(filter.Value.ToString());
                        var expr = new SearchFilterTranslator().Translate<T>(sf, parameter);
                        var predicate = Expression.Lambda(expr, parameter);
                        retVal = Expression.Not(CallAny(member, predicate));
                    }
                    break;

                case FilterOperation.In:
                    retVal = Expression.Call(constant, inMethod, member);
                    break;

                case FilterOperation.NotIn:
                    retVal = Expression.Not(Expression.Call(constant, inMethod, member));
                    break;
            }

            return retVal;
        }

        #region any impl
        bool IsIEnumerable(Type type)
        {
            return type.GetTypeInfo().IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            Type[] t = type.GetTypeInfo().FindInterfaces((m, o) => IsIEnumerable(m), null);
            System.Diagnostics.Debug.Assert(t.Length == 1);
            return t[0];
        }
        Expression CallAny(Expression collection, Expression predicate)
        {
            Type cType = GetIEnumerableImpl(collection.Type);
            //collection = Expression.Convert(collection, cType);

            Type elemType = cType.GetGenericArguments()[0];
            Type predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

            Type[] typeArgs = new[] { elemType };
            int typeArity = typeArgs.Length;
            // Enumerable.Any<T>(IEnumerable<T>, Func<T,bool>)

            var methods = typeof(Enumerable).GetTypeInfo().GetMethods() // BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "Any")
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            var res = Expression.Call(methods.Last(), collection, predicate);

            return res;
        }
        #endregion

    }

    public static class ExpressionMethods
    {
        public static MethodInfo GetMethodByName(this MemberExpression m, string methodName)
        {
            return m.Type.GetMethods().Where(x => x.Name.Equals(methodName)).FirstOrDefault(); ;
        }
    }
}

