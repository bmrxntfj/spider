using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spider
{
    public static class InfrastructureExtension
    {
        public static decimal SumOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            if (source.Any())
            {
                return source.Sum(selector);
            }
            return 0;
        }
        public static decimal SumOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            if (source.Any())
            {
                var a = source.Sum(selector);
                return a.HasValue ? a.Value : 0m;
            }
            return 0m;
        }
        public static int SumOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            if (source.Any())
            {
                var a = source.Sum(selector);
                return a.HasValue ? a.Value : 0;

            }
            return 0;
        }
        public static int SumOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            if (source.Any())
            {
                var a = source.Sum(selector);
                return a;
            }
            return 0;
        }
        public static string Substr(this string s, int length)
        {
            return s.Substr(0, length);
        }
        public static string Substr(this string s, int start, int length)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.Length - start < length ? s.Substring(start) : s.Substring(start, length);
        }
        public static string Neaten(this string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.Trim();
        }
        public static string Neaten(this string s, params char[] trimChars)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.Trim(trimChars);
        }
        public static string NeatenStart(this string s, params char[] trimChars)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.TrimStart(trimChars);
        }
        public static string NeatenEnd(this string s, params char[] trimChars)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.TrimEnd(trimChars);
        }
        public static string RemoveBlank(this string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return s.Replace(" ", string.Empty);
        }
        public static Match Match(this string s, string pattern)
        {
            return new Regex(pattern).Match(s);
        }
        public static MatchCollection Matchs(this string s, string pattern)
        {
            return new Regex(pattern).Matches(s);
        }
        
        public static DateTime Tomorrow(this DateTime datetime)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);
        }
        public static DateTime Yesterday(this DateTime datetime)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-1);
        }
        public static bool In(this DateTime datetime, DateTime start, DateTime end)
        {
            return datetime >= start && datetime <= end;
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) { return; }
            foreach (var s in source) { action(s); }
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null) { return; }
            var i = 0;
            foreach (var s in source) { action(s, i); i++; }
        }
        public static void ForEach<T>(this IEnumerator source, Action<T> action)
        {
            if (source == null) { return; }
            while (source.MoveNext())
            {
                action((T)source.Current);
            }
        }
        public static void ForEach<T>(this IEnumerable source, Action<T> action)
        {
            if (source == null) { return; }
            var enumer = source.GetEnumerator();
            while (enumer.MoveNext())
            {
                action((T)enumer.Current);
            }
        }
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>> action)
        {
            if (source == null) { return; }
            foreach (var s in source) { action(s); }
        }
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> source, Action<KeyValuePair<TKey, TValue>, int> action)
        {
            if (source == null) { return; }
            var i = 0;
            foreach (var s in source) { action(s, i); i++; }
        }
        public static void Clear<T>(this ConcurrentQueue<T> quene)
        {
            while (quene.Count > 0)
            {
                var t = default(T);
                quene.TryDequeue(out t);
            }
        }
        public static bool IsAnonymousType(this Type type) { return type.Name.StartsWith("<>f__AnonymousType"); }
        public static void For(this int times, Action<int> callback)
        {
            for (var i = 0; i < times; i++)
            {
                callback(i);
            }
        }
        public static void For(this double times, Action<int> callback)
        {
            for (var i = 0; i < times; i++)
            {
                callback(i);
            }
        }
        public static string ToTrace(this Exception e)
        {
            if (e == null) { return string.Empty; }
            var logBuilder = new StringBuilder();
            Exception ex = e;
            while (ex != null)
            {
                logBuilder.AppendLine(ex.ToString());
                ex = ex.InnerException;
            }
            return logBuilder.ToString();
        }
        public static string Join(this IEnumerable<string> s, string separator)
        {
            return string.Join(separator, s);
        }
    }
}
