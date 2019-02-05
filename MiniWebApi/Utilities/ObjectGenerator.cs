using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MiniWebApi.Utilities
{
    static class ObjectGenerator
    {
        private static readonly Dictionary<Type, Func<object>> Creators = new Dictionary<Type, Func<object>>();
        private static readonly Dictionary<Type,Tuple<PropertyInfo[], Action<object, object[]>>> Setters = new Dictionary<Type, Tuple<PropertyInfo[], Action<object, object[]>>>();

        /// <summary>
        /// Create new object by using Emit, it will be much faster than the reflection.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns>The created object.</returns>
        public static object New(this Type type)
        {
            if (!Creators.ContainsKey(type))
            {
                var defaultConstructor = type.GetConstructor(new Type[] { });
                if (defaultConstructor == null)
                {
                    throw new InvalidOperationException($"No default constructor for type:{type}");
                }
                var dynamicMethod = new DynamicMethod(
                    name: $"Creare_{type.Name}",
                    returnType: typeof(object),
                    parameterTypes: null);

                var il = dynamicMethod.GetILGenerator();
                il.Emit(OpCodes.Newobj, defaultConstructor);
                il.Emit(OpCodes.Ret);
                var creator = (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
                Creators.Add(type,creator);
            }
            return Creators[type]();
        }


        /// <summary>
        /// Create new object by using Emit, it will be much faster than the reflection.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="parameters">The key value of the parameters, they will be converted according to the types.</param>
        /// <returns>The created object.</returns>
        public static object New(this Type type, NameValueCollection parameters)
        {
            if (!Setters.ContainsKey(type))
            {
                var properties = type.GetProperties().Where(x => x.CanRead && x.CanWrite).ToArray();
                var methodList = new List<MethodInfo>();
                foreach (var property in properties)
                {
                    var method = type.GetMethod($"set_{property.Name}");
                    methodList.Add(method);
                }
                var dynamicMethod = new DynamicMethod(
                    name: $"Set_{type.Name}_Parameters",
                    returnType: null,
                    parameterTypes: new[] { typeof(object), typeof(object[]) });

                var il2 = dynamicMethod.GetILGenerator();

                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    il2.Emit(OpCodes.Ldarg_0);
                    il2.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
                    il2.Emit(OpCodes.Ldarg_1);
                    //Put an integer value in stack which is the index in object[]
                    il2.Emit(OpCodes.Ldc_I4_S, i);
                    //Get the reference of index which is the integer value from the object[].
                    il2.Emit(OpCodes.Ldelem_Ref);
                    //Cast or unbox the reference.
                    il2.Emit(property.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, property.PropertyType);
                    il2.Emit(OpCodes.Callvirt, methodList[i]);
                }
                il2.Emit(OpCodes.Ret);
                var setter = (Action<object, object[]>)dynamicMethod.CreateDelegate(typeof(Action<object, object[]>));
                Setters.Add(type, new Tuple<PropertyInfo[], Action<object, object[]>>(properties, setter));
            }
            var obj = type.New();
            var propertyArray = Setters[type].Item1;
            var args = new List<object>();
            foreach (var property in propertyArray)
            {
                if (parameters.AllKeys.Contains(property.Name))
                {
                    var propertyValue = WebApiConverter.StringToObject(parameters[property.Name], property.PropertyType);
                    if (propertyValue != null && property.PropertyType == typeof(string))
                    {
                        propertyValue = Uri.UnescapeDataString((string)propertyValue);
                    }
                    args.Add(propertyValue);
                }
                else
                {
                    args.Add(property.PropertyType.Default());
                }
            }
            Setters[type].Item2(obj, args.ToArray());
            return obj;
        }


        /// <summary>
        /// Get the default value for value type, null for reference type.
        /// </summary>
        /// <param name="type">The type for the value.</param>
        /// <returns>The default value.</returns>
        public static object Default(this Type type)
        {
            if (!type.IsValueType) return null;
            if (type == typeof(byte))
            {
                return default(byte);
            }
            if (type == typeof(sbyte))
            {
                return default(sbyte);
            }
            if (type == typeof(short))
            {
                return default(short);
            }
            if (type == typeof(int))
            {
                return default(int);
            }
            if (type == typeof(float))
            {
                return default(float);
            }
            if (type == typeof(long))
            {
                return default(long);
            }
            if (type == typeof(double))
            {
                return default(double);
            }
            if (type == typeof(ushort))
            {
                return default(ushort);
            }
            if (type == typeof(uint))
            {
                return default(uint);
            }
            if (type == typeof(ulong))
            {
                return default(ulong);
            }
            if (type == typeof(char))
            {
                return default(char);
            }
            if (type == typeof(decimal))
            {
                return default(decimal);
            }
            if (type == typeof(bool))
            {
                return default(bool);
            }
            if (type.IsEnum)
            {
                return type.GetEnumValues().GetValue(0);
            }
            return type.New();
        }
    }
}
