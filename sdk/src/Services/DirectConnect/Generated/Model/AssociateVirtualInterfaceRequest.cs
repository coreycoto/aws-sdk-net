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
 * Do not modify this file. This file is generated from the directconnect-2012-10-25.normal.json service model.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.IO;

using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace Amazon.DirectConnect.Model
{
    /// <summary>
    /// Container for the parameters to the AssociateVirtualInterface operation.
    /// Associates a virtual interface with a specified link aggregation group (LAG) or connection.
    /// Connectivity to AWS is temporarily interrupted as the virtual interface is being migrated.
    /// If the target connection or LAG has an associated virtual interface with a conflicting
    /// VLAN number or a conflicting IP address, the operation fails. 
    /// 
    ///  
    /// <para>
    /// Virtual interfaces associated with a hosted connection cannot be associated with a
    /// LAG; hosted connections must be migrated along with their virtual interfaces using
    /// <a>AssociateHostedConnection</a>.
    /// </para>
    ///  
    /// <para>
    /// Hosted virtual interfaces (an interface for which the owner of the connection is not
    /// the owner of physical connection) can only be reassociated by the owner of the physical
    /// connection.
    /// </para>
    /// </summary>
    public partial class AssociateVirtualInterfaceRequest : AmazonDirectConnectRequest
    {
        private string _connectionId;
        private string _virtualInterfaceId;

        /// <summary>
        /// Gets and sets the property ConnectionId. 
        /// <para>
        /// The ID of the LAG or connection with which to associate the virtual interface.
        /// </para>
        ///  
        /// <para>
        /// Example: dxlag-abc123 or dxcon-abc123
        /// </para>
        ///  
        /// <para>
        /// Default: None
        /// </para>
        /// </summary>
        public string ConnectionId
        {
            get { return this._connectionId; }
            set { this._connectionId = value; }
        }

        // Check to see if ConnectionId property is set
        internal bool IsSetConnectionId()
        {
            return this._connectionId != null;
        }

        /// <summary>
        /// Gets and sets the property VirtualInterfaceId. 
        /// <para>
        /// The ID of the virtual interface.
        /// </para>
        ///  
        /// <para>
        /// Example: dxvif-123dfg56
        /// </para>
        ///  
        /// <para>
        /// Default: None
        /// </para>
        /// </summary>
        public string VirtualInterfaceId
        {
            get { return this._virtualInterfaceId; }
            set { this._virtualInterfaceId = value; }
        }

        // Check to see if VirtualInterfaceId property is set
        internal bool IsSetVirtualInterfaceId()
        {
            return this._virtualInterfaceId != null;
        }

    }
}