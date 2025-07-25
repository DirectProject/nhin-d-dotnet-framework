﻿/* 
 Copyright (c) 2010, Direct Project
 All rights reserved.

 Authors:
    Joe Shook     jshook@kryptiq.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using Health.Direct.Agent;
using Health.Direct.Common.Mail.Notifications;
using Health.Direct.Common.Mime;
using Health.Direct.Config.Client.MonitorService;
using Health.Direct.Config.Store;
using Health.Direct.SmtpAgent.Config;

namespace Health.Direct.SmtpAgent
{
    internal class MonitorService
    {
        SmtpAgentSettings m_settings;

        internal MonitorService(SmtpAgentSettings settings)
        {
            m_settings = settings;
        }
        
        internal void StartMdn(OutgoingMessage message)
        {
            Debug.Assert(m_settings.HasMdnManager);

            IMdnMonitor client = m_settings.MdnMonitor.CreateMdnMonitorClient();
            using (client as IDisposable)
            {
                List<Mdn> mdns = CreateMdnStarts(message);
                client.Start(mdns.ToArray());
            }
        }

        // extract into MdnMonitorParser
        private static List<Mdn> CreateMdnStarts(OutgoingMessage message)
        {
            return message.Recipients.Select(
                recipient => new Mdn(message.Message.IDValue
                    , new MailAddress(recipient.Address).Address
                    , new MailAddress(message.NotifyTo.ToString()).Address
                    , message.IsTimelyAndReliable.GetValueOrDefault(false))
                ).ToList();
        }


        internal void UpdateMdn(IncomingMessage message)
        {
            Debug.Assert(m_settings.HasMdnManager);

            IMdnMonitor client = m_settings.MdnMonitor.CreateMdnMonitorClient();
            using (client as IDisposable)
            {
                var notification = MDNParser.Parse(message.Message);
                var disposition = notification.Disposition;
                var originalMessageId = notification.OriginalMessageID;
                var originalSender = message.Recipients.First().Address;
                var originalRecipient = message.Sender.Address;

                client.Update(
                    new Mdn // extract into MdnMonitorParser
                        {
                            MessageId = originalMessageId,
                            Recipient = originalRecipient,
                            Sender = originalSender,
                            Status = disposition.Notification.ToString()
                        });
            }
        }

        
    }
}
