# MattUtils.Monads

Because `NullReferenceException` is just a funny misspelling of
`ShouldveUsedHaskellException`.

## What's this all about, then?

Let's suppose you have a DTO, it's just a POCO, and you're in a
situation where you can't map it completely to some domain object or
other. Now your domain object has fields that default to, well,
`default(T)` for whatever `T` you've given them.  You write domain logic
under the assumption that your domain objects aren't shite, and
somewhere down a long chain of computation you subtract something from,
I dunno, `DateTime.MinValue` and get an `IntegerOverflowException`.

Wouldn't it be nice if the compiler yelled at you when you forgot to
assign more sensible defaults to the fields you couldn't map?

## And monads are going to solve this for me, huh?

Well, they can help at least. Instead of declaring your unmappable date
as

```
  DateTime foo; // defaults to DateTime.Min
```

You can declare it as

```
  Maybe<DateTime> foo; // defaults to Maybe<DateTime>.Nil
```

Now the compiler will yell at you if you try to do math on it. The only
way forward is explicitly to consider the possibility that `foo` didn't
get mapped:

```
  foo.Nothing(() => throw new Exception("OMG ONOZ"))
     .Just(d => DoSomethingWith(d));
```

## But `Maybe` is so much nicer in Haskell!

(sigh) I _know._ If Haskell's an option for you, go with my blessing.
