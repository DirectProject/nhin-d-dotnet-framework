﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="urn:ihe:iti:xds-b:2007", ConfigurationName="XDSRepository")]
public interface XDSRepository
{
    
    [System.ServiceModel.OperationContractAttribute(Action="urn:ihe:iti:2007:ProvideAndRegisterDocumentSet-b", ReplyAction="urn:ihe:iti:2007:ProvideAndRegisterDocumentSet-bResponse")]
    System.ServiceModel.Channels.Message ProvideAndRegisterDocumentSet(System.ServiceModel.Channels.Message request);
    
    [System.ServiceModel.OperationContractAttribute(Action="urn:ihe:iti:2007:RetrieveDocumentSet", ReplyAction="urn:ihe:iti:2007:RetrieveDocumentSetResponse")]
    System.ServiceModel.Channels.Message RetrieveDocumentSet(System.ServiceModel.Channels.Message request);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public interface XDSRepositoryChannel : XDSRepository, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class XDSRepositoryClient : System.ServiceModel.ClientBase<XDSRepository>, XDSRepository
{
    
    public XDSRepositoryClient()
    {
    }
    
    public XDSRepositoryClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public XDSRepositoryClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public XDSRepositoryClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public XDSRepositoryClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public System.ServiceModel.Channels.Message ProvideAndRegisterDocumentSet(System.ServiceModel.Channels.Message request)
    {
        return base.Channel.ProvideAndRegisterDocumentSet(request);
    }
    
    public System.ServiceModel.Channels.Message RetrieveDocumentSet(System.ServiceModel.Channels.Message request)
    {
        return base.Channel.RetrieveDocumentSet(request);
    }
}
