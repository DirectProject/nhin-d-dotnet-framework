﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Umesh Madan     umeshma@microsoft.com
    Arien Malec     arien.malec@nhindirect.org
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using Health.Direct.Agent.Certificates;
using Health.Direct.Common.Certificates;
using Health.Direct.Common.Policies;
using Health.Direct.Policy;

namespace Health.Direct.Agent
{
    /// <summary>
    /// Status of trust checking
    /// </summary>
    public enum TrustEnforcementStatus
    {
        /// <summary>
        /// Failed trust enforcement
        /// </summary>
        Failed = -1,
        /// <summary>
        /// Trust status could not be determined
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Trust checked and confirmed.
        /// </summary>
        Success = 1,                     // Signature valid, siging cert trusted, and certs match perfectly
    }

    /// <summary>
    /// Encapsulates enforcement of a Direct trust model, including certificate and trust anchor validation.
    /// </summary>
    public class TrustModel
    {
        /// <summary>
        /// A trust model with default configurations for chain validator
        /// </summary>
        public static readonly TrustModel Default = new TrustModel();

        TrustChainValidator m_certChainValidator;
        IPolicyResolver m_trustPolicyResolver;
        
        /// <summary>
        /// Constructs an instance with a default chain validator.
        /// </summary>
        public TrustModel()
            : this(new TrustChainValidator())
        {
        }

        /// <summary>
        /// Constructs an instance specifying a certificate chain validator.
        /// </summary>
        /// <param name="validator">The <see cref="TrustChainValidator"/> to use in validating certificate chains</param>
        public TrustModel(TrustChainValidator validator):this(validator, null)
        {
        }

        /// <summary>
        /// Constructs an instance specifying a certificate chain validator.
        /// </summary>
        /// <param name="validator">The <see cref="TrustChainValidator"/> to use in validating certificate chains</param>
        /// <param name="policyResolver">The <see cref="IPolicyResolver"/> to use in resolving policies.</param>
        public TrustModel(TrustChainValidator validator, IPolicyResolver policyResolver)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }

            m_certChainValidator = validator;

            
            m_trustPolicyResolver = policyResolver;
        }



        /// <summary>
        /// Gets the <see cref="TrustChainValidator"/> instance used by this model to validate certificate chains.
        /// </summary>
        public TrustChainValidator CertChainValidator
        {
            get
            {
                return m_certChainValidator;
            }
        }

        /// <summary>
        /// Enforces the trust model on an incoming message by marking
        /// the <c>Status</c> property of <see cref="DirectAddress"/> instances for the receivers
        /// </summary>
        /// <param name="message">The <see cref="IncomingMessage"/> to validate trust for.</param>
        /// <exception cref="AgentException">If this message has no signatures</exception>
        public void Enforce(IncomingMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (!message.HasSignatures)
            {
                throw new AgentException(AgentError.UnsignedMessage);
            }
            //
            // The message could have multiple signatures, including, possibly, some not by the sender
            // 
            this.FindSenderSignatures(message);
            if (!message.HasSenderSignatures)
            {
                throw new AgentException(AgentError.MissingSenderSignature);
            }
            // 
            // For each recipient, find at least one valid sender signature that the recipient trusts
            //
            DirectAddress sender = message.Sender;
            DirectAddressCollection recipients = message.DomainRecipients;
            foreach (DirectAddress recipient in recipients)
            {
                recipient.Status = TrustEnforcementStatus.Failed;
                //
                // First, find a signature that this recipient trusts
                //
                MessageSignature trustedSignature = this.FindTrustedSignature(message, recipient, recipient.TrustAnchors);
                if (trustedSignature != null)
                {
                    recipient.Status = TrustEnforcementStatus.Success;
                    //
                    // Signature has already been verified by FindTrustedSignature!
                    //
                }
            }
        }

        /// <summary>
        /// Enforces the trust model on an outgoing message by marking
        /// the <c>Status</c> property of <see cref="DirectAddress"/> instances for the receivers
        /// </summary>
        /// <param name="message">The <see cref="OutgoingMessage"/> to validate trust for.</param>
        public void Enforce(OutgoingMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            DirectAddress sender = message.Sender;

            foreach (DirectAddress recipient in message.Recipients)
            {
                recipient.Status = TrustEnforcementStatus.Failed;

                // The recipient is trusted if we at least one certificate that the sender trusts.
                recipient.Certificates = this.FindTrustedCerts(recipient.Certificates, sender.TrustAnchors);
                if (recipient.HasCertificates)
                {
                    recipient.Status = TrustEnforcementStatus.Success;
                }
            }
        }

        X509Certificate2Collection FindTrustedCerts(X509Certificate2Collection certs, X509Certificate2Collection anchors)
        {
            if (certs == null)
            {
                return null;
            }

            return certs.Where(c => m_certChainValidator.IsTrustedCertificate(c, anchors));
        }

        void FindSenderSignatures(IncomingMessage message)
        {
            message.SenderSignatures = null;

            DirectAddress sender = message.Sender;
            SignerInfoCollection allSigners = message.Signatures.SignerInfos;
            MessageSignatureCollection senderSignatures = null;
            bool match;

            foreach (SignerInfo signer in allSigners)
            {
                bool isOrgCertificate = false;

                match = signer.Certificate.MatchEmailNameOrName(sender.Address);
                if (!match)
                {
                    match = signer.Certificate.MatchDnsOrEmailOrName(sender.Host);
                    isOrgCertificate = match;
                }

                if (match)
                {
                    senderSignatures = senderSignatures ?? new MessageSignatureCollection();
                    senderSignatures.Add(new MessageSignature(signer, isOrgCertificate));
                }
            }

            message.SenderSignatures = senderSignatures;
        }

        
        MessageSignature FindTrustedSignature(IncomingMessage message, MailAddress recipient, X509Certificate2Collection anchors)
        {
            DirectAddress sender = message.Sender;
            MessageSignatureCollection signatures = message.SenderSignatures;
            MessageSignature lastTrustedSignature = null;

            foreach (MessageSignature signature in signatures)
            {
                bool certTrustedAndInPolicy =
                    (m_certChainValidator.IsTrustedCertificate(signature.Certificate, anchors) &&
                     signature.CheckSignature());
                if (certTrustedAndInPolicy && recipient != null)
                {
                    certTrustedAndInPolicy = IsCertPolicyCompliant(recipient, signature.Certificate, PolicyFilter.Default);
                }

                if (certTrustedAndInPolicy)
                {

                    if (!sender.HasCertificates)
                    {
                        // Can't really check thumbprints etc. So, this is about as good as its going to get
                        return signature;
                    }

                    if (signature.CheckThumbprint(sender))
                    {
                        return signature;
                    }
                    //
                    // We'll save this guy, but keep looking for a signer whose thumbprint we can verify
                    // If we can't find one, we'll use the last trusted signer we found.. and just mark the recipient's trust
                    // enforcement status as Success_ThumbprintMismatch
                    //
                    lastTrustedSignature = signature;
                }
            }
            return lastTrustedSignature;
        }

        /// <summary>
        /// Resolve incoming public policies base on recipient 
        /// Any negative policy will retturn an uncompliant result.
        /// No policies will result in compliance.
        /// </summary>
        /// <param name="recipient">Incoming messages are sent to the recipent</param>
        /// <param name="cert">Signing cert</param>
        /// <param name="policyFilter">The <see cref="IPolicyFilter"/> to use in validating certificate against policies</param>
        public bool IsCertPolicyCompliant(MailAddress recipient, X509Certificate2 cert, IPolicyFilter policyFilter = null)
        {
            bool isCompliant = true;
            // apply the policy if it exists
            if (m_trustPolicyResolver != null)
            {
                IList<IPolicyExpression> expressions = m_trustPolicyResolver.GetIncomingPolicy(recipient);

                foreach (var expression in expressions)
                {
                    try
                    {
                        // check for compliance
                        if (policyFilter.IsCompliant(cert, expression)) continue;
                        isCompliant = false;
                        break;
                    }
                    catch (PolicyRequiredException)
                    {
                        isCompliant = false;
                        break;
                    }
                    catch (PolicyProcessException ppe)
                    {
                        throw new AgentException(AgentError.InvalidPolicy, ppe);
                    }
                }
            }
            return isCompliant;
        }
    }
}