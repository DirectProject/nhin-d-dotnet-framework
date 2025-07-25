﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Health.Direct.Config.Client.RecordRetrieval {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:directproject:config/store/082010", ConfigurationName="RecordRetrieval.IRecordRetrievalService")]
    public interface IRecordRetrievalService {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IRecordRetrievalService/GetMatchingDnsRecor" +
            "ds", ReplyAction="urn:directproject:config/store/082010/IRecordRetrievalService/GetMatchingDnsRecor" +
            "dsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IRecordRetrievalService/GetMatchingDnsRecor" +
            "dsConfigStoreFaultFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.DnsRecord[] GetMatchingDnsRecords(string domainName, Health.Direct.Common.DnsResolver.DnsStandard.RecordType typeID);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IRecordRetrievalService/GetCertificatesForO" +
            "wner", ReplyAction="urn:directproject:config/store/082010/IRecordRetrievalService/GetCertificatesForO" +
            "wnerResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IRecordRetrievalService/GetCertificatesForO" +
            "wnerConfigStoreFaultFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.Certificate[] GetCertificatesForOwner(string domain);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRecordRetrievalServiceChannel : Health.Direct.Config.Client.RecordRetrieval.IRecordRetrievalService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RecordRetrievalServiceClient : System.ServiceModel.ClientBase<Health.Direct.Config.Client.RecordRetrieval.IRecordRetrievalService>, Health.Direct.Config.Client.RecordRetrieval.IRecordRetrievalService {
        
        public RecordRetrievalServiceClient() {
        }
        
        public RecordRetrievalServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RecordRetrievalServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RecordRetrievalServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RecordRetrievalServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Health.Direct.Config.Store.DnsRecord[] GetMatchingDnsRecords(string domainName, Health.Direct.Common.DnsResolver.DnsStandard.RecordType typeID) {
            return base.Channel.GetMatchingDnsRecords(domainName, typeID);
        }
        
        public Health.Direct.Config.Store.Certificate[] GetCertificatesForOwner(string domain) {
            return base.Channel.GetCertificatesForOwner(domain);
        }
    }
}
