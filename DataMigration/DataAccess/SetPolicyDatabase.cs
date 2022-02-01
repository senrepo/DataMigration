using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DataMigration
{
    public class SetPolicyDatabase : ISetPolicyDatabase
    {

        public SetPolicyDatabase()
        {
        }

        public bool SetPolicy(IPolicyQuoteTransaction transaction)
        {
            return true;
        }

        protected virtual int GetPolicyTerm(string policy)
        {
            var termNumber = default(int);
            if (Information.IsNumeric(policy) == true || string.IsNullOrWhiteSpace(policy))
            {
                termNumber = 0;
            }
            else
            {
                int parsedTerm;
                if (int.TryParse(policy?.Substring(policy.Length - 2), out parsedTerm))
                {
                    termNumber = parsedTerm;
                }
            }
            return termNumber;
        }


    }
}
