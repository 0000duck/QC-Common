using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QuantumConcepts.Common.Extensions {
    public static class LinqExtensions {
        public static TEntity CreateIfNew<TEntity>(this ITable<TEntity> source, Func<TEntity, Expression<Func<TEntity, bool>>> selector, TEntity newItem) where TEntity : class {
            //TKey newItemField = fieldSelector(newItem);
            //Type entityType = typeof(TEntity);
            //Type keyType = typeof(TKey);
            //Type selectorType = typeof(Func<TEntity, TKey>);
            //ConstantExpression constant = Expression.Constant(newItemField);
            //ParameterExpression entityParameter = Expression.Parameter(entityType, "o");
            //MethodCallExpression call = Expression.Call(fieldSelector, entityParameter);
            ////LambdaExpression lambda = Expression.Lambda<Func<TEntity, TKey>>(call);
            //BinaryExpression equal = Expression.Equal(constant, call);
            //Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(equal, entityParameter);



            /*
            Expression <Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Constant(newItemField),
                    Expression.Lambda<Func<TEntity, TKey>>(Expression.Call(fieldSelector.Method, Expression.Parameter(entityType, "o")))),
                Expression.Parameter(entityType, "o"));*/

            if (!source.Any(selector(newItem)))
                source.InsertOnSubmit(newItem);

            return newItem;
        }
    }
}
