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
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Health.Direct.Common.DnsResolver;
using Health.Direct.SmtpAgent.Tests;
using Xunit;

namespace Health.Direct.DnsResponder.Tests
{
    public class BasicTest : Tester
    {
        public static IEnumerable<object[]> Domains
        {
            get
            {
                foreach (string domain in TestStore.Default.Domains)
                {
                    yield return new object[] { domain, UseUdp };
                    yield return new object[] { domain, UseTcp };
                }
            }
        }

        public static IEnumerable<object[]> UnknownDomains
        {
            get
            {
                yield return new object[] { "adljflakd", UseUdp };
                yield return new object[] { "lasdkjfal", UseUdp };
                yield return new object[] { "adljflakd", UseTcp };
                yield return new object[] { "lasdkjfal", UseTcp };
            }
        }

        public static IEnumerable<object[]> NotSupported
        {
            get
            {
                foreach (string domain in TestStore.Default.Domains)
                {
                    foreach (DnsStandard.RecordType type in TestServer.NotSupported)
                    {
                        yield return new object[] { domain, type, UseUdp };
                        yield return new object[] { domain, type, UseTcp };
                    }
                }
            }
        }

        [Theory]
        [MemberData("Domains")]
        public void TestLookupA(string domain, bool useUDP)
        {
            IEnumerable<AddressRecord> matches = ResolveA(TestServer.Default, domain, useUDP);
            IEnumerable<DnsResourceRecord> expectedMatches = TestStore.Default.Store.Records[domain, DnsStandard.RecordType.ANAME];
            Assert.True(Equals(matches, expectedMatches));
        }

        [Theory]
        [MemberData("UnknownDomains")]
        public void TestFailure(string domain, bool useUDP)
        {
            IEnumerable<AddressRecord> matches = ResolveA(TestServer.Default, domain, useUDP);
            Assert.True(matches == null || matches.Count() == 0);
        }

        const int MultithreadThreadCount = 16;
        const int MultithreadRepeat = 500;

        [Theory]
        [MemberData("NotSupported")]
        public void TestNotSupported(string domain, DnsStandard.RecordType type, bool useUDP)
        {
            DnsClient client = TestServer.Default.CreateClient();
            client.UseUDPFirst = !useUDP;
            DnsResponse response = client.Resolve(new DnsRequest(type, domain));
            Assert.True(!response.IsSuccess);
        }

        [Fact]
        public void TestTypeA()
        {
            this.TestType<AddressRecord>(DnsStandard.RecordType.ANAME, "localhost", r => r.IPAddress.ToString() == "127.0.0.1");
        }

        [Fact]
        public void TestTypeSRV()
        {
            this.TestType<SRVRecord>(DnsStandard.RecordType.SRV, "foo.com", r => (r.Port == 353 && r.Weight == 20 && r.Target == "x.y.z.com"));
        }

        /// <summary>
        /// https://code.google.com/p/nhin-d/issues/detail?id=240
        /// Demonstration of the correct LINQ query for sorting SRV records 
        /// DCDT tests exercise actual LDAP queries here <see cref="DCDTTests"/>
        /// </summary>
        [Fact]
        public void TestTypeSRVSort()
        {
            DnsResponse response = new DnsResponse();
            response.AnswerRecords.Add(new SRVRecord("foo.order.com", 100, 389, "x.y.z.com", 50));
            response.AnswerRecords.Add(new SRVRecord("foo.order.com", 100, 389, "x.y.z.com", 100));
            response.AnswerRecords.Add(new SRVRecord("foo.order.com", 0, 389, "x.y.z.com", 0));
            response.AnswerRecords.Add(new SRVRecord("foo.order.com", 0, 389, "x.y.z.com", 50));
            response.AnswerRecords.Add(new SRVRecord("foo.order.com", 0, 389, "x.y.z.com", 100));


            List<SRVRecord> responseList = response.AnswerRecords.SRV
                .OrderBy(r => r.Priority)
                .OrderByDescending(r => r.Weight)
                .ToList();

            Assert.NotEqual(responseList[0].ToString(), "foo.order.com:389 Priority:0 Weight:0");

            responseList = response.AnswerRecords.SRV
                .OrderBy(r => r.Priority)
                .ThenByDescending(r => r.Weight)
                .ToList();


            Assert.Equal(responseList[0].ToString(), "foo.order.com:389 Priority:0 Weight:0");
            Assert.Equal(responseList[1].ToString(), "foo.order.com:389 Priority:50 Weight:100");
            Assert.Equal(responseList[2].ToString(), "foo.order.com:389 Priority:50 Weight:0");
            Assert.Equal(responseList[3].ToString(), "foo.order.com:389 Priority:100 Weight:100");
            Assert.Equal(responseList[4].ToString(), "foo.order.com:389 Priority:100 Weight:0");
        }

        [Fact]
        public void TestTypeTXT()
        {
            this.TestType<TextRecord>(DnsStandard.RecordType.TXT, "goo.com", r => r.HasStrings && r.Strings.Count == 3);
        }

        [Fact]
        public void TestBigRecord()
        {
            // Test with a fake client
            this.TestType<TextRecord>(DnsStandard.RecordType.TXT, "bigrecord.com", r => r.HasStrings && r.Strings.Count == 128);
            // Test with the real client
            using (DnsClient client = TestServer.Default.CreateClient())
            {
                TextRecord[] records = null;
                Assert.Null(Record.Exception(() => records = client.ResolveTXT("bigrecord.com").ToArray()));
                Assert.True(records != null && records.Length == 1 && records[0].Strings.Count == 128);
            }
        }

        [Fact]
        public void TestTypeMX()
        {
            this.TestType<MXRecord>(DnsStandard.RecordType.MX, "goo.com", r => r.Exchange == "foo.bar.xyz");
        }

        void TestType<T>(DnsStandard.RecordType recordType, string domain, Func<T, bool> verify)
            where T : DnsResourceRecord
        {
            bool threw = false;
            try
            {
                using (Socket socket = TestServer.Default.CreateTCPSocket())
                {
                    TestTCPClient client = new TestTCPClient(socket);
                    DnsRequest request = new DnsRequest(recordType, domain);
                    client.Send(request);

                    DnsResourceRecord record = client.Receive();
                    Assert.True(record.Type == recordType);
                    Assert.True(record.Name.Equals(request.Question.Domain, StringComparison.OrdinalIgnoreCase));

                    T rr = null;

                    Assert.Null(Record.Exception(() => rr = (T)record));
                    Assert.True(verify(rr));
                }
            }
            catch
            {
                threw = true;
            }

            Assert.False(threw);
        }

        [Theory]
        [InlineData(UseUdp)]
        [InlineData(UseTcp)]
        public void TestMultithread(bool useUDP)
        {
            TestThreads(MultithreadThreadCount, useUDP, TimeSpan.FromMilliseconds(5000));
        }

        static void TestThreads(int threadCount, bool udp, TimeSpan timeout)
        {
            TestThreads(threadCount, udp, Repeater(TestStore.Default.Domains, MultithreadRepeat), TestStore.Default.Count * MultithreadRepeat, timeout);
        }

        static void TestThreads(int threadCount, bool udp, IEnumerable<string> domains, int expectedSuccess, TimeSpan timeout)
        {
            TestThread[] threads = new TestThread[threadCount];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new TestThread(DnsStandard.RecordType.ANAME, udp, TestServer.Default, timeout);
            }
            for (int i = 0; i < threads.Length; ++i)
            {
                if (!udp && i == threads.Length - 1)
                {
                    threads[i].Start(domains, threads[i].TcpSocketDropper);
                }
                else
                {
                    threads[i].Start(domains);
                }
            }
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Stop();
            }
            for (int i = 0; i < threads.Length; ++i)
            {
                Assert.True(threads[i].Failure == 0);
                Assert.True(threads[i].Success == expectedSuccess);
            }

            if (udp)
            {
                TestServer.Default.AreMaxUdpAcceptsOutstanding();
            }
            else
            {
                TestServer.Default.AreMaxTcpAcceptsOutstanding();
            }
        }

        [Fact]
        public void TestNoNagle()
        {
            // test using a server with Nagle turned off
            using (FakeServer server = new FakeServer(TestStore.Default.Store, 53535))
            {
                server.Start();
                using (DnsClient client = new DnsClient("127.0.0.1", 53535))
                {
                    client.UseUDPFirst = false;
                    client.Timeout = TimeSpan.FromSeconds(10);

                    server.BeginAccept();
                    Assert.True(client.ResolveA("foo.com").Count() == 3);

                    TextRecord[] txtRecords = null;

                    server.BeginAccept();

                    Assert.Null(Record.Exception(() => txtRecords = client.ResolveTXT("bigrecord.com").ToArray()));
                    Assert.True(txtRecords != null && txtRecords.Length == 1);
                    Assert.True(txtRecords[0].Strings.Count == 128);
                }
            }
        }
    }
}