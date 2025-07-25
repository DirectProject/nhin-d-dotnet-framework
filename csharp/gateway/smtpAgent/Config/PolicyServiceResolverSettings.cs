﻿/* 
 Copyright (c) 2014, Direct Project
 All rights reserved.

 Authors:
    Joe Shook     Joseph.Shook@Surescipts.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/

using System;
using System.Xml.Serialization;
using Health.Direct.Agent.Config;
using Health.Direct.Common.Caching;
using Health.Direct.Common.Certificates;
using Health.Direct.Common.Policies;
using Health.Direct.Config.Client;
using Health.Direct.SmtpAgent.Policy;

namespace Health.Direct.SmtpAgent.Config
{

    
    [XmlType("Public")]
    public class PublicPolicyServiceResolverSettings : PolicyResolverSettings
    {
        /// <summary>
        /// The name of this store.
        /// </summary>
        public override string Name
        {
            get { return CertPolicyResolvers.PublicPolicyName; }
        }

        [XmlElement]
        public ClientSettings ClientSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Optional (but you're an idiot if you don't provide) cache settings
        /// </summary>
        [XmlElement]
        public CacheSettings CacheSettings
        {
            get;
            set;
        }

        public override IPolicyResolver CreateResolver()
        {
            PublicPolicyResolver resolver = new PublicPolicyResolver(this);
            return resolver;
        }

        public override void Validate()
        {
            if (this.ClientSettings == null)
            {
                throw new SmtpAgentException(SmtpAgentError.MissingPolicyResolverClientSettings);
            }
            this.ClientSettings.Validate();
        }

    }

    [XmlType("Private")]
    public class PrivatePolicyServiceResolverSettings : PolicyResolverSettings
    {
        /// <summary>
        /// The name of this store.
        /// </summary>
        public override string Name
        {
            get { return CertPolicyResolvers.PrivatePolicyName; }
        }

        [XmlElement]
        public ClientSettings ClientSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Optional (but you're an idiot if you don't provide) cache settings
        /// </summary>
        [XmlElement]
        public CacheSettings CacheSettings
        {
            get;
            set;
        }
        public override IPolicyResolver CreateResolver()
        {
            PrivatePolicyResolver resolver = new PrivatePolicyResolver(this);
            return resolver;
        }

        public override void Validate()
        {
            if (this.ClientSettings == null)
            {
                throw new SmtpAgentException(SmtpAgentError.MissingPolicyResolverClientSettings);
            }
            this.ClientSettings.Validate();
        }

    }


    [XmlType("Trust")]
    public class TrustPolicyServiceResolverSettings : PolicyResolverSettings
    {
        /// <summary>
        /// The name of this store.
        /// </summary>
        public override string Name
        {
            get { return CertPolicyResolvers.TrustPolicyName; }
        }

        [XmlElement]
        public ClientSettings ClientSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Optional (but you're an idiot if you don't provide) cache settings
        /// </summary>
        [XmlElement]
        public CacheSettings CacheSettings
        {
            get;
            set;
        }
        public override IPolicyResolver CreateResolver()
        {
            TrustPolicyResolver resolver = new TrustPolicyResolver(this);
            return resolver;
        }

        public override void Validate()
        {
            if (this.ClientSettings == null)
            {
                throw new SmtpAgentException(SmtpAgentError.MissingPolicyResolverClientSettings);
            }
            this.ClientSettings.Validate();
        }

    }
}
