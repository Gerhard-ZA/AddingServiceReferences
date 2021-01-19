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

            bool hasBasicPartyService = false;
            bool hasWsPartyService = false;
            bool hasUpdate = false;

            //Verify if the BasicHttpBinding element exist
            foreach (BasicHttpBindingElement binding in secgroup.Bindings.BasicHttpBinding.Bindings)
            {
                if (binding.Name == "BasicHttpBinding_PartyServ")
                    hasBasicPartyService = true;
            }

            //Verify if the WSHttpBinding element exist
            foreach (BasicHttpBindingElement binding in secgroup.Bindings.BasicHttpBinding.Bindings)
            {
                if (binding.Name == "WSHttpBinding_PartyServ")
                    hasWsPartyService = true;
            }

            //Basic Service
            if (hasBasicPartyService == false)
            {
                secgroup.Bindings.BasicHttpBinding.Bindings.Add(CreateBasicHttpBinding("BasicHttpBinding_PartyServ",
                                                                                        BasicHttpSecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("BasicHttpBinding_PartyServ", "https://ws.domain.com/Party/Service.svc",
                                                                "basicHttpBinding", "BasicHttpBinding_INPartyServ", "NewParty.ServiceRef"));

                hasUpdate = true;
            }

            //WS Service
            if (hasWsPartyService == false)
            {
                secgroup.Bindings.WSHttpBinding.Bindings.Add(CreateWSHttpBinding("WSHttpBinding_PartyServ",
                                                                                        SecurityMode.Transport, HttpClientCredentialType.None));

                secgroup.Client.Endpoints.Add(CreateEndPoint("WSHttpBinding_PartyServ", "https://ws.domain.com/Party/Service2.svc",
                                                                "wsHttpBinding", "WSHttpBinding_PartyServ", "NewParty.ServiceRef2"));

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
            basicHttpBinding.Security.Mode = mode;
            basicHttpBinding.Security.Transport.ClientCredentialType = credentialType;
            return basicHttpBinding;
        }

        private WSHttpBindingElement CreateWSHttpBinding(string name, SecurityMode mode, HttpClientCredentialType credentialType)
        {
            WSHttpBindingElement wsHttpBinding = new WSHttpBindingElement();
            wsHttpBinding.Name = name;
            wsHttpBinding.Security.Mode = mode;
            wsHttpBinding.Security.Transport.ClientCredentialType = credentialType;
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
