namespace Caliburn.DynamicProxy.Interceptors
{
	using Castle.DynamicProxy;

    /// <summary>
    /// A base class for interceptors.
	/// </summary>
#if NET
	[System.Serializable]
#endif
	public abstract class InterceptorBase : IInitializableInterceptor
    {
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public abstract void Intercept(IInvocation invocation);

        /// <summary>
        /// Initializes the interceptor with the specified proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        public virtual void Initialize(object proxy)
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return GetType() == obj.GetType();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}