using System.Collections.Generic;

namespace DebugSample;

public delegate void DebugCommandExecute(IDebugCommandHost host, string command, IList<string> arguments);
