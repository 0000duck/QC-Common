using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QuantumConcepts.Common.Extensions {
    public static class LinqExtensions {
        public static TEntity CreateIfNew<TEntity>(this ITable<TEntity> source, Func<TEntity, Expression<Func<TEntity, bool>>> selector, TEntity newItem) where TEntity : class {
            if (!source.Any(selector(newItem)))
            {
                source.InsertOnSubmit(newItem);
            }

            return newItem;
        }
    }
}
