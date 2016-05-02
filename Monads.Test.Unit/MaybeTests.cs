using System;
using FluentAssertions;
using NUnit.Framework;

namespace Monads.Test.Unit
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void maybe_is_nil_by_default()
        {
            var nothing = new Maybe<int>();

            nothing.Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void returning_x_as_a_maybe_results_in_just_x()
        {
            var x = Maybe.Return(42);

            x.Should().Be(Maybe<int>.Just(42));
        }

        [Test]
        public void nothing_callback_should_be_called_on_default()
        {
            var nil = Maybe<int>.Nil;

            nil.Nothing(() => 42)
               .Just(i => i)
               .Should().Be(42);
        }

        [Test]
        public void just_callback_should_be_called_with_value()
        {
            var x = Maybe.Return(42);

            x.Nothing(() => 19)
             .Just(i => i)
             .Should().Be(42);
        }

        [Test]
        public void bind_lambda_returns_nil_when_given_nil()
        {
            Func<bool, Maybe<int>> lambda = b => b ? Maybe.Return(42) : Maybe.Return(19);

            Maybe.Bind(lambda)(Maybe<bool>.Nil).Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void bind_lambda_returns_f_of_x_when_given_just_x()
        {
            Func<bool, Maybe<int>> lambda = b => b ? Maybe.Return(42) : Maybe<int>.Nil;

            Maybe.Bind(lambda)(Maybe.Return(true)).Should().Be(Maybe.Return(42));
        }

        [Test]
        public void when_a_lifted_function_is_passed_Nil_it_returns_Nil()
        {
            var foo = Maybe<int>.Nil;

            foo.Lift(i => i + 1).Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void when_a_lifted_function_is_passed_just_x_it_returns_just_f_of_x()
        {
            var foo = Maybe.Return(42);

            foo.Lift(i => i + 1).Should().Be(Maybe.Return(43));
        }
    }
}
