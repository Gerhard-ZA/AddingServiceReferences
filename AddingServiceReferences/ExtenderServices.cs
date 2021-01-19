using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Api;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.CA;
using PX.SM;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Web.Configuration;
using System.ServiceModel.Configuration;
using System.ServiceModel;

namespace AddingServiceReferences
{
    class ExtenderServices : PXGraph<ExtenderServices>
    {
        public ExtenderServices()
        {

        }

        public void updateWebConfig()
        {
            var configuration = WebConfigurationManager.OpenWebConfiguration("~");
            ServiceModelSectionGroup secgroup = (ServiceModelSectionGroup)configuration.GetSectionGroup("system.serviceModel");

            bool hasValidation = false;
            bool hasNIF = false;
            bool hasPayNow = false;
            bool hasPrtner = false;

            bool hasUpdate = false;

            foreach (BasicHttpBindingElement binding in secgroup.Bindings.BasicHttpBinding.Bindings)
            {
                if (binding.Name == "BasicHttpBinding_INIWS_Validation")
                    hasValidation = true;

                if (binding.Name == "BasicHttpBinding_INIWS_NIF")
                    hasNIF = true;

                if (binding.Name == "BasicHttpsBinding_IPayNow")
                    hasPayNow = true;
            }

            foreach (WSHttpBindingElement binding in secgroup.Bindings.WSHttpBinding.Bindings)
            {
                if (binding.Name == "WSHttpBinding_INIWS_Partner")
                    hasPrtner = true;
            }


            //INIWS Validation
            if (hasValidation == false)
            {
                secgroup.Bindings.BasicHttpBinding.Bindings.Add(CreateBasicHttpBinding("BasicHttpBinding_INIWS_Validation",
                                                                                        BasicHttpSecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("BasicHttpBinding_INIWS_Validation", "https://ws.netcash.co.za/NIWS/NIWS_Validation.svc",
                                                                "basicHttpBinding", "BasicHttpBinding_INIWS_Validation", "NIWS_Validation.INIWS_Validation"));

                hasUpdate = true;
            }

            //INIWS NIF
            if (hasNIF == false)
            {
                secgroup.Bindings.BasicHttpBinding.Bindings.Add(CreateBasicHttpBinding("BasicHttpBinding_INIWS_NIF",
                                                                                        BasicHttpSecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("BasicHttpBinding_INIWS_NIF", "https://ws.netcash.co.za/NIWS/NIWS_NIF.svc",
                                                                "basicHttpBinding", "BasicHttpBinding_INIWS_NIF", "NIWS_NIF.INIWS_NIF"));

                hasUpdate = true;
            }

            //INIWS IPAYNow
            if (hasPayNow == false)
            {
                secgroup.Bindings.BasicHttpBinding.Bindings.Add(CreateBasicHttpBinding("BasicHttpsBinding_IPayNow",
                                                                                        BasicHttpSecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("BasicHttpsBinding_IPayNow", "https://ws.netcash.co.za/PayNow/PayNow.svc",
                                                                "basicHttpBinding", "BasicHttpsBinding_IPayNow", "NIWS_PayNow.IPayNow"));

                hasUpdate = true;
            }

            //INIWS Partner
            if (hasPrtner == false)
            {
                secgroup.Bindings.WSHttpBinding.Bindings.Add(CreateWSHttpBinding("WSHttpBinding_INIWS_Partner",
                                                                                        BasicHttpSecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("WSHttpBinding_INIWS_Partner", "https://ws.netcash.co.za/NIWS/NIWS_Validation.svc",
                                                                "wsHttpBinding", "WSHttpBinding_INIWS_Partner", "NIWS_Partner.INIWS_Partner"));

                hasUpdate = true;
            }

            if (hasUpdate == true)
            {
                configuration.Save();
            }
        }

        private BasicHttpBindingElement CreateBasicHttpBinding(string name, BasicHttpSecurityMode mode, HttpClientCredentialType credentialType)
        {
            BasicHttpBindingElement basicHttpBinding = new BasicHttpBindingElement();
            basicHttpBinding.Name = name;
            basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            return basicHttpBinding;
        }

        private WSHttpBindingElement CreateWSHttpBinding(string name, BasicHttpSecurityMode mode, HttpClientCredentialType credentialType)
        {
            WSHttpBindingElement wsHttpBinding = new WSHttpBindingElement();
            wsHttpBinding.Name = name;
            wsHttpBinding.Security.Mode = SecurityMode.Transport;
            wsHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            return wsHttpBinding;
        }

        private ChannelEndpointElement CreateEndPoint(string name, string address, string binding, string bindingConfiguration, string contract)
        {
            Uri addressUri = new Uri(address);

            ChannelEndpointElement endpoint = new ChannelEndpointElement();
            endpoint.Name = name;
            endpoint.Binding = binding;
            endpoint.BindingConfiguration = bindingConfiguration;
            endpoint.Address = addressUri;
            endpoint.Contract = contract;

            return endpoint;
        }
    }
}
