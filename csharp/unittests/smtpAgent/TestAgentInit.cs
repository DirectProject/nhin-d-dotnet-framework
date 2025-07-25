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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Health.Direct.Agent.Config;
using Health.Direct.Common.Certificates;
using Health.Direct.Common.Container;
using Health.Direct.Common.Diagnostics;
using Health.Direct.Common.Policies;
using Health.Direct.SmtpAgent.Config;
using Health.Direct.SmtpAgent.Diagnostics;
using Xunit;

namespace Health.Direct.SmtpAgent.Tests
{
    /// <summary>
    /// Test various agent initializations
    /// </summary>
    public class TestAgentInit
    {
        MessageArrivalEventHandler m_handler;

        public TestAgentInit()
        {
            m_handler = new MessageArrivalEventHandler();
        }


        public static IEnumerable<object[]> ConfigFileNames
        {
            get
            {
                yield return new[] { "TestSmtpAgentConfig.xml" };
                yield return new[] { "TestSmtpAgentConfigService.xml" };
            }
        }

        [Theory]
        [MemberData("DirectTenancConfigFiles")]
        public void TestDirectTenantWithService(string fileName)
        {
            m_handler.InitFromConfigFile(Fullpath(fileName));
        }

        [Fact]
        public void TestContainer()
        {
            SmtpAgentSettings settings = null;
            Assert.Null(Record.Exception(() => settings = SmtpAgentSettings.LoadSettings(Fullpath("TestPlugin.xml"))));
            Assert.NotNull(settings.Container);
            Assert.True(settings.Container.HasComponents);

            ResetSmtpAgentFactory();
            SmtpAgent agent = SmtpAgentFactory.Create(Fullpath("TestPlugin.xml"));

            ILogFactory logFactory = null;
            Assert.Null(Record.Exception(() => logFactory = IoC.Resolve<ILogFactory>()));

            IAuditor auditor = null;
            Assert.Null(Record.Exception(() => auditor = IoC.Resolve<IAuditor<IBuildAuditLogMessage>>()));
            Assert.True(auditor is DummyAuditor);
        }

        [Fact]
        public void TestAddressDomainEnabled_Settings()
        {
            SmtpAgentSettings settings = null;
            Assert.Null(Record.Exception(() => settings = SmtpAgentSettings.LoadSettings(Fullpath("TestSmtpAgentConfigService.xml"))));

            Assert.True(settings.AddressManager.HasSettings);
            using (XmlNodeReader reader = new XmlNodeReader(settings.AddressManager.Settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AddressManagerSettings), new XmlRootAttribute(settings.AddressManager.Settings.LocalName));
                AddressManagerSettings addressManagerSettings = (AddressManagerSettings)serializer.Deserialize(reader);
                Assert.NotNull(addressManagerSettings);
                Assert.True(addressManagerSettings.EnableDomainSearch);
            }
        }

        [Fact]
        public void TestPolicyResolver_Settings()
        {
            SmtpAgentSettings settings = null;
            Assert.Null(Record.Exception(() => settings = SmtpAgentSettings.LoadSettings(Fullpath("TestSmtpAgentConfigService.xml"))));

            Verify(settings.CertPolicies);
        }

        //
        // Use reflection to uninitialize the factory.
        // Do not publicly expose this reset feature as SmtpAgentFactory is correct as designed.
        // But our tests need to reload the singleton in process.
        //
        private void ResetSmtpAgentFactory()
        {
            var initialized = typeof(SmtpAgentFactory).GetField("m_initialized", BindingFlags.NonPublic | BindingFlags.Static);
            initialized.SetValue(null, false);
        }


        [Theory]
        [MemberData("ConfigFiles")]
        public void TestLoadConfig(string fileName)
        {
            SmtpAgentSettings settings = null;

            Assert.Null(Record.Exception(() => settings = SmtpAgentSettings.LoadSettings(Fullpath(fileName))));
            Assert.NotNull(settings);

            Assert.NotNull(settings.PublicCerts);
            this.Verify(settings.PublicCerts);

            Assert.NotNull(settings.PrivateCerts);
            this.Verify(settings.PrivateCerts);

            Assert.NotNull(settings.Anchors);
            this.Verify(settings.Anchors);
        }

        public static IEnumerable<object[]> ConfigFiles
        {
            get
            {
                yield return new[] { "TestSmtpAgentConfig.xml" };
                yield return new[] { "TestSmtpAgentConfigService.xml" };
                yield return new[] { "TestSmtpAgentConfigServiceProd.xml" };
            }
        }


        [Theory]
        [MemberData("DirectTenancConfigFiles")]
        public void TestDirectTenantLoadConfig(string fileName)
        {
            SmtpAgentSettings settings = null;

            Assert.Null(Record.Exception(() => settings = SmtpAgentSettings.LoadSettings(Fullpath(fileName))));
            Assert.NotNull(settings);

            this.Verify(settings.DomainTenants);
        }

        public static IEnumerable<object[]> DirectTenancConfigFiles
        {
            get
            {
                yield return new[] { "TestDomainTenancyConfig.xml" };
            }
        }

        string Fullpath(string fileName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "SmtpAgentTestFiles");
            return Path.Combine(folderPath, fileName);
        }

        void Verify(CertificateSettings settings)
        {
            Assert.NotNull(settings.Resolvers);
            foreach (CertResolverSettings resolverSettings in settings.Resolvers)
            {
                Assert.Null(Record.Exception(() => resolverSettings.Validate()));

                CertServiceResolverSettings serviceResolverSettings = resolverSettings as CertServiceResolverSettings;
                if (serviceResolverSettings != null)
                {
                    Assert.False(serviceResolverSettings.OrgCertificatesOnly);
                }

                ICertificateResolver resolver = null;
                Assert.Null(Record.Exception(() => resolver = resolverSettings.CreateResolver()));
                Assert.NotNull(resolver);

                if (serviceResolverSettings != null)
                {
                    Assert.False(((ConfigCertificateResolver)resolver).OrgCertificatesOnly);

                    serviceResolverSettings.OrgCertificatesOnly = true;
                    resolver = serviceResolverSettings.CreateResolver();

                    Assert.True(((ConfigCertificateResolver)resolver).OrgCertificatesOnly);
                }
            }
        }

        void Verify(PolicySettings settings)
        {
            Assert.NotNull(settings.Resolvers);
            Assert.Equal(3, settings.Resolvers.Count());
            IPolicyResolver trustResolver = settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.TrustPolicyName).CreateResolver();
            IPolicyResolver privateResolver = settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.PrivatePolicyName).CreateResolver();
            IPolicyResolver publicResolver = settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.PublicPolicyName).CreateResolver();
            Assert.NotNull(trustResolver);
            Assert.NotNull(privateResolver);
            Assert.NotNull(publicResolver);

            TrustPolicyServiceResolverSettings trustSettings =
                settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.TrustPolicyName) as
                    TrustPolicyServiceResolverSettings;

            Assert.True(trustSettings.CacheSettings.Cache);
            Assert.True(trustSettings.CacheSettings.NegativeCache);
            Assert.Equal(60, trustSettings.CacheSettings.CacheTTLSeconds);

            PrivatePolicyServiceResolverSettings privateSettings =
                settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.PrivatePolicyName) as
                    PrivatePolicyServiceResolverSettings;

            Assert.True(privateSettings.CacheSettings.Cache);
            Assert.True(privateSettings.CacheSettings.NegativeCache);
            Assert.Equal(60, privateSettings.CacheSettings.CacheTTLSeconds);


            PublicPolicyServiceResolverSettings publicSettings =
                settings.Resolvers.FirstOrDefault(r => r.Name == CertPolicyResolvers.PublicPolicyName) as
                    PublicPolicyServiceResolverSettings;

            Assert.True(publicSettings.CacheSettings.Cache);
            Assert.True(publicSettings.CacheSettings.NegativeCache);
            Assert.Equal(60, publicSettings.CacheSettings.CacheTTLSeconds);
        }

        void Verify(TrustAnchorSettings settings)
        {
            Assert.NotNull(settings.Resolver);
            Assert.Null(Record.Exception(() => settings.Validate()));

            AnchorServiceResolverSettings serviceResolverSettings = settings.Resolver as AnchorServiceResolverSettings;
            if (serviceResolverSettings != null)
            {
                Assert.False(serviceResolverSettings.OrgCertificatesOnly);
            }

            ITrustAnchorResolver resolver = null;
            Assert.Null(Record.Exception(() => resolver = settings.Resolver.CreateResolver()));
            Assert.NotNull(resolver);

            if (serviceResolverSettings != null)
            {
                ConfigAnchorResolver serviceResolver = (ConfigAnchorResolver)resolver;
                Assert.False(serviceResolver.OrgCertificatesOnly);

                Assert.False(((CertificateResolver)serviceResolver.IncomingAnchors).OrgCertificatesOnly);
                Assert.False(((CertificateResolver)serviceResolver.OutgoingAnchors).OrgCertificatesOnly);

                serviceResolverSettings.OrgCertificatesOnly = true;
                serviceResolver = (ConfigAnchorResolver)serviceResolverSettings.CreateResolver();

                Assert.True(((CertificateResolver)serviceResolver.IncomingAnchors).OrgCertificatesOnly);
                Assert.True(((CertificateResolver)serviceResolver.OutgoingAnchors).OrgCertificatesOnly);
            }
        }

        private void Verify(DomainSettings settings)
        {
            Assert.NotNull(settings.Resolver);
            Assert.Null(Record.Exception(() => settings.Validate()));

            DomainServiceResolverSettings serviceResolverSettings = settings.Resolver as DomainServiceResolverSettings;
            if (serviceResolverSettings != null)
            {
                Assert.True(serviceResolverSettings.AgentName == "Agent1");
                Assert.True(serviceResolverSettings.ClientSettings.Url == "http://localhost/ConfigService/DomainManagerService.svc/Domains");
            }
        }
    }

    public class DummyAuditor : IAuditor<IBuildAuditLogMessage>
    {
        public void Log(string category)
        {
        }

        public void Log(string category, string message)
        {
        }

        public IBuildAuditLogMessage BuildAuditLogMessage { get; private set; }
    }
}