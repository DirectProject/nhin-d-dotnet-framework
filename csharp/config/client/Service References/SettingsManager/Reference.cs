﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Health.Direct.Config.Client.SettingsManager {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:directproject:config/store/082010", ConfigurationName="SettingsManager.IPropertyManager")]
    public interface IPropertyManager {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/AddProperty", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/AddPropertyResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/AddPropertyConfigStoreFaul" +
            "tFault", Name="ConfigStoreFault")]
        void AddProperty(Health.Direct.Config.Store.Property property);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/AddProperties", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/AddPropertiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/AddPropertiesConfigStoreFa" +
            "ultFault", Name="ConfigStoreFault")]
        void AddProperties(Health.Direct.Config.Store.Property[] properties);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/SetProperty", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/SetPropertyResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/SetPropertyConfigStoreFaul" +
            "tFault", Name="ConfigStoreFault")]
        void SetProperty(Health.Direct.Config.Store.Property properties);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/SetProperties", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/SetPropertiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/SetPropertiesConfigStoreFa" +
            "ultFault", Name="ConfigStoreFault")]
        void SetProperties(Health.Direct.Config.Store.Property[] properties);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/GetProperties", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/GetPropertiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/GetPropertiesConfigStoreFa" +
            "ultFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.Property[] GetProperties(string[] propertyNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/GetPropertiesByPrefix", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/GetPropertiesByPrefixRespo" +
            "nse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/GetPropertiesByPrefixConfi" +
            "gStoreFaultFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.Property[] GetPropertiesByPrefix(string propertyNamePrefix);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/RemoveProperty", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/RemovePropertyResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/RemovePropertyConfigStoreF" +
            "aultFault", Name="ConfigStoreFault")]
        void RemoveProperty(string propertyNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IPropertyManager/RemoveProperties", ReplyAction="urn:directproject:config/store/082010/IPropertyManager/RemovePropertiesResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IPropertyManager/RemovePropertiesConfigStor" +
            "eFaultFault", Name="ConfigStoreFault")]
        void RemoveProperties(string[] propertyNames);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPropertyManagerChannel : Health.Direct.Config.Client.SettingsManager.IPropertyManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PropertyManagerClient : System.ServiceModel.ClientBase<Health.Direct.Config.Client.SettingsManager.IPropertyManager>, Health.Direct.Config.Client.SettingsManager.IPropertyManager {
        
        public PropertyManagerClient() {
        }
        
        public PropertyManagerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PropertyManagerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PropertyManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PropertyManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void AddProperty(Health.Direct.Config.Store.Property property) {
            base.Channel.AddProperty(property);
        }
        
        public void AddProperties(Health.Direct.Config.Store.Property[] properties) {
            base.Channel.AddProperties(properties);
        }
        
        public void SetProperty(Health.Direct.Config.Store.Property properties) {
            base.Channel.SetProperty(properties);
        }
        
        public void SetProperties(Health.Direct.Config.Store.Property[] properties) {
            base.Channel.SetProperties(properties);
        }
        
        public Health.Direct.Config.Store.Property[] GetProperties(string[] propertyNames) {
            return base.Channel.GetProperties(propertyNames);
        }
        
        public Health.Direct.Config.Store.Property[] GetPropertiesByPrefix(string propertyNamePrefix) {
            return base.Channel.GetPropertiesByPrefix(propertyNamePrefix);
        }
        
        public void RemoveProperty(string propertyNames) {
            base.Channel.RemoveProperty(propertyNames);
        }
        
        public void RemoveProperties(string[] propertyNames) {
            base.Channel.RemoveProperties(propertyNames);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:directproject:config/store/082010", ConfigurationName="SettingsManager.IBlobManager")]
    public interface IBlobManager {
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IBlobManager/AddBlob", ReplyAction="urn:directproject:config/store/082010/IBlobManager/AddBlobResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IBlobManager/AddBlobConfigStoreFaultFault", Name="ConfigStoreFault")]
        void AddBlob(Health.Direct.Config.Store.NamedBlob blob);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IBlobManager/UpdateBlob", ReplyAction="urn:directproject:config/store/082010/IBlobManager/UpdateBlobResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IBlobManager/UpdateBlobConfigStoreFaultFaul" +
            "t", Name="ConfigStoreFault")]
        void UpdateBlob(Health.Direct.Config.Store.NamedBlob blob);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IBlobManager/GetBlob", ReplyAction="urn:directproject:config/store/082010/IBlobManager/GetBlobResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IBlobManager/GetBlobConfigStoreFaultFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.NamedBlob GetBlob(string blobName);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IBlobManager/GetBlobsByPrefix", ReplyAction="urn:directproject:config/store/082010/IBlobManager/GetBlobsByPrefixResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IBlobManager/GetBlobsByPrefixConfigStoreFau" +
            "ltFault", Name="ConfigStoreFault")]
        Health.Direct.Config.Store.NamedBlob[] GetBlobsByPrefix(string blobNamePrefix);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:directproject:config/store/082010/IBlobManager/RemoveBlob", ReplyAction="urn:directproject:config/store/082010/IBlobManager/RemoveBlobResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Health.Direct.Config.Store.ConfigStoreFault), Action="urn:directproject:config/store/082010/IBlobManager/RemoveBlobConfigStoreFaultFaul" +
            "t", Name="ConfigStoreFault")]
        void RemoveBlob(string blobName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBlobManagerChannel : Health.Direct.Config.Client.SettingsManager.IBlobManager, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BlobManagerClient : System.ServiceModel.ClientBase<Health.Direct.Config.Client.SettingsManager.IBlobManager>, Health.Direct.Config.Client.SettingsManager.IBlobManager {
        
        public BlobManagerClient() {
        }
        
        public BlobManagerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BlobManagerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BlobManagerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BlobManagerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void AddBlob(Health.Direct.Config.Store.NamedBlob blob) {
            base.Channel.AddBlob(blob);
        }
        
        public void UpdateBlob(Health.Direct.Config.Store.NamedBlob blob) {
            base.Channel.UpdateBlob(blob);
        }
        
        public Health.Direct.Config.Store.NamedBlob GetBlob(string blobName) {
            return base.Channel.GetBlob(blobName);
        }
        
        public Health.Direct.Config.Store.NamedBlob[] GetBlobsByPrefix(string blobNamePrefix) {
            return base.Channel.GetBlobsByPrefix(blobNamePrefix);
        }
        
        public void RemoveBlob(string blobName) {
            base.Channel.RemoveBlob(blobName);
        }
    }
}
