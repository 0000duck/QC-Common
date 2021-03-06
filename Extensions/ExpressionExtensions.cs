﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Extensions
{
    /// <summary>Defines extension methods for building and working with Expressions.</summary>
    public static class ExpressionExtensions
    {
        /// <summary>Ands the Expressions.</summary>
        /// <typeparam name="T">The target type of the Expression.</typeparam>
        /// <param name="expressions">The Expression(s) to and.</param>
        /// <returns>A new Expression.</returns>
        public static Expression<Func<T, bool>> And<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            if (expressions.IsNullOrEmpty())
                return null;

            Expression<Func<T, bool>> finalExpression = expressions.First();

            foreach (Expression<Func<T, bool>> e in expressions.Skip(1))
                finalExpression = finalExpression.And(e);

            return finalExpression;
        }

        /// <summary>Ors the Expressions.</summary>
        /// <typeparam name="T">The target type of the Expression.</typeparam>
        /// <param name="expressions">The Expression(s) to or.</param>
        /// <returns>A new Expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            if (expressions.IsNullOrEmpty())
                return null;

            Expression<Func<T, bool>> finalExpression = expressions.First();

            foreach (Expression<Func<T, bool>> e in expressions.Skip(1))
                finalExpression = finalExpression.Or(e);

            return finalExpression;
        }

        /// <summary>Ands the Expression with the provided Expression.</summary>
        /// <typeparam name="T">The target type of the Expression.</typeparam>
        /// <param name="expression1">The left Expression to and.</param>
        /// <param name="expression2">The right Expression to and.</param>
        /// <returns>A new Expression.</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            //Reuse the first expression's parameter
            ParameterExpression param = expression1.Parameters.Single();
            Expression left = expression1.Body;
            Expression right = RebindParameter(expression2.Body, expression2.Parameters.Single(), param);
            BinaryExpression body = Expression.AndAlso(left, right);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>Ors the Expression with the provided Expression.</summary>
        /// <typeparam name="T">The target type of the Expression.</typeparam>
        /// <param name="expression1">The left Expression to or.</param>
        /// <param name="expression2">The right Expression to or.</param>
        /// <returns>A new Expression.</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            //Reuse the first expression's parameter
            ParameterExpression param = expression1.Parameters.Single();
            Expression left = expression1.Body;
            Expression right = RebindParameter(expression2.Body, expression2.Parameters.Single(), param);
            BinaryExpression body = Expression.OrElse(left, right);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>Updates the supplied expression using the appropriate parameter.</summary>
        /// <param name="expression">The expression to update.</param>
        /// <param name="oldParameter">The original parameter of the expression.</param>
        /// <param name="newParameter">The target parameter of the expression.</param>
        /// <returns>The updated expression.</returns>
        private static Expression RebindParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    {
                        ParameterExpression parameterExpression = (ParameterExpression)expression;

                        return (parameterExpression.Name == oldParameter.Name ? newParameter : parameterExpression);
                    }
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression memberExpression = (MemberExpression)expression;

                        return memberExpression.Update(RebindParameter(memberExpression.Expression, oldParameter, newParameter));
                    }
                case ExpressionType.Negate:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.Quote:
                    {
                        UnaryExpression unaryExpression = (UnaryExpression)expression;

                        return unaryExpression.Update(RebindParameter(unaryExpression.Operand, oldParameter, newParameter));
                    }
                case ExpressionType.Coalesce:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    {
                        BinaryExpression binaryExpression = (BinaryExpression)expression;

                        return binaryExpression.Update(RebindParameter(binaryExpression.Left, oldParameter, newParameter), binaryExpression.Conversion, RebindParameter(binaryExpression.Right, oldParameter, newParameter));
                    }
                case ExpressionType.Call:
                    {
                        MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

                        return methodCallExpression.Update(RebindParameter(methodCallExpression.Object, oldParameter, newParameter), methodCallExpression.Arguments.Select(arg => RebindParameter(arg, oldParameter, newParameter)));
                    }
                case ExpressionType.Invoke:
                    {
                        InvocationExpression invocationExpression = (InvocationExpression)expression;

                        return invocationExpression.Update(RebindParameter(invocationExpression.Expression, oldParameter, newParameter), invocationExpression.Arguments.Select(arg => RebindParameter(arg, oldParameter, newParameter)));
                    }
                case ExpressionType.Lambda:
                    {
                        LambdaExpression lambdaExpression = (LambdaExpression)expression;

                        return Expression.Lambda(RebindParameter(lambdaExpression.Body, oldParameter, newParameter), lambdaExpression.Parameters);
                    }
                default:
                    {
                        return expression;
                    }
            }
        }

        public static Expression<Func<T, bool>> BuildContainsExpression<T, R>(this Expression<Func<T, R>> valueSelector, IEnumerable<R> values)
        {
            if (valueSelector == null)
                throw new ArgumentNullException("valueSelector");

            if (values.IsNullOrEmpty())
                return (e => false);

            ParameterExpression parameterExpression = valueSelector.Parameters.Single();
            IEnumerable<BinaryExpression> equalExpressions = null;
            Expression aggregationExpression = null;

            equalExpressions = values.Select(v => Expression.Equal(valueSelector.Body, Expression.Constant(v, typeof(R))));
            aggregationExpression = equalExpressions.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<T, bool>>(aggregationExpression, parameterExpression);
        }

        public static Expression<Func<T, bool>> BuildDoesNotContainExpression<T, R>(this Expression<Func<T, R>> valueSelector, IEnumerable<R> values)
        {
            if (valueSelector == null)
                throw new ArgumentNullException("valueSelector");

            if (values.IsNullOrEmpty())
                return (e => false);

            ParameterExpression parameterExpression = valueSelector.Parameters.Single();
            IEnumerable<BinaryExpression> notEqualExpressions = null;
            Expression aggregationExpression = null;

            notEqualExpressions = values.Select(v => Expression.NotEqual(valueSelector.Body, Expression.Constant(v, typeof(R))));
            aggregationExpression = notEqualExpressions.Aggregate<Expression>((accumulate, equal) => Expression.And(accumulate, equal));

            return Expression.Lambda<Func<T, bool>>(aggregationExpression, parameterExpression);
        }

        public static string GetMemberName<T, V>(this Expression<Func<T, V>> expression)
        {
            return GetMemberName(expression as LambdaExpression);
        }

        public static string GetMemberName(this LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            try
            {
                string memberName = expression.Coalesce(
                    e => (e.Body as MemberExpression).ValueOrDefault(m => m.Member.Name),
                    e => (e.Body as UnaryExpression).ValueOrDefault(u => (u.Operand as MemberExpression).ValueOrDefault(m => m.Member.Name)));

                if (memberName == null)
                    throw new InvalidOperationException("Could not determine the member name from the expression.");

                return memberName;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not determine the column name from the expression. In order to automatically determine the column name, the selector must be simple, for instance, \"o => o.Firstname\".", ex);
            }
        }
    }
}