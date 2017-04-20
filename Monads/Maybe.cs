using System;
using System.ComponentModel.Design;

namespace Monads
{
    public struct Maybe<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        private Maybe(T val, bool hasVal)
        {
            _value = val;
            _hasValue = hasVal;
        }

        public static Maybe<T> Nil => new Maybe<T>();
        public static Maybe<T> Just(T x) => new Maybe<T>(x, true);

        public MaybeNothing<T, Tres> Nothing<Tres>(Func<Tres> nothing)
        {
            return new MaybeNothing<T, Tres>(this, nothing);
        }

        internal Tres Callbacks<Tres>(Func<Tres> nothing, Func<T, Tres> just)
        {
            if (_hasValue)
                return just(_value);

            return nothing();
        }

        public Maybe<Tres> Lift<Tres>(Func<T, Tres> func)
        {
            if (_hasValue)
                return Maybe.Return(func(_value));

            return Maybe<Tres>.Nil;
        }

        public Maybe<Tres> Then<Tres>(Func<T, Tres> func)
        {
            return Lift(func);
        }

        public Maybe<T> Filter(Func<T,Boolean> func)
        {
            return _hasValue ? (func(_value) ? this : Nil) : Nil;
        }
    }

    public class MaybeNothing<Tm, Tres>
    {
        private readonly Maybe<Tm> _m;
        private readonly Func<Tres> _nothing;

        internal MaybeNothing(Maybe<Tm> m, Func<Tres> nothing)
        {
            _m = m;
            _nothing = nothing;
        }

        public Tres Just(Func<Tm, Tres> just)
        {
            return _m.Callbacks<Tres>(_nothing, just);
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Return<T>(T val)
        {
            return Maybe<T>.Just(val);
        }

        public static Func<Maybe<Tsrc>, Maybe<Tres>> Bind<Tsrc, Tres>(Func<Tsrc, Maybe<Tres>> f)
        {
            return m => m.Nothing(() => Maybe<Tres>.Nil).Just(f);
        }

        public static Maybe<Tres> Bind<Tsrc,Tres>(this Maybe<Tsrc> x, Func<Tsrc, Maybe<Tres>> f)
        {
            return Bind(f)(x);
        }

        public static Maybe<Tres> Select<Tsrc,Tres>(this Maybe<Tsrc> x, Func<Tsrc,Tres> f)
        {
            return x.Lift(f);
        }

        public static Maybe<T> Where<T>(this Maybe<T> x, Func<T,Boolean> p)
        {
            return x.Filter(p);
        }

        public static Maybe<Tres> SelectMany<Tsrc,Tint,Tres>(this Maybe<Tsrc> x, Func<Tsrc, Maybe<Tint>> f, Func<Tsrc,Tint,Tres> g)
        {
            return x.Bind(outer => f(outer).Bind(inner => Return(g(outer,inner))));
        }
    }
}