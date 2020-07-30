using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DeployBot.Runner
{
    public class ScriptingException : Exception
    {
        public List<string> ErrorMessages { get; set; }

        public ScriptingException(Exception inner, ICollection<ErrorRecord> errorRecords) 
            : base("An unexpected error occurred while executing script.", inner)
        {
            ErrorMessages = errorRecords.Select(er => er.ToString()).ToList();
        }
    }
}
