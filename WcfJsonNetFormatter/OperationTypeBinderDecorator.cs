namespace WcfJsonFormatter.Ns
{
    using System;

    /// <summary>
    /// A custom binder which implements the new <see cref="Newtonsoft.Json.Serialization.ISerializationBinder"/> interface.
    /// </summary>
    /// <seealso cref="WcfJsonFormatter.OperationTypeBinder" />
    /// <seealso cref="Newtonsoft.Json.Serialization.ISerializationBinder" />
    public class OperationTypeBinderDecorator : OperationTypeBinder, Newtonsoft.Json.Serialization.ISerializationBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationTypeBinderDecorator"/> class.
        /// </summary>
        /// <param name="serviceRegister"></param>
        public OperationTypeBinderDecorator(IServiceRegister serviceRegister)
            : base(serviceRegister)
        {
        }
    }
}
