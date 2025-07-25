﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Health.Direct.Policy.Tests
{
    public class BinaryCollectionPolicyOperatorExecutor_executeTest
    {
        [Fact]
        public void TestExecute_Intersection_AssertResults()
        {
            
            IPolicyValue<IList<String>> op1 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "A", "B", "C", "D" });
            IPolicyValue<IList<String>> op2 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "A", "B", "E" });

            IEnumerable<string> resultList = 
                PolicyOperator<IList<string>, IList<string>, IEnumerable<string>>.INTERSECT.Execute(
                op1.GetPolicyValue(), op2.GetPolicyValue());

            var result = resultList.ToList();
            result.Count().Should().Be(2);
            result.Contains("A").Should().BeTrue();
            result.Contains("B").Should().BeTrue();
            
            op1 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "A", "B", "C" });
            op2 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "D", "E", "F" });

            resultList = PolicyOperator<IList<string>, IList<string>, IEnumerable<string>>.INTERSECT.Execute(
                op1.GetPolicyValue(), op2.GetPolicyValue());
            resultList.Any().Should().BeFalse();

            //
            // Change ordering
            //
            op1 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "D", "C", "B", "A" });
            op2 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "A", "E", "B" });
            resultList = PolicyOperator<IList<string>, IList<string>, IEnumerable<string>>.INTERSECT.Execute(
                op1.GetPolicyValue(), op2.GetPolicyValue());

            result = resultList.ToList();
            result.Count().Should().Be(2);
            result.Contains("A").Should().BeTrue();
            result.Contains("B").Should().BeTrue();
        }

        [Fact]
        public void TestExecute_Intersection_StringConversion_AssertResults()
        {
            
            IPolicyValue<IList<String>> op1 =
                PolicyValueFactory.GetInstance<IList<String>>(new List<string> {"A", "B", "C", "D"});
            IPolicyValue<String> op2 = new PolicyValue<string>("A,B,E");

            IEnumerable<string> resultList =
                PolicyOperator<IList<string>, string, IEnumerable<string>>.INTERSECT.Execute(
                    op1.GetPolicyValue(), op2.GetPolicyValue());

       
            var result = resultList.ToList();
            result.Count().Should().Be(2);
            result.Contains("A").Should().BeTrue();
            result.Contains("B").Should().BeTrue();

            op1 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "A", "B", "C" });
            op2 = new PolicyValue<string>("D,E,F");

            resultList =
                PolicyOperator<IList<string>, string, IEnumerable<string>>.INTERSECT.Execute(
                    op1.GetPolicyValue(), op2.GetPolicyValue());
            resultList.Any().Should().BeFalse();

            op1 = PolicyValueFactory.GetInstance<IList<String>>(new List<string> { "D", "C", "B", "A" });
            op2 = new PolicyValue<string>("A,E,B");
            resultList =
                PolicyOperator<IList<string>, string, IEnumerable<string>>.INTERSECT.Execute(
                    op1.GetPolicyValue(), op2.GetPolicyValue());

            result = resultList.ToList();
            result.Count().Should().Be(2);
            result.Contains("A").Should().BeTrue();
            result.Contains("B").Should().BeTrue();
        }
    }
}
