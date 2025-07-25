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
using System;
using System.IO;
using System.Xml.Serialization;

using Health.Direct.Config.Client;

namespace Health.Direct.Config.Console
{
    [XmlRoot("ConsoleSettings")]
    public class ConsoleSettings
    {
        public event EventHandler HostAndPortChanged;

        private void InvokeHostAndPortChanged()
        {
            EventHandler changed = HostAndPortChanged;
            if (changed != null) changed(this, EventArgs.Empty);
        }

        [XmlElement]
        public ClientSettings DomainManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings AddressManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings AnchorManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings CertificateManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings DnsRecordManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings PropertyManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings BlobManager
        {
            get;
            set;
        }
        
        
        [XmlElement]
        public ClientSettings BundleManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings MdnMonitorManager
        {
            get;
            set;
        }

        [XmlElement]
        public ClientSettings CertPolicyManager
        {
            get;
            set;
        }

        public void SetHost(string host, int port)
        {
            this.DomainManager.SetHost(host, port);
            this.AddressManager.SetHost(host, port);
            this.CertificateManager.SetHost(host, port);
            this.AnchorManager.SetHost(host, port);
            this.BundleManager.SetHost(host, port);
            if (this.PropertyManager != null)
            {
                this.PropertyManager.SetHost(host, port);
            }
            if (this.BlobManager != null)
            {
                this.BlobManager.SetHost(host, port);
            }
            if (this.MdnMonitorManager != null)
            {
                this.MdnMonitorManager.SetHost(host, port);
            }
            if (this.CertPolicyManager != null)
            {
                this.CertPolicyManager.SetHost(host, port);
            }
            InvokeHostAndPortChanged();
        }
        
        public void Validate()
        {
            Validate(this.DomainManager, "DomainManager", true);
            Validate(this.AddressManager, "AddressManager", true);
            Validate(this.AnchorManager, "AnchorManager", true);
            Validate(this.BundleManager, "BundleManager", true);
            Validate(this.CertificateManager, "CertificateManager", true);
            Validate(this.PropertyManager, "PropertyManager", false);
            Validate(this.BlobManager, "BlobManager", false);
            Validate(this.MdnMonitorManager, "MdnMonitorManager", false);
            Validate(this.CertPolicyManager, "BlobManager", false);
        }
        
        void Validate(ClientSettings settings, string name, bool required)
        {
            if (settings == null && !required)
            {
                return;
            }
            
            if (settings == null)
            {
                throw new ArgumentException(string.Format("Invalid {0} Config", name));
            }
            try
            {
                settings.Validate();
            }
            catch(Exception ex)
            {
                throw new ArgumentException(name, ex);
            }
        }
        
        public static ConsoleSettings Load()
        {
            return Load("ConfigConsoleSettings.xml");
        }
        
        public static ConsoleSettings Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConsoleSettings));
            using(Stream stream = File.OpenRead(path))
            {
                ConsoleSettings settings = (ConsoleSettings) serializer.Deserialize(stream);
                settings.Validate();
                return settings;
            }
        }
    }

    public class TokenSettings
    {
        private string m_pkcs11LibraryPath;
        private string m_tokenSerialNumber;
        private string m_tokenLabel;
        private string m_applicationName;

        static TokenSettings()
        {

        }
        [XmlElement(ElementName = "Library")]
        public string Pkcs11LibraryPath
        {
            get
            {
                return m_pkcs11LibraryPath;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("value was null or empty", nameof(value));
                m_pkcs11LibraryPath = value;
            }
        }

        /// <summary>
        /// This should always be true.
        /// From PKCS#11 V2.20: CRYPTOGRAPHIC TOKEN INTERFACE STANDARD, section 6.6.2 "Applications and threads"
        /// 
        /// 2. The application can specify that it will be accessing the library concurrently from
        ///    multiple threads, and the library must be able to use native operation system
        ////   synchronization primitives to ensure proper thread-safe behavior.
        /// 
        /// 
        /// </summary>
        public bool UseOsLocking { get { return true; } }

        [XmlElement]
        public string TokenLabel
        {
            get
            {
                return m_tokenLabel;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("value was null or empty", nameof(value));
                m_tokenLabel = value;
            }
        }
        
        [XmlElement]
        public string ApplicationName
        {
            get
            {
                return m_applicationName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("value was null or empty", nameof(value));
                m_applicationName = value;
            }
        }

        [XmlElement(ElementName = "UserPin")]
        public string NormalUserPin { get; set; }

        /// <summary>
        /// Arguments passed to the C_Initialize function in LowLevelAPI41 tests.
        /// </summary>
        public static Net.Pkcs11Interop.LowLevelAPI41.CK_C_INITIALIZE_ARGS InitArgs41 = null;

        /// <summary>
        /// PIN of the normal user.
        /// </summary>
        public byte[] NormalUserPinArray = null;

        /// <summary>
        /// PKCS#11 URI that identifies private key usable in signature creation tests.
        /// </summary>
        public string PrivateKeyUri = null;
    }
}