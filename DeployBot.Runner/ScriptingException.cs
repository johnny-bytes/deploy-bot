using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Scripting Exception.");
            builder.AppendFormat("Message: {0}\r\n", this.Message);

            builder.AppendLine("Scripting errors:");
            ErrorMessages.ForEach(message => builder.AppendLine(message));
            builder.AppendLine();

            return builder.ToString();
        }
    }
}
