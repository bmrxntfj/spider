using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Spider
{
    public static class ReflectionExtension
    {
        public static T GetCustomAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return (T)type.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this FieldInfo field, bool inherit = true) where T : Attribute
        {
            return (T)field.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }
        public static T GetCustomAttribute<T>(this MethodInfo method, bool inherit = true) where T : Attribute
        {
            return (T)method.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this PropertyInfo property, bool inherit = true) where T : Attribute
        {
            return (T)property.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this ParameterInfo parameter, bool inherit = true) where T : Attribute
        {
            return (T)parameter.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();
        }

        public static T[] GetCustomAttributes<T>(this Type type, bool inherit = true) where T : Attribute
        {
            return (T[])type.GetCustomAttributes(typeof(T), inherit);
        }

        public static T[] GetCustomAttributes<T>(this MethodInfo method, bool inherit = true) where T : Attribute
        {
            return (T[])method.GetCustomAttributes(typeof(T), inherit);
        }

        public static bool IsInherit(this Type type, Type baseType)
        {
            return type.IsSubclassOf(baseType)
                || type.GetInterfaces().Contains(baseType)
                || (baseType.IsGenericType && type.GetInterfaces().Where(c => c.IsGenericType).Select(c => c.GetGenericTypeDefinition()).Contains(baseType));
        }

        static List<Type> PrimitiveTypes = new List<Type> {
            typeof(byte), typeof(byte?),
            typeof(sbyte), typeof(sbyte?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?),
            typeof(decimal), typeof(decimal?),
            typeof(char), typeof(char?),
            typeof(bool), typeof(bool?),
            typeof(float), typeof(float?),
            typeof(double), typeof(double?),
            typeof(bool), typeof(bool?),
            typeof(DateTime), typeof(DateTime?),
            typeof(DateTimeOffset), typeof(DateTimeOffset?),
            typeof(Guid), typeof(Guid?),typeof(string),
            typeof(byte[])
        };

        public static bool IsPrimitiveType(this Type type)
        {
            return PrimitiveTypes.Contains(type);
        }

    }
}
