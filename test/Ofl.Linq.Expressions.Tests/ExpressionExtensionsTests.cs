using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Ofl.Linq.Expressions.Tests
{
    public class ExpressionExtensionsTests
    {
        // NOTE: Need this instead of anonymous types because we need to specify a type at some point
        // for the lambda expression routines.
        #region Test types.

        private class TestPropertyBase
        {
            public virtual string Property { get; } = "Hello from test base.";
        }

        private class TestPropertyExtended : TestPropertyBase
        {
            public override string Property { get; } = "Hello from test extended.";
        }

        private class Test
        {
            public TestPropertyExtended Property { get; } = new TestPropertyExtended();
        }

        #endregion

        #region Tests.

        [Fact]
        public void Test_UseCreateGetPropertyExpressionInOtherExpression()
        {
            // Get the property info.
            PropertyInfo propertyInfo = typeof(Test).GetProperty(nameof(Test.Property));

            // The parameter expression.
            var parameterExpression = Expression.Parameter(typeof(Test), "_" + Guid.NewGuid().ToString("N"));

            // Get the expression.  It's a member expression.
            var propertyExpression = propertyInfo.CreateGetPropertyExpression(parameterExpression);

            // Create an expression which compares it to null.
            var notEqualExpression =
                Expression.NotEqual(propertyExpression, Expression.Constant(null, propertyInfo.PropertyType));

            // Create the lambda, compile, and run.
            Func<Test, bool> func = Expression.Lambda<Func<Test, bool>>(notEqualExpression, parameterExpression)
                .Compile();

            // Run.
            Assert.True(func(new Test()));
        }

        [Fact]
        public void Test_CreateGetPropertyExpression()
        {
            // Get the property info.
            PropertyInfo propertyInfo = typeof(Test).GetProperty(nameof(Test.Property));

            // Get the expression.  It's a member expression.
            var expression = propertyInfo.CreateGetPropertyExpression();

            // It's not null.
            Assert.NotNull(expression);

            // The member is the property expression.
            Assert.Equal(propertyInfo, expression.Member);
        }

        [Fact]
        public void CreateGetPropertyLambdaExpression_Invariant()
        {
            // Get the property info.
            PropertyInfo propertyInfo = typeof(Test).GetProperty(nameof(Test.Property));

            // Get the expression.  It's a member expression.
            var expression = propertyInfo.CreateGetPropertyLambdaExpression<Test, TestPropertyExtended>();

            // Create the instance.
            var expected = new Test();

            // Compile.
            Func<Test, TestPropertyExtended> lambda = expression.Compile();

            // Run.
            TestPropertyExtended actual = lambda(expected);

            // Assert.
            Assert.Equal(actual, expected.Property);
        }

        [Fact]
        public void CreateGetPropertyLambdaExpression_Contravariant()
        {
            // Get the property info.
            PropertyInfo propertyInfo = typeof(Test).GetProperty(nameof(Test.Property));

            // Get the expression.  It's a member expression.
            var expression = propertyInfo.CreateGetPropertyLambdaExpression<Test, TestPropertyBase>();

            // Create the instance.
            var expected = new Test();

            // Compile.
            Func<Test, TestPropertyBase> lambda = expression.Compile();

            // Run.
            TestPropertyBase actual = lambda(expected);

            // Assert.
            Assert.Equal(actual, expected.Property);
        }

        [Fact]
        public void CreateGetPropertyLambdaExpression_Object()
        {
            // Get the property info.
            PropertyInfo propertyInfo = typeof(Test).GetProperty(nameof(Test.Property));

            // Get the expression.  It's a member expression.
            var expression = propertyInfo.CreateGetPropertyLambdaExpression<Test, object>();

            // Create the instance.
            var expected = new Test();

            // Compile.
            Func<Test, object> lambda = expression.Compile();

            // Run.
            object actual = lambda(expected);

            // Assert.
            Assert.Equal(actual, expected.Property);
        }

        #endregion
    }
}
