using System;
using System.Collections.Generic;
using System.Linq;

namespace VMLab.Contract.Helpers
{
    public static class DictionaryExtensions
    {
        public static IDictionary<T, T2> RemovalAll<T, T2>(this IDictionary<T, T2> target,
            Func<KeyValuePair<T, T2>, bool> action)
        {
            var removelist = (from item in target where action(item) select item.Key).ToList();

            foreach (var item in removelist)
                target.Remove(item);

            return target;
        }
    }
}
