using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace WcfJsonService.Example.Extra
{
    /// <summary>
    /// 
    /// </summary>
    public class WebFaultClientMessageInspector
        : IClientMessageInspector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply != null)
            {
                HttpResponseMessageProperty httpProp = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (httpProp != null && httpProp.StatusCode == HttpStatusCode.InternalServerError)
                    throw new CommunicationException(httpProp.StatusDescription);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return null;
        }
    }
}
