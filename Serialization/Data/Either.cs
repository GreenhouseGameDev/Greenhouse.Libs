namespace Greenhouse.Libs.Serialization.Data;

public static class Either {
    public static Either<L, R> Left<L, R>(L value) where L : notnull where R : notnull
        => new(value);
    public static Either<L, R> Right<L, R>(R value) where L : notnull where R : notnull
        => new(value);
}

public readonly struct Either<L, R> where L : notnull where R : notnull {
    public readonly L? Left;
    public readonly R? Right;

    internal Either(L left) {
        Left = left;
        Validate();
    }
    internal Either(R right) {
        Right = right;
        Validate();
    }

    private void Validate() {
        if (Left == null && Right == null)
            throw new InvalidOperationException("Could not create an Either with more or less than one value.");
    }

    public Either<A, B> SelectBoth<A, B>(Func<L, A> leftFunc, Func<R, B> rightFunc) where A : notnull where B : notnull {
        if (Left != null)
            return new(leftFunc(Left));
        if (Right != null)
            return new(rightFunc(Right));
        throw new ArgumentException("Left and right are null");
    }

    public Either<A, R> SelectLeft<A>(Func<L, A> func) where A : notnull {
        if (Left != null)
            return new(func(Left));
        return new(Right!);
    }

    public Either<L, A> SelectRight<A>(Func<R, A> func) where A : notnull {
        if (Right != null)
            return new(func(Right));
        return new(Left!);
    }
    public T Select<T>(Func<L, T> leftFunc, Func<R, T> rightFunc) where T: notnull {
        Either<T, T> either = SelectBoth(leftFunc, rightFunc);
        return either.Left ?? either.Right!;
    }

    public override string ToString() {
        string retValue = "Either<";
        if (Left != null)
            retValue += $"Left={Left}";
        else
            retValue += $"Right={Right}";
        retValue += ">";
        return retValue;
    }
}
