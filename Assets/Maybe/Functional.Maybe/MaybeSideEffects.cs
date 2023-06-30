using System;

namespace Functional.Maybe
{
    /// <summary>
    /// Applying side effects into the Maybe call chain
    /// </summary>
    public static class MaybeSideEffects
    {
        public static Maybe<T> Do<T>(this Maybe<T> m, Action fn)
        {
            return m.Do(_ => fn());
        }

        public static Maybe<T> Set<T>(this Maybe<T> m, ref T t)
        {
            if (m.HasValue)
            {
                t = m.Value;
            }
            return m;
        }

        /// <summary>
        /// Calls <paramref name="fn"/> if <paramref name="m"/> has value, otherwise does nothing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static Maybe<T> Do<T>(this Maybe<T> m, Action<T> fn)
        {
            if (m.IsSomething())
                fn(m.Value);
            return m;
        }

        public static Maybe<T> Emit<T>(this Maybe<T> m, IObserver<T> obs)
        {
            if (m.IsSomething())
                obs.OnNext(m.Value);
            return m;
        }

        public static Maybe<T> EmitOnly<T>(this Maybe<T> m, IObserver<T> obs, bool errorOnAbsent = false)
        {
            if (m.IsSomething())
            {
                obs.OnNext(m.Value);
                obs.OnCompleted();
            }
            else
            {
                if (errorOnAbsent)
                {
                    obs.OnError(new Exception("Absent Value"));
                }
            }

            return m;
        }

        /// <summary>
        /// Calls <paramref name="fn"/> if <paramref name="m"/> has value, otherwise calls <paramref name="else"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="fn"></param>
        /// <param name="else"></param>
        /// <returns></returns>
        public static Maybe<T> Match<T>(this Maybe<T> m, Action<T> fn, Action @else)
        {
            if (m.IsSomething())
                fn(m.Value);
            else
                @else();
            return m;
        }
    }
}