/*
 * Copyright 2010-2014 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

/*
 * Do not modify this file. This file is generated from the cloudhsm-2014-05-30.normal.json service model.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Amazon.CloudHSM.Model;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using ThirdParty.Json.LitJson;

namespace Amazon.CloudHSM.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// CreateHsm Request Marshaller
    /// </summary>       
    public class CreateHsmRequestMarshaller : IMarshaller<IRequest, CreateHsmRequest> , IMarshaller<IRequest,AmazonWebServiceRequest>
    {
        public IRequest Marshall(AmazonWebServiceRequest input)
        {
            return this.Marshall((CreateHsmRequest)input);
        }

        public IRequest Marshall(CreateHsmRequest publicRequest)
        {
            IRequest request = new DefaultRequest(publicRequest, "Amazon.CloudHSM");
            string target = "CloudHsmFrontendService.CreateHsm";
            request.Headers["X-Amz-Target"] = target;
            request.Headers["Content-Type"] = "application/x-amz-json-1.1";
            request.HttpMethod = "POST";

            string uriResourcePath = "/";
            request.ResourcePath = uriResourcePath;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                JsonWriter writer = new JsonWriter(stringWriter);
                writer.WriteObjectStart();
                var context = new JsonMarshallerContext(request, writer);
                if(publicRequest.IsSetClientToken())
                {
                    context.Writer.WritePropertyName("ClientToken");
                    context.Writer.Write(publicRequest.ClientToken);
                }

                if(publicRequest.IsSetEniIp())
                {
                    context.Writer.WritePropertyName("EniIp");
                    context.Writer.Write(publicRequest.EniIp);
                }

                if(publicRequest.IsSetExternalId())
                {
                    context.Writer.WritePropertyName("ExternalId");
                    context.Writer.Write(publicRequest.ExternalId);
                }

                if(publicRequest.IsSetIamRoleArn())
                {
                    context.Writer.WritePropertyName("IamRoleArn");
                    context.Writer.Write(publicRequest.IamRoleArn);
                }

                if(publicRequest.IsSetSshKey())
                {
                    context.Writer.WritePropertyName("SshKey");
                    context.Writer.Write(publicRequest.SshKey);
                }

                if(publicRequest.IsSetSubnetId())
                {
                    context.Writer.WritePropertyName("SubnetId");
                    context.Writer.Write(publicRequest.SubnetId);
                }

                if(publicRequest.IsSetSubscriptionType())
                {
                    context.Writer.WritePropertyName("SubscriptionType");
                    context.Writer.Write(publicRequest.SubscriptionType);
                }

                if(publicRequest.IsSetSyslogIp())
                {
                    context.Writer.WritePropertyName("SyslogIp");
                    context.Writer.Write(publicRequest.SyslogIp);
                }

        
                writer.WriteObjectEnd();
                string snippet = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(snippet);
            }


            return request;
        }


    }
}