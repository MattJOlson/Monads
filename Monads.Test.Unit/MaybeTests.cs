﻿using System;
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

        [Test]
        public void dont_filter_Just_when_true()
        {
            var foo = Maybe.Return(42);

            foo.Filter(x => x % 2 == 0).Should().Be(Maybe.Return(42));
        }

        [Test]
        public void filter_Just_to_Nil_when_false()
        {
            var foo = Maybe.Return(42);

            foo.Filter(x => x % 2 == 1).Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void filter_idempotent_with_Nil()
        {
            var foo = Maybe<int>.Nil;

            foo.Filter(x => true).Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void use_linq_to_map_Just()
        {
            var foo = Maybe.Return(42);

            var res = from x in foo select x + 1;

            res.Should().Be(Maybe.Return(43));
        }

        [Test]
        public void use_linq_to_map_Nil()
        {
            var foo = Maybe<int>.Nil;

            var res = from x in foo select x + 1;

            res.Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void use_linq_for_filter_dont_filter_Just_when_true()
        {
            var foo = Maybe.Return(42);

            var res = from x in foo where x % 2 == 0 select x;

            res.Should().Be(Maybe.Return(42));
        }

        [Test]
        public void use_linq_for_filter_to_Nil_when_false()
        {
            var foo = Maybe.Return(42);

            var res = from x in foo where x % 2 == 1 select x;

            res.Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void use_linq_for_filter_idempotent_with_Nil()
        {
            var foo = Maybe<int>.Nil;

            var res = from x in foo where true select x;

            res.Should().Be(Maybe<int>.Nil);
        }

        [Test]
        public void use_linq_to_flatmap()
        {
            var foo = Maybe.Return(Maybe.Return(42));

            var res = from x in foo from y in x select y + 1;

            res.Should().Be(Maybe.Return(43));
        }

        [Test]
        public void use_linq_to_flatmap_nested_nil()
        {
            var foo = Maybe.Return(Maybe<int>.Nil);

            var res = from x in foo from y in x select y + 1;

            res.Should().Be(Maybe<int>.Nil);
        }
    }
}
