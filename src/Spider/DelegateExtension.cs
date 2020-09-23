using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spider
{
    public static class Try
    {
        public static Action<Exception> TodoExceptionHook { get; set; }

        public readonly static Action<Exception> FailEmpty = e => { };

        public static void Todo(this Action fn, Action<Exception> fail = null, Action finallyAction = null)
        {
            fn.Doing(null, e =>
            {
                if (TodoExceptionHook != null) { TodoExceptionHook(e); }
                if (fail != null) { fail(e); }
            }, finallyAction);
        }
        public static void TodoAsync(this Action fn, Action<Exception> fail = null, Action finallyAction = null)
        {
            fn.DoingAsync(null, e =>
            {
                if (TodoExceptionHook != null) { TodoExceptionHook(e); }
                if (fail != null) { fail(e); }
            }, finallyAction);
        }

        public static T Todo<T>(this Func<T> fn, Action<Exception> fail = null, Action finallyAction = null)
        {
            return fn.Doing<T>(null, e =>
            {
                if (TodoExceptionHook != null) { TodoExceptionHook(e); }
                if (fail != null) { fail(e); }
            }, finallyAction);
        }

        public static void TodoAsync<T>(this Func<T> fn, Action<Exception> fail = null, Action finallyAction = null)
        {
            fn.DoingAsync<T>(null, e =>
            {
                if (TodoExceptionHook != null) { TodoExceptionHook(e); }
                if (fail != null) { fail(e); }
            }, finallyAction);
        }

        public static void Doing(this Action fn, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }

            try
            {
                fn();
                if (success != null) { success(); }
            }
            catch (Exception ex)
            {
                if (fail != null)
                {
                    fail(ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (finallyAction != null) { finallyAction(); }
            }
        }
        public static T Doing<T>(this Func<T> fn, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            T t = default(T);
            if (fn == null) { return t; }

            try
            {
                t = fn();
                if (success != null) { success(); }
            }
            catch (Exception e)
            {
                if (fail != null)
                {
                    fail(e);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (finallyAction != null) { finallyAction(); }
            }
            return t;
        }

        public static void DoingAsync(this EventHandler fn, object sender, EventArgs args, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(sender, args);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }
        public static void DoingAsync<T>(this EventHandler<T> fn, object sender, T args, Action success = null, Action<Exception> fail = null, Action finallyAction = null) where T : EventArgs
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(sender, args);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }
        public static void DoingAsync(this Action fn, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn();
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T>(this Action<T> fn, T arg, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(arg);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2>(this Action<T1, T2> fn, T1 arg1, T2 arg2, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(arg1, arg2);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2, T3>(this Action<T1, T2, T3> fn, T1 arg1, T2 arg2, T3 arg3, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(arg1, arg2, arg3);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> fn, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    fn(arg1, arg2, arg3, arg4);
                    if (success != null) { success(); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<TResult>(this Func<TResult> fn, Action<TResult> success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    var result = fn();
                    if (success != null) { success(result); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T, TResult>(this Func<T, TResult> fn, T arg1, Action<TResult> success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    var result = fn(arg1);
                    if (success != null) { success(result); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2, TResult>(this Func<T1, T2, TResult> fn, T1 arg1, T2 arg2, Action<TResult> success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    var result = fn(arg1, arg2);
                    if (success != null) { success(result); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> fn, T1 arg1, T2 arg2, T3 arg3, Action<TResult> success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    var result = fn(arg1, arg2, arg3);
                    if (success != null) { success(result); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null) { finallyAction(); }
                }
            });
        }

        public static void DoingAsync<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> fn, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Action<TResult> success = null, Action<Exception> fail = null, Action finallyAction = null)
        {
            if (fn == null) { return; }
            Task.Run(() =>
            {
                try
                {
                    var result = fn(arg1, arg2, arg3, arg4);
                    if (success != null) { success(result); }
                }
                catch (Exception ex)
                {
                    if (fail != null)
                    {
                        fail(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }
    }
}
