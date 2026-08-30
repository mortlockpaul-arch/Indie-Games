using System.Collections.Generic;

namespace Maximinus.DebugTools;

public delegate void DebugCommandExecute(IDebugCommandHost host, string command, IList<string> arguments);
