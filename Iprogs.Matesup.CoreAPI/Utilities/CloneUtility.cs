using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Iprogs.Matesup.CoreAPI.Utilities
{
    public static class CloneUtility
    {
        public static T DeepClone<T>(T obj)
        {
            try
            {
                return (T)CloneUtility.DeepClone((object)obj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object DeepClone(object obj)
        {
            try
            {
                if (obj == null)
                    return (object)null;
                Type type = obj.GetType();
                if (CloneUtility.IsArray(type))
                    return CloneUtility.CloneIArray(obj);
                if (CloneUtility.IsIList(type))
                    return CloneUtility.CloneIList(obj);
                if (CloneUtility.IsIDictionary(type))
                    return CloneUtility.CloneIDictionary(obj);
                if (CloneUtility.IsEnum(type))
                    return CloneUtility.CloneEnum(obj);
                if (CloneUtility.IsSimpleType(type))
                    return CloneUtility.CloneSimpleType(obj);
                if (CloneUtility.IsClass(type))
                    return CloneUtility.CloneClass(obj);
                if (CloneUtility.IsTimespan(type))
                    return CloneUtility.CloneTimespan(obj);
                if (CloneUtility.IsStruct(type))
                    return CloneUtility.CloneStruct(obj);
                throw new NotImplementedException("Cannot compare object of type " + type.Name);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static object CloneIDictionary(object obj)
        {
            object instance = Activator.CreateInstance(obj.GetType());
            IDictionary dictionary1 = obj as IDictionary;
            IDictionary dictionary2 = instance as IDictionary;
            IDictionaryEnumerator enumerator = dictionary1.GetEnumerator();
            while (enumerator.MoveNext())
                dictionary2.Add(CloneUtility.DeepClone(enumerator.Key), CloneUtility.DeepClone(enumerator.Value));
            return instance;
        }

        private static object CloneIList(object obj)
        {
            object instance = Activator.CreateInstance(obj.GetType(), (object)2);
            IList list1 = obj as IList;
            IList list2 = instance as IList;
            foreach (object obj1 in (IEnumerable)list1)
                list2.Add(CloneUtility.DeepClone(obj1));
            return instance;
        }

        private static object CloneIArray(object obj)
        {
            object instance = Activator.CreateInstance(obj.GetType(), (object)((Array)obj).Length);
            IList list = obj as IList;
            Array array = instance as Array;
            IEnumerator enumerator = list.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                array.SetValue(CloneUtility.DeepClone(enumerator.Current), index);
                ++index;
            }
            return instance;
        }

        private static object CloneEnum(object obj)
        {
            return obj;
        }

        private static object CloneSimpleType(object obj)
        {
            return obj;
        }

        private static object CloneClass(object obj)
        {
            Type type = obj.GetType();
            object instance = Activator.CreateInstance(type);
            CloneUtility.PerformCloneProperties(type, obj, instance);
            CloneUtility.PerformCloneFields(type, obj, instance);
            return instance;
        }

        private static void PerformCloneProperties(Type t1, object obj, object resultValue)
        {
            foreach (PropertyInfo property in t1.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.CanRead)
                {
                    object obj1 = property.GetValue(obj, (object[])null);
                    if (property.CanWrite)
                        property.SetValue(resultValue, CloneUtility.DeepClone(obj1), (object[])null);
                }
            }
        }

        private static void PerformCloneFields(Type t1, object obj, object resultValue)
        {
            foreach (FieldInfo field in t1.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object obj1 = field.GetValue(obj);
                field.SetValue(resultValue, CloneUtility.DeepClone(obj1));
            }
        }

        private static object CloneTimespan(object obj)
        {
            return obj;
        }

        private static object CloneStruct(object obj)
        {
            Type type = obj.GetType();
            object instance = Activator.CreateInstance(type);
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (CloneUtility.ValidStructSubType(property.PropertyType))
                    property.SetValue(instance, CloneUtility.DeepClone(property.GetValue(obj, (object[])null)), (object[])null);
            }
            return instance;
        }

        private static bool IsTimespan(Type t)
        {
            return t == typeof(TimeSpan);
        }

        private static bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        private static bool IsStruct(Type t)
        {
            return t.IsValueType;
        }

        private static bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || t == typeof(DateTime) || (t == typeof(Decimal) || t == typeof(string)) || t == typeof(Guid);
        }

        private static bool ValidStructSubType(Type t)
        {
            return CloneUtility.IsSimpleType(t) || CloneUtility.IsEnum(t) || (CloneUtility.IsArray(t) || CloneUtility.IsClass(t)) || (CloneUtility.IsIDictionary(t) || CloneUtility.IsTimespan(t)) || CloneUtility.IsIList(t);
        }

        private static bool IsArray(Type t)
        {
            return t.IsArray;
        }

        private static bool IsClass(Type t)
        {
            return t.IsClass;
        }

        private static bool IsIDictionary(Type t)
        {
            return t.GetInterface("System.Collections.IDictionary", true) != (Type)null;
        }

        private static bool IsIList(Type t)
        {
            return t.GetInterface("System.Collections.IList", true) != (Type)null;
        }

        private static bool IsChildType(Type t)
        {
            return CloneUtility.IsClass(t) || CloneUtility.IsArray(t) || (CloneUtility.IsIDictionary(t) || CloneUtility.IsIList(t)) || CloneUtility.IsStruct(t);
        }
    }
}