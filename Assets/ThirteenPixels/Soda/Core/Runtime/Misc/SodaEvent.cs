// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An event with a parameter.
    /// SodaEvents are very robust and allow you to add and remove responses while they're invoked.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    public sealed class SodaEvent<T> : SodaEventBase<Action<T>>
    {
        private readonly Func<T> callbackParameter;
        private T invokeAgainParameter;
        private readonly Dictionary<Action, Action<T>> parameterlessResponses = new Dictionary<Action, Action<T>>();
#if UNITY_EDITOR
        private readonly Dictionary<Action<T>, object> parameterlessListenerTargets = new Dictionary<Action<T>, object>();
#endif

        /// <summary>
        /// Creates a new SodaEvent<T>.
        /// </summary>
        /// <param name="callbackParameter">A function returning the parameter value that is used when using AddResponseAndInvoke.</param>
        /// <param name="onChangeResponseCollection">Action to invoke whenever a response is added or removed.</param>
        public SodaEvent(Func<T> callbackParameter = null, Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
            this.callbackParameter = callbackParameter;
        }

        /// <summary>
        /// Add a response to be invoked with the event. The callback is immediately invoked with the current value.
        /// </summary>
        /// <param name="response">The response to add and invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponseAndInvoke(Action<T> response)
        {
#line hidden
            if (callbackParameter == null)
            {
                throw new Exception("Cannot invoke this SodaEvent while adding a response.");
            }
#line default

            var success = AddResponse(response);
            if (success)
            {
                try
                {
                    response(callbackParameter());
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }
            return success;
        }
        
        /// <summary>
        /// Add a parameterless response to be invoked with the event. The callback is immediately invoked with the current value.
        /// </summary>
        /// <param name="response">The response to add and invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponseAndInvoke(Action response)
        {
#line hidden
            if (callbackParameter == null)
            {
                throw new Exception("Cannot invoke this SodaEvent while adding a response.");
            }
#line default

            var success = AddResponse(response);
            if (success)
            {
                try
                {
                    response();
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }
            return success;
        }

        /// <summary>
        /// Adds a parameterless response to the response list.
        /// </summary>
        /// <param name="response">The response to add.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public bool AddResponse(Action<T> response)
        {
            return InternalAddResponse(response);
        }

        public sealed override bool AddResponse(Action response)
        {
            Action<T> wrapper = t => response();
#if UNITY_EDITOR
            parameterlessListenerTargets.Add(wrapper, response.Target);
#endif
            var success = InternalAddResponse(wrapper);
            if (success)
            {
                parameterlessResponses.Add(response, wrapper);
            }
            return success;
        }

        public void RemoveResponse(Action<T> response)
        {
            InternalRemoveResponse(response);
        }

        public sealed override void RemoveResponse(Action response)
        {
            if (parameterlessResponses.TryGetValue(response, out var wrapper))
            {
#if UNITY_EDITOR
                parameterlessListenerTargets.Remove(wrapper);
#endif
                InternalRemoveResponse(wrapper);
                
                parameterlessResponses.Remove(response);  // ASSETMOD: fixes bug, replace with official solution
            }
        }

        internal void Invoke(T parameter)
        {
            if (currentlyBeingInvoked)
            {
                invokeAgain = true;
                invokeAgainParameter = parameter;
                return;
            }

            RunInvocation(action => action(parameter));

            if (invokeAgain)
            {
                invokeAgain = false;
                var p = invokeAgainParameter;
                invokeAgainParameter = default;

                Invoke(p);
            }
        }

#if UNITY_EDITOR
        internal sealed override int GetListeners(object[] listeners)
        {
            object GetTarget(Action<T> action)
            {
                if (parameterlessListenerTargets.TryGetValue(action, out var target))
                {
                    return target;
                }

                return action.Target;
            }

            return WriteToArray(responses.Select(action => GetTarget(action)), listeners);
        }
#endif
    }

    /// <summary>
    /// An event without a parameter.
    /// SodaEvents are very robust and allow you to add and remove responses while they're invoked.
    /// </summary>
    public sealed class SodaEvent : SodaEventBase<Action>
    {
        internal void Invoke()
        {
            if (currentlyBeingInvoked)
            {
                invokeAgain = true;
                return;
            }

            RunInvocation(action => action());

            if (invokeAgain)
            {
                invokeAgain = false;
                Invoke();
            }
        }

        /// <summary>
        /// Add a response to be invoked with the event.
        /// </summary>
        /// <param name="response">The response to invoke.</param>
        /// <returns>True if the response could be added, false if it was already added before.</returns>
        public sealed override bool AddResponse(Action response)
        {
            return InternalAddResponse(response);
        }

        /// <summary>
        /// Removes a response so it's no longer invoked.
        /// </summary>
        /// <param name="response">The response to remove.</param>
        public sealed override void RemoveResponse(Action response)
        {
            InternalRemoveResponse(response);
        }

#if UNITY_EDITOR
        internal sealed override int GetListeners(object[] listeners)
        {
            return WriteToArray(responses.Select(action => action.Target), listeners);
        }
#endif
    }

    public abstract class SodaEventBase<T> : SodaEventBase
    {
        protected readonly List<T> responses = new List<T>();
        protected readonly HashSet<T> responseSet = new HashSet<T>();
        protected readonly List<T> responsesToRemove = new List<T>();

        /// <summary>
        /// The number of registered responses.
        /// </summary>
        public int responseCount => responses.Count;

        public SodaEventBase(Action onChangeResponseCollection = null) : base(onChangeResponseCollection)
        {
        }

        protected bool InternalAddResponse(T response)
        {
            if (!responseSet.Contains(response))
            {
                responses.Add(response);
                responseSet.Add(response);
                onChangeResponseCollection?.Invoke();
                return true;
            }
            return false;
        }

        protected void InternalRemoveResponse(T response)
        {
            if (currentlyBeingInvoked)
            {
                responsesToRemove.Add(response);
            }
            else
            {
                ActuallyRemoveResponse(response);
            }
        }

        protected void RunInvocation(Action<T> invocation)
        {
            currentlyBeingInvoked = true;

            responsesToRemove.Clear();

            for (var i = responses.Count - 1; i >= 0; i--)
            {
                try
                {
                    invocation(responses[i]);
                }
                catch (Exception e)
                {
#line hidden
#if UNITY_EDITOR
                    // Don't use an extra catch to improve test robustness.
                    if (!(e is TestException))
                    {
                        Debug.LogException(e);
                    }
#else
                    Debug.LogException(e);
#endif
#line default
                }
            }

            foreach (var response in responsesToRemove)
            {
                ActuallyRemoveResponse(response);
            }
            responsesToRemove.Clear();

            currentlyBeingInvoked = false;
        }

        protected bool ActuallyRemoveResponse(T response)
        {
            return responseSet.Remove(response) && responses.Remove(response);
        }
    }

    public abstract class SodaEventBase
    {
#if UNITY_EDITOR
        /// <summary>
        /// Exception that is used for test cases.
        /// This exception type is not logged to the console if it is thrown during an invocation.
        /// </summary>
        internal class TestException : Exception { }
#endif

        protected readonly Action onChangeResponseCollection;
        // For preventing cyclic/recursive invocation
        protected bool currentlyBeingInvoked = false;
        protected bool invokeAgain = false;

        public SodaEventBase(Action onChangeResponseCollection = null)
        {
            this.onChangeResponseCollection = onChangeResponseCollection;
        }

        public abstract bool AddResponse(Action response);
        public abstract void RemoveResponse(Action response);

#if UNITY_EDITOR
        internal abstract int GetListeners(object[] listeners);
#endif

        protected static int WriteToArray<T>(IEnumerable<T> input, T[] output)
        {
            var index = 0;
            using (var values = input.GetEnumerator())
            {
                while (index < output.Length && values.MoveNext())
                {
                    output[index] = values.Current;
                    index++;
                }
            }
            return index;
        }
    }
}