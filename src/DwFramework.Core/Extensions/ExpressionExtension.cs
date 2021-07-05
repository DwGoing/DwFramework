using System;
using System.Linq.Expressions;

namespace DwFramework.Core
{
    public static class ExpressionExtension
    {
        private class SetParamExpressionVisitor : ExpressionVisitor
        {
            public ParameterExpression Parameter { get; set; }

            public SetParamExpressionVisitor() { }

            public SetParamExpressionVisitor(ParameterExpression parameter) => Parameter = parameter;

            public Expression Modify(Expression exp) => Visit(exp);

            protected override Expression VisitParameter(ParameterExpression parameter) => Parameter;
        }

        /// <summary>
        /// 合并成新的表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Merge<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2, Func<Expression, Expression, Expression> merge)
        {
            var parameter = Expression.Parameter(typeof(T), "parameter");
            var visitor = new SetParamExpressionVisitor(parameter);
            var newExp1 = visitor.Modify(exp1.Body);
            var newExp2 = visitor.Modify(exp2.Body);
            var newBodyExp = merge(newExp1, newExp2);
            return Expression.Lambda<Func<T, bool>>(newBodyExp, parameter);
        }

        /// <summary>
        /// AND操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return exp1.Merge(exp2, Expression.AndAlso);
        }

        /// <summary>
        /// OR操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return exp1.Merge(exp2, Expression.OrElse);
        }

        /// <summary>
        /// NOT操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> exp)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(exp.Body), exp.Parameters[0]);
        }
    }
}
