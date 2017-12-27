using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ofl.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        private static ParameterExpression CreateParameterExpression(this Type type)
        {
            // Validate parameters.
            if (type == null) throw new ArgumentNullException(nameof(type));

            // Create the expression and return.
            return Expression.Parameter(type, "o");
        }

        public static MemberExpression CreateGetPropertyExpression(this PropertyInfo propertyInfo) => propertyInfo.CreateGetPropertyExpression(
            propertyInfo.DeclaringType.CreateParameterExpression());

        public static MemberExpression CreateGetPropertyExpression(this PropertyInfo propertyInfo, ParameterExpression parameterExpression)
        {
            // Validate parameters.
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));
            if (parameterExpression == null) throw new ArgumentNullException(nameof(parameterExpression));

            // Validate the parameter expression.
            if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(parameterExpression.Type.GetTypeInfo()))
                // Throw.
                throw new InvalidOperationException($"The type of { nameof(parameterExpression) } ({ parameterExpression.Type } " +
                    $"is not assignable to the type of { nameof(propertyInfo.DeclaringType) } " +
                    $"({ propertyInfo.DeclaringType }) passed in the { nameof(propertyInfo) } parameter.");

            // Access the property.
            MemberExpression propertyExpression = Expression.Property(parameterExpression, propertyInfo);

            // Return the property expression.
            return propertyExpression;
        }

        public static Expression<Func<T, TProperty>> CreateGetPropertyLambdaExpression<T, TProperty>(this PropertyInfo propertyInfo) =>
            propertyInfo.CreateGetPropertyLambdaExpression<T, TProperty>(typeof(T).CreateParameterExpression());

        public static Expression<Func<T, TProperty>> CreateGetPropertyLambdaExpression<T, TProperty>(this PropertyInfo propertyInfo,
            ParameterExpression parameterExpression)
        {
            // Validate parameters.
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));
            if (parameterExpression == null) throw new ArgumentNullException(nameof(parameterExpression));

            // Get the property expression.
            MemberExpression propertyExpression = propertyInfo.CreateGetPropertyExpression(parameterExpression);

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
