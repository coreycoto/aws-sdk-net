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
using System.Net;
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
    /// Response Unmarshaller for DescribeClient operation
    /// </summary>  
    public class DescribeClientResponseUnmarshaller : JsonResponseUnmarshaller
    {
        public override AmazonWebServiceResponse Unmarshall(JsonUnmarshallerContext context)
        {
            DescribeClientResponse response = new DescribeClientResponse();

            context.Read();
            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
                if (context.TestExpression("Certificate", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.Certificate = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("ClientArn", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.ClientArn = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("HAPartitionGroupList", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
                    response.HAPartitionGroupList = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("Label", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.Label = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("LastModifiedTimestamp", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.LastModifiedTimestamp = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("PartitionList", targetDepth))
                {
                    var unmarshaller = new ListUnmarshaller<string, StringUnmarshaller>(StringUnmarshaller.Instance);
                    response.PartitionList = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("State", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.State = unmarshaller.Unmarshall(context);
                    continue;
                }
                if (context.TestExpression("StateDetails", targetDepth))
                {
                    var unmarshaller = StringUnmarshaller.Instance;
                    response.StateDetails = unmarshaller.Unmarshall(context);
                    continue;
                }
            }

            return response;
        }

        public override AmazonServiceException UnmarshallException(JsonUnmarshallerContext context, Exception innerException, HttpStatusCode statusCode)
        {
            ErrorResponse errorResponse = JsonErrorResponseUnmarshaller.GetInstance().Unmarshall(context);
            if (errorResponse.Code != null && errorResponse.Code.Equals("CloudHsmServiceException"))
            {
                return new CloudHsmServiceException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
            }
            return new AmazonCloudHSMException(errorResponse.Message, innerException, errorResponse.Type, errorResponse.Code, errorResponse.RequestId, statusCode);
        }

        private static DescribeClientResponseUnmarshaller _instance = new DescribeClientResponseUnmarshaller();        

        internal static DescribeClientResponseUnmarshaller GetInstance()
        {
            return _instance;
        }
        public static DescribeClientResponseUnmarshaller Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}