using System;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WcfJsonService.Example.Extra
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalErrorHandler : IErrorHandler
    {
        private readonly bool includeExceptionDetailInFaults;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeExceptionDetailInFaults"></param>
        public GlobalErrorHandler(bool includeExceptionDetailInFaults)
        {
            this.includeExceptionDetailInFaults = includeExceptionDetailInFaults;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool HandleError(Exception error)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="fault"></param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (version != MessageVersion.None || error == null)
            {
                return;
            }

            // If the exception is not derived from FaultException and the fault message is already present
            //   then only another error handler could have provided the fault so we should not replace it
            FaultException errorAsFaultException = error as FaultException;
            if (errorAsFaultException == null && fault != null)
            {
                return;
            }

            var newEx = new FaultException(
                string.Format("Exception caught at GlobalErrorHandler{0}Method: {1}{2}Message:{3}",
                             Environment.NewLine, error.TargetSite.Name, Environment.NewLine, error.Message));

            MessageFault msgFault = newEx.CreateMessageFault();
            fault = Message.CreateMessage(version, msgFault, newEx.Action);
        }

    }
}
