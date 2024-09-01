using Greenhouse.Libs.Serialization.Data;

namespace Greenhouse.Libs.Serialization.Result
{
    public static class Either
    {
        public static Either<L, R> Left<L, R>(L value) where L : notnull where R : notnull => new(Optional.Of(value), Optional.Empty<R>());
        public static Either<L, R> Right<L, R>(R value) where L : notnull where R : notnull => new(Optional.Empty<L>(), Optional.Of(value));
    }

    public readonly struct Either<L, R> where L : notnull where R : notnull
    {
        public readonly Optional<L> Left;
        public readonly Optional<R> Right;

        internal Either(Optional<L> left, Optional<R> right)
        {
            Left = left;
            Right = right;
            Validate();
        }

        private void Validate()
        {
            if (Left.HasValue == Right.HasValue)
                throw new InvalidOperationException("Could not create an Either with more or less than one value.");
        }

        public Either<A, B> SelectBoth<A, B>(Func<L, A> leftFunc, Func<R, B> rightFunc) where A : notnull where B : notnull
        {
            return new Either<A, B>(Left.Select(leftFunc), Right.Select(rightFunc));  
        }

        public Either<A, R> SelectLeft<A>(Func<L, A> func) where A : notnull
        {
            return new Either<A, R>(Left.Select(func), Right);
        }

        public Either<L, A> SelectRight<A>(Func<R, A> func) where A : notnull
        {
            return new Either<L, A>(Left, Right.Select(func));
        }
        public T Select<T>(Func<L, T> leftFunc, Func<R, T> rightFunc) where T: notnull
        {
            Either<T, T> either = SelectBoth(leftFunc, rightFunc);
            return either.Left.OrElse(either.Right.Get());
        }

        public override string ToString()
        {
            string retValue = "Either<";
            if (Left.HasValue)
                retValue += $"Left={Left.Get()}";
            else
                retValue += $"Right={Right.Get()}";
            retValue += ">";
            return retValue;
        }
    }
}
