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
    }
}