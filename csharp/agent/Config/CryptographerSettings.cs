﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System.Xml.Serialization;

using Health.Direct.Common.Cryptography;

namespace Health.Direct.Agent.Config
{
    /// <summary>
    /// Settings for cryptographic methods.
    /// </summary>
    [XmlType("CryptographerSettings")]
    public class CryptographerSettings
    {
        private ISmimeCryptographer m_cryptographer;

        /// <summary>
        /// Creates an instance, normally called via one of the <see cref="AgentSettings"/> class factory methods.
        /// </summary>
        public CryptographerSettings()
        {
        }

        EncryptionAlgorithm m_defaultEncryption = EncryptionAlgorithm.AES128;
        DigestAlgorithm m_defaultDigest = DigestAlgorithm.SHA1;
        
        /// <summary>
        /// Plugin cryptographer settings.
        /// </summary>
        [XmlElement("PluginCryptographer", typeof(PluginCryptographerSettings))]
        public PluginCryptographerSettings Cryptographer
        {
            get;
            set;
        }

        /// <summary>
        /// The default encryption algorithm to use.
        /// </summary>
        [XmlElement]
        public EncryptionAlgorithm DefaultEncryption
        {
            get
            {
                return m_defaultEncryption;
            }
            set
            {
                m_defaultEncryption = value;
            }
        }
        
        /// <summary>
        /// The default digest algorithm to use.
        /// </summary>
        [XmlElement]
        public DigestAlgorithm DefaultDigest
        {
            get
            {
                return m_defaultDigest;
            }
            set
            {
                m_defaultDigest = value;
            }
        }
        
        /// <summary>
        /// Creates an <see cref="SMIMECryptographer"/> with the configured settings
        /// </summary>
        /// <returns>The configured cryptographer.</returns>
        public ISmimeCryptographer Create()
        {
            if (Cryptographer == null)
            {
                return new SMIMECryptographer(DefaultEncryption, DefaultDigest);
            }
            else
            {
                return Cryptographer.Create();
            }
        }
    }
}