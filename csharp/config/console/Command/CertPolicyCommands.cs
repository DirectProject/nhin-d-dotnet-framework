/* 
 Copyright (c) 2014, Direct Project
 All rights reserved.

 Authors:
    Joe Shook     Joseph.Shook@Surescripts.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Health.Direct.Config.Client;
using Health.Direct.Config.Client.DomainManager;
using Health.Direct.Config.Store;
using Health.Direct.Config.Tools;
using Health.Direct.Config.Tools.Command;
using Health.Direct.Policy.Extensions;

namespace Health.Direct.Config.Console.Command
{
    /// <summary>
    /// Commands to manage Certificate Policies
    /// </summary>
    public class CertPolicyCommands : CommandsBase<CertPolicyStoreClient>
    {
        const int DefaultChunkSize = 10;
        
        //---------------------------------------
        //
        // Commands
        //  Sections: CertPolicy and CertPolicyGroup
        //
        //---------------------------------------

        internal CertPolicyCommands(ConfigConsole console, Func<CertPolicyStoreClient> client)
            : base(console, client)
        {
        }



        /*
         * 
         * 
         *  CertPolicy section
         * 
         * 
         * 
         */


        /// <summary>
        /// Import and add a certificate policy
        /// </summary>
        [Command(Name = "Policy_Add", Usage = PolicyAddUsage)]
        public void CertPolicyAdd(string[] args)
        {
            string name = args.GetRequiredValue(0);
            string policyFile = args.GetRequiredValue(1);

            if (!File.Exists(policyFile))
            {
                WriteLine("File does not exist", policyFile);
                return;
            }
            string policyText = File.ReadAllText(policyFile);
            string description = args.GetOptionalValue(3, string.Empty);
            PushPolicy(name, policyText, description, false);
        }

        private const string PolicyAddUsage
            = "Import a certificate policy from a file and push it into the config store."
              + Constants.CRLF + "Policies are associated to policy groups.  Policy groups are linked to owners(domains or emails)."
              + Constants.CRLF + "    name filePath options"
              + Constants.CRLF + " \t description: (optional) additional description";

        /// <summary>
        /// Import and add a certificate policy, if one does not already exist
        /// </summary>
        [Command(Name = "Policy_Ensure", Usage = PolicyEnsureUsage)]
        public void CertPolicyEnsure(string[] args)
        {
            string name = args.GetRequiredValue(0);
            string policyFile = args.GetRequiredValue(1);

            if (!File.Exists(policyFile))
            {
                WriteLine("File does not exist", policyFile);
                return;
            }
            string policyText = File.ReadAllText(policyFile);
            string description = args.GetOptionalValue(3, string.Empty);
            PushPolicy(name, policyText, description, true);
        }

        private const string PolicyEnsureUsage
            = "Import a certificate policy from a file and push it into the config store - if not already there."
              + Constants.CRLF + "Policies are associated to policy groups.  Policy groups are linked to owners(domains or emails)."
              + Constants.CRLF + "    name filePath options"
              + Constants.CRLF + " \t description: (optional) additional description";


        /// <summary>
        /// Retrieve a certificate policy
        /// </summary>
        [Command(Name = "Policy_Get", Usage = PolicyGetUsage)]
        public void CertPolicyGet(string[] args)
        {
            string name = args.GetRequiredValue(0);
            Print(GetCertPolicy(name));
        }

        
        private const string PolicyGetUsage
            = "Retrieve information for an existing certificate policy by name."
              + Constants.CRLF + "    name";

        /// <summary>
        /// List all certificate policies
        /// </summary>
        [Command(Name = "Policies_List", Usage = "List all Policies")]
        public void CertPoliciesList(string[] args)
        {
            int chunkSize = args.GetOptionalValue(0, DefaultChunkSize);
            Print(Client.EnumerateCertPolicies(chunkSize));
        }


        /// <summary>
        /// How many certificate policies exist? 
        /// </summary>
        [Command(Name = "Policies_Count", Usage = "Retrieve # of certificate policies.")]
        public void CertPoliciesCount(string[] args)
        {
            WriteLine("{0} certificate polices", Client.GetCertPoliciesCount());
        }


        /// <summary>
        /// Delete policy from system by policy name.
        /// </summary>
        [Command(Name = "Policy_Delete", Usage = PolicyDeleteUsage)]
        public void CertPolicyDelete(string[] args)
        {
            string name = args.GetRequiredValue(0);
            Client.RemovePolicy(name);
        }

        private const string PolicyDeleteUsage
            = "Delete policy from system by policy name."
              + Constants.CRLF + "policyName"
              + Constants.CRLF + " \t policyName: Name of the policy.  Place the policy name in quotes (\"\") if there are spaces in the name.";


        /// <summary>
        /// Delete policy from a policy group 
        /// </summary>
        [Command(Name = "Policy_DeleteFromGroup", Usage = PolicyDeleteFromGroupUsage)]
        public void CertPolicyDeleteFromGroup(string[] args)
        {
            long mapId = args.GetRequiredValue<long>(0);
            Client.RemovePolicyUseFromGroup(mapId);
        }
        
        private const string PolicyDeleteFromGroupUsage
            = "Delete policy from a policy group ."
              + Constants.CRLF + "mapId"
              + Constants.CRLF + " \t mapId: Id that associates a group to a policy usage.";



        /*
         * 
         * 
         *  CertPolicyGroup section
         * 
         * 
         * 
         */

        /// <summary>
        /// Create a certificate policy group
        /// </summary>
        [Command(Name = "PolicyGroup_Add", Usage = PolicyGroupAddUsage)]
        public void CertPolicyGroupAdd(string[] args)
        {
            string name = args.GetRequiredValue(0);
            string description = args.GetOptionalValue(1, string.Empty);
            PushPolicyGroup(name, description, false);
        }

        private const string PolicyGroupAddUsage
            = "Create a certificate policy group."
              + Constants.CRLF + "Certificate policy groups are created.  " 
              + Constants.CRLF + "Use Policy_AddToGroup to join policies to groups and assign usage." 
              + Constants.CRLF + "Use Policy_AddToOwner to join groups to Domains or emails"
              + Constants.CRLF + "    name options"
              + Constants.CRLF + " \t description: (optional) additional description";

        /// <summary>
        /// Create a certificate policy group if one does not already exist
        /// </summary>
        [Command(Name = "PolicyGroup_Ensure", Usage = PolicyGroupEnsureUsage)]
        public void CertPolicyGroupEnsure(string[] args)
        {
            string name = args.GetRequiredValue(0);
            string description = args.GetOptionalValue(1, string.Empty);
            PushPolicyGroup(name, description, true);
        }

        private const string PolicyGroupEnsureUsage
            = "Create a certificate policy group - if not already there."
              + Constants.CRLF + "Certificate policy groups are created.  "
              + Constants.CRLF + "Use Policy_AddToGroup to join policies to groups and assign usage."
              + Constants.CRLF + "Use Policy_AddToOwner to join groups to Domains or emails"
              + Constants.CRLF + "    name options"
              + Constants.CRLF + " \t description: (optional) additional description";

        /// <summary>
        /// Retrieve a certificate policy group
        /// </summary>
        [Command(Name = "PolicyGroup_Get", Usage = PolicyGroupGetUsage)]
        public void CertPolicyGroupGet(string[] args)
        {
            string name = args.GetRequiredValue(0);
            Print(GetCertPolicyGroup(name));
        }

        private const string PolicyGroupGetUsage
           = "Retrieve information for an existing certificate policy group by name."
             + Constants.CRLF + "    name";


        /// <summary>
        /// Create a certificate policy group
        /// </summary>
        [Command(Name = "Policy_AddToGroup", Usage = PolicyAddToGroupUsage)]
        public void CertPolicyAddToGroup(string[] args)
        {
            string policyName = args.GetRequiredValue(0);
            string groupName = args.GetRequiredValue(1);
            string use = args.GetRequiredValue(2);
            CertPolicyUse policyUse;
            if (!Enum.TryParse(use, true, out policyUse))
            {
                WriteLine("Invalid CertPolicyUse{0}", use);
            }
            bool incoming = args.GetOptionalValue<bool>(3, true);
            bool outgoing = args.GetOptionalValue<bool>(4, true);
            PushAddPolicyToGroup(policyName, groupName, policyUse, incoming, outgoing, false);

        }

        private const string PolicyAddToGroupUsage
            = "Adds an existing policy to a group with a provided usage."
              + Constants.CRLF + "policyName groupNames policyUse incoming outgoing"
              + Constants.CRLF + " \t policyName: Name of the policy to add to the group.  Place the policy name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t groupName: Name of the policy group to add the policy to.  Place the policy group name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t policyUse: Usage name of the policy in the group.  Must be one of the following values: TRUST, PRIVATE_RESOLVER, PUBLIC_RESOLVER."
              + Constants.CRLF + " \t forIncoming: Indicates if policy is used for incoming messages.  Defaults to true"
              + Constants.CRLF + " \t forOutgoing: Indicates if policy is used for outgoing messages.  Defaults to true";

        /// <summary>
        /// Add policy to group with usage, if one does not already exist
        /// </summary>
        [Command(Name = "Policy_EnsureToGroup", Usage = PolicyEnsureToGroupUsage)]
        public void CertPolicyEnsureToGroup(string[] args)
        {
            string policyName = args.GetRequiredValue(0);
            string groupName = args.GetRequiredValue(1);
            string use = args.GetRequiredValue(2);
            CertPolicyUse policyUse;
            if (!Enum.TryParse(use, true, out policyUse))
            {
                WriteLine("Invalid CertPolicyUse{0}", use);
            }
            bool incoming = args.GetOptionalValue<bool>(3, true);
            bool outgoing = args.GetOptionalValue<bool>(4, true);
            PushAddPolicyToGroup(policyName, groupName, policyUse, incoming, outgoing, true);

        }

        private const string PolicyEnsureToGroupUsage
             = "Adds an existing policy to a group with a provided usage - if not already there."
              + Constants.CRLF + "policyName groupNames policyUse incoming outgoing"
              + Constants.CRLF + " \t policyName: Name of the policy to add to the group.  Place the policy name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t groupName: Name of the policy group to add the policy to.  Place the policy group name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t policyUse: Usage name of the policy in the group.  Must be one of the following values: TRUST, PRIVATE_RESOLVER, PUBLIC_RESOLVER."
              + Constants.CRLF + " \t forIncoming: Indicates if policy is used for incoming messages.  Defaults to true"
              + Constants.CRLF + " \t forOutgoing: Indicates if policy is used for outgoing messages.  Defaults to true";

        /// <summary>
        /// List all polici
        /// </summary>
        [Command(Name = "PolicyGroups_List", Usage = "List all policy groups")]
        public void CertPolicyGroupList(string[] args)
        {
            int chunkSize = args.GetOptionalValue(0, DefaultChunkSize);
            Print(Client.EnumerateCertPolicyGroups(chunkSize));
        }


        /// <summary>
        /// How many certificate policies exist? 
        /// </summary>
        [Command(Name = "PolicyGroups_Count", Usage = "Retrieve # of certificate policy groups.")]
        public void CertPolicyGroupCount(string[] args)
        {
            WriteLine("{0} certificate polices", Client.GetCertPolicyGroupCount());
        }

        /// <summary>
        /// List all polici
        /// </summary>
        [Command(Name = "PolicyUsage_List", Usage = PolicyUsageListUsage)]
        public void CertPolicyUsageList(string[] args)
        {
            string groupName = args.GetRequiredValue(0);
            Print(GetCertPolicyGroupWithPolicies(groupName));
        }

        private const string PolicyUsageListUsage
            = "List policies and their usage with in a policy group."
              + Constants.CRLF + "groupName"
              + Constants.CRLF +
              " \t groupName: Name of the policy group to search on.  Place the policy group name in quotes (\") if there are spaces in the name.";



        /// <summary>
        /// Associate a certificate policy group to an owner
        /// </summary>
        [Command(Name = "PolicyGroup_AddOwner", Usage = PolicyGroupAddOwnerUsage)]
        public void CertPolicyGroupAddOwner(string[] args)
        {
            string groupName = args.GetRequiredValue(0);
            string owner = args.GetRequiredValue(1);
            AssociatePolicyGroupToDomain(groupName, owner, false);
        }

        private const string PolicyGroupAddOwnerUsage
            = "Adds an existing policy group to an existing owner."
              + Constants.CRLF + "groupName owner"
              + Constants.CRLF + " \t groupName: Name of the policy group.  Place the policy group name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t owner: Name of the owner to associate with groupName."; 
              
              
        /// <summary>
        /// Associate a certificate policy group to an owner, if one does not already exist
        /// </summary>
        [Command(Name = "PolicyGroup_EnsureOwner", Usage = PolicyGroupEnsureOwnerUsage)]
        public void CertPolicyGroupEnsureOwner(string[] args)
        {
            string groupName = args.GetRequiredValue(0);
            string owner = args.GetRequiredValue(1);
            AssociatePolicyGroupToDomain(groupName, owner, true);
        }
        
        private const string PolicyGroupEnsureOwnerUsage
             = "Adds an existing policy group to an existing owner.  - if not already there."
              + Constants.CRLF + "groupName owner"
              + Constants.CRLF + " \t groupName: Name of the policy group.  Place the policy group name in quotes (\") if there are spaces in the name."
              + Constants.CRLF + " \t owner: Name of the owner to associate with groupName.";


        /// <summary>
        /// List all owners associated to a policy group
        /// </summary>
        [Command(Name = "PolicyGroup_OwnersList", Usage = PolicyGroupOwnersListUsage)]
        public void CertPolicyGroup_OwnersList(string[] args)
        {
            string groupName = args.GetRequiredValue(0);
            Print(getCertPolicyGroupWithOwners(groupName));
        }

        private const string PolicyGroupOwnersListUsage
            = "List owners associated with in a policy group."
              + Constants.CRLF + "groupName"
              + Constants.CRLF +
              " \t groupName: Name of the policy group to search on.  Place the policy group name in quotes (\") if there are spaces in the name.";

        /// <summary>
        /// Delete policy group from system by group name.
        /// </summary>
        [Command(Name = "PolicyGroup_Delete", Usage = PolicyGroupDeleteUsage)]
        public void CertPolicyGroupDelete(string[] args)
        {
            string name = args.GetRequiredValue(0);
            Client.RemovePolicyGroup(name);
        }

        private const string PolicyGroupDeleteUsage
            = "Delete policy group from system by group name."
              + Constants.CRLF + "groupName"
              + Constants.CRLF + " \t groupName: Name of the policy group.  Place the group name in quotes (\"\") if there are spaces in the name.";


        /// <summary>
        /// Deletes an existing policy group from a owner.
        /// </summary>
        [Command(Name = "PolicyGroup_DeleteFromOwner", Usage = PolicyGroupDeleteFromOwnerUsage)]
        public void CertPolicyGroupDeleteFromOwner(string[] args)
        {
            string groupName = args.GetRequiredValue(0);
            string ownerName = args.GetRequiredValue(1);
            Client.DisassociatePolicyGroupFromDomain(groupName, ownerName);
        }

        private const string PolicyGroupDeleteFromOwnerUsage
            = "Deletes an existing policy group from a owner."
              + Constants.CRLF + "groupName, ownerName"
              + Constants.CRLF + " \t groupId: Name of the policy group to delete from the owner.  Place the policy group name in quotes (\"\") if there are spaces in the name."
              + Constants.CRLF + " \t ownerName: Name of the owner to delete the policy group from.";


        //---------------------------------------
        //
        // Implementation details
        //
        //---------------------------------------    
        
   
        internal void PushPolicy(string name, string policyText, string description, bool checkForDupes)
        {
            try
            {
                if (!checkForDupes || !Client.Contains(name))
                {
                    CertPolicy certPolicy = new CertPolicy(name, description,policyText.ToBytesUtf8());
                    Client.AddPolicy(certPolicy);
                    WriteLine("Added {0}", certPolicy.Name);
                }
                else
                {
                    WriteLine("Exists {0}", name);
                }
            }
            catch (FaultException<ConfigStoreFault> ex)
            {
                if (ex.Detail.Error == ConfigStoreError.UniqueConstraint)
                {
                    WriteLine("Exists {0}", name);
                }
                else
                {
                    throw;
                }
            }
        }
        
        internal void PushPolicyGroup(string name, string description, bool checkForDupes)
        {
            try
            {
                if (!checkForDupes || !Client.Contains(name))
                {
                    CertPolicyGroup certPolicyGroup = new CertPolicyGroup(name, description);
                    Client.AddPolicyGroup(certPolicyGroup);
                    WriteLine("Added {0}", certPolicyGroup.Name);
                }
                else
                {
                    WriteLine("Exists {0}", name);
                }
            }
            catch (FaultException<ConfigStoreFault> ex)
            {
                if (ex.Detail.Error == ConfigStoreError.UniqueConstraint)
                {
                    WriteLine("Exists {0}", name);
                }
                else
                {
                    throw;
                }
            }
        }

        internal void PushAddPolicyToGroup(string policyName, string groupName, CertPolicyUse policyUse, bool incoming, bool outgoing, bool checkForDupes)
        {
            try
            {
                if (!checkForDupes || !Client.Contains(policyName, groupName, policyUse, incoming, outgoing))
                {
                    Client.AddPolicyToGroup(policyName, groupName, policyUse, incoming, outgoing);
                    WriteLine("Added {0} to {1}", policyName, groupName);
                    WriteLine("With the following usage >");
                    WriteLine(" \t policyUse: {0}", policyUse.ToString());
                    WriteLine(" \t forIncoming: {0}", incoming);
                    WriteLine(" \t forOutgoing: {0}", outgoing);
                }
                else
                {
                    WriteLine("Policy to group association already exists");
                }
            }
            catch (FaultException<ConfigStoreFault> ex)
            {
                if (ex.Detail.Error == ConfigStoreError.UniqueConstraint)
                {
                    WriteLine("Policy to group association already exists");
                }
                else
                {
                    throw;
                }
            }
        }

        private void AssociatePolicyGroupToDomain(string groupName, string owner, bool checkForDupes)
        {
            try
            {
                if (!checkForDupes || !Client.Contains(groupName, owner))
                {
                    Client.AssociatePolicyGroupToDomain(groupName, owner);
                    WriteLine("Associated {0} to {1}", groupName, owner);
                }
                else
                {
                    WriteLine("Group to owner association already exists");
                }
            }
            catch (FaultException<ConfigStoreFault> ex)
            {
                if (ex.Detail.Error == ConfigStoreError.UniqueConstraint)
                {
                    WriteLine("Policy to group association already exists");
                }
                else
                {
                    throw;
                }
            }
        }

        private CertPolicy GetCertPolicy(string name)
        {
            CertPolicy certPolicy = Client.GetPolicyByName(name);
            if (certPolicy == null)
            {
                throw new ArgumentException(string.Format("CertPolicy {0} not found", name));
            }
            return certPolicy;
        }

        private CertPolicyGroup GetCertPolicyGroup(string name)
        {
            CertPolicyGroup certPolicy = Client.GetPolicyGroupByName(name);
            if (certPolicy == null)
            {
                throw new ArgumentException(string.Format("CertPolicy {0} not found", name));
            }
            return certPolicy;
        }

        private List<CertPolicyGroupMap> GetCertPolicyGroupWithPolicies(string name)
        {
            List<CertPolicyGroupMap> maps = Client.GetPolicyGroupByNameWithPolicies(name).ToList();
            if (maps == null)
            {
                throw new ArgumentException(string.Format("CertPolicy {0} not found", name));
            }
            return maps;
        }


        private List<CertPolicyGroupDomainMap> getCertPolicyGroupWithOwners(string name)
        {
            List<CertPolicyGroupDomainMap> maps = Client.GetPolicyGroupByNameWithOwners(name).ToList();
            if (maps == null)
            {
                throw new ArgumentException(string.Format("CertPolicy {0} not found", name));
            }
            return maps;
        }


        public void Print(IEnumerable<CertPolicy> policies)
        {
            foreach (CertPolicy policy in policies)
            {
                Print(policy);
                CommandUI.PrintSectionBreak();
            }
        }

        public void Print(CertPolicy policy)
        {
            CommandUI.Print("ID", policy.ID);
            CommandUI.Print("Name", policy.Name);
            CommandUI.Print("Description", policy.Description);
            CommandUI.Print("CreateDate", policy.CreateDate);
            CommandUI.Print("Data", policy.Data.ToUtf8String());
            CommandUI.Print("# of Groups", policy.CertPolicyGroups == null ? 0 : policy.CertPolicyGroups.Count);
        }

        public void Print(IEnumerable<CertPolicyGroup> policies)
        {
            foreach (CertPolicyGroup group in policies)
            {
                Print(group);
                CommandUI.PrintSectionBreak();
            }
        }
        
        public void Print(CertPolicyGroup group)
        {
            CommandUI.Print("ID", group.ID);
            CommandUI.Print("Name", group.Name);
            CommandUI.Print("Description", group.Description);
            CommandUI.Print("CreateDate", group.CreateDate);
        }

        public void Print(List<CertPolicyGroupMap> maps)
        {
            if (!maps.Any())
            {
                WriteLine("Group has no policies associated with it.");
                return;
            }
            CommandUI.Print("GroupName", maps.First().CertPolicyGroup.Name);
            CommandUI.Print("Description", maps.First().CertPolicyGroup.Description);
            foreach (CertPolicyGroupMap map in maps)
            {
                CommandUI.Print(" \t PolicyName \t ", map.CertPolicy.Name);
                CommandUI.Print(" \t Description \t ", map.CertPolicy.Description);
                CommandUI.Print(" \t MapId (link) \t ", map.ID);
                CommandUI.Print(" \t PolicyUse \t ", map.Use);
                CommandUI.Print(" \t ForIncoming \t ", map.ForIncoming);
                CommandUI.Print(" \t ForOutgoing \t ", map.ForOutgoing);
                CommandUI.Print(" \t CreateDate \t ", map.CreateDate);
                CommandUI.PrintDivider();
            }
        }

        public void Print(List<CertPolicyGroupDomainMap> maps)
        {
            if (!maps.Any())
            {
                WriteLine("Group has no owners associated with it.");
                return;
            }
            CommandUI.Print("GroupName", maps.First().CertPolicyGroup.Name);
            CommandUI.Print("Description", maps.First().CertPolicyGroup.Description);
            foreach (CertPolicyGroupDomainMap map in maps)
            {
                CommandUI.Print(" \t Owner   \t ", map.Owner);
                CommandUI.Print(" \t CreateDate \t ", map.CreateDate);
                CommandUI.PrintDivider();
            }
        }
    }
}