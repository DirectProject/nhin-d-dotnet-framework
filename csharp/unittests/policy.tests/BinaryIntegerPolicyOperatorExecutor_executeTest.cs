﻿/* 
 Copyright (c) 2013, Direct Project
 All rights reserved.

 Authors:
    Joe Shook      jshook@kryptiq.com
  
Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
Neither the name of The Direct Project (directproject.org) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/

using Xunit;

namespace Health.Direct.Policy.Tests
{
    public class BinaryIntegerPolicyOperatorExecutor_ExecuteTest
    {
       
        [Fact]
        public void testExecute_bitwiseAnd_assertResults()
        {
            Assert.Equal((2 & 4), PolicyOperator<int, int>.BITWISE_AND.Execute(2, 4));
            Assert.NotEqual((2 & 4), PolicyOperator<int, int>.BITWISE_AND.Execute(2, 2));
        }

        [Fact]
        public void testExecute_bitwiseOr_assertResults()
        {
            Assert.Equal((2 | 4), PolicyOperator<int, int>.BITWISE_OR.Execute(2, 4));
            Assert.NotEqual((2 | 5), PolicyOperator<int, int>.BITWISE_OR.Execute(2, 4));


            Assert.Equal((2 | 4), PolicyOperator.BitwiseOr(2, 4));
            Assert.Equal("|", PolicyOperator.BitwiseOr<int, int>().GetOperatorToken());

        }
    }
}
