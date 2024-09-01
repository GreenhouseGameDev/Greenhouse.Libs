using System.Collections;

namespace Greenhouse.Libs.Serialization.Data
{
    public static class Optional
    {
        public static Optional<R> Of<R>(R value) where R : notnull => new(value);
        public static Optional<R> OfNullable<R>(R? value) where R : notnull => value == null ? Empty<R>() : Of(value);
        public static Optional<R> OfNullable<R>(R? value) where R : struct => value.HasValue ? Of(value.Value) : Empty<R>();
        public static Optional<R> Empty<R>() where R : notnull => default;
    }

    public readonly struct Optional<R> : IEnumerable<R> where R : notnull
    {
        private readonly R? _value;
        public readonly bool HasValue => _value != null;

        public Optional(R? value) 
        {
            _value = value;
        }
        
        public R Get()
        {
            if (!HasValue)
                throw new InvalidOperationException("Could not access the value of empty optional: " + nameof(Optional<R>));
            return _value!;
        }

        public R OrElse(R other)
        {
            if (!HasValue)
                return other;
            return _value!;
        }

        public R OrElseGet(Func<R> func)
        {
            return HasValue ? _value! : func();
        }

        public R OrElseThrow(Func<Exception> exception)
        {
            return HasValue ? _value! : throw exception();
        }

        public Optional<T> Select<T>(Func<R, T> func) where T : notnull
        {
            if (!HasValue)
                return Optional.Empty<T>();
            return Optional.OfNullable(func(_value!));
        }

        public IEnumerator<R> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            string retValue = "Optional<";
            if (!HasValue)
                retValue += "Empty";
            else
                retValue += _value;
            retValue += ">";
            return retValue;
        }

        private struct Enumerator : IEnumerator<R>
        {
            private readonly Optional<R> _optional;
            private bool hasMoved;

            public Enumerator(Optional<R> optional)
            {
                _optional = optional;
                hasMoved = false;
            }

            public R Current => _optional.Get();

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (hasMoved)
                    return false;
                hasMoved = true;
                return _optional.HasValue;
            }

            public void Reset()
            {
                hasMoved = false;
            }
        }
    }

}
