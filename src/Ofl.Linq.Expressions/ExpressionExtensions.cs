using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ofl.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static MemberExpression CreateGetPropertyExpression(this PropertyInfo propertyInfo)
        {
            // Validate parameters.
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            // Create the parameter expression.
            ParameterExpression parameterExpression = Expression.Parameter(propertyInfo.DeclaringType, "o");

            // Access the property.
            MemberExpression propertyExpression = Expression.Property(parameterExpression, propertyInfo);

            // Return the property expression.
            return propertyExpression;
        }

        public static Expression<Func<T, TProperty>> CreateGetPropertyLambdaExpression<T, TProperty>(this PropertyInfo propertyInfo)
        {
            // Validate parameters.
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            // Get the property expression.
            MemberExpression propertyExpression = propertyInfo.CreateGetPropertyExpression();

            // The expression.
            Expression expression = propertyExpression;

            // The property type.
            Type type = typeof(TProperty);

            // If the property info type is not equal to the property type, use convert.
            if (propertyInfo.PropertyType != type)
                // Convert.
                expression = Expression.Convert(expression, type);            

            // Package in a lambda.
            return Expression.Lambda<Func<T, TProperty>>(expression, propertyExpression.Expression as ParameterExpression);
        }
    }
}
