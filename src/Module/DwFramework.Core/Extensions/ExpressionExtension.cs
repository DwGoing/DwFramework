using System;
using System.Linq.Expressions;

namespace DwFramework.Core.Extensions
{
    public static class ExpressionExtension
    {
        private class SetParamExpressionVisitor : ExpressionVisitor
        {
            public ParameterExpression Parameter { get; set; }

            public SetParamExpressionVisitor() { }

            public SetParamExpressionVisitor(ParameterExpression parameter)
            {
                Parameter = parameter;
            }

            public Expression Modify(Expression exp)
            {
                return Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression parameter)
            {
                return Parameter;
            }
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return exp1.Compose(exp2, Expression.AndAlso);
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2)
        {
            return exp1.Compose(exp2, Expression.OrElse);
        }
        public static Expression<Func<T, bool>> Compose<T>(this Expression<Func<T, bool>> exp1, Expression<Func<T, bool>> exp2, Func<Expression, Expression, Expression> merge)
        {
            var parameter = Expression.Parameter(typeof(string), "name");
            SetParamExpressionVisitor visitor = new SetParamExpressionVisitor(parameter);
            var newExp1 = visitor.Modify(exp1.Body);
            var newExp2 = visitor.Modify(exp2.Body);
            var newBodyExp = merge(newExp1, newExp2);
            return Expression.Lambda<Func<T, bool>>(newBodyExp, parameter);
        }
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> exp)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(exp.Body), exp.Parameters[0]);
        }
    }
}
