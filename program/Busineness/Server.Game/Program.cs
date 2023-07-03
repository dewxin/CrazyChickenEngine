global using Engine.Common.Unit;
global using Engine.IService;
using CommandLine;
using Share.Common.Unit;
using System;
using System.Collections.Generic;
using Engine.ServerEnd;
using Server.Game._ServerGetter;
using Block.Assorted.Logging;

namespace Server.Game
{
    class Options
    {
		[Value(0)]
		public IEnumerable<string> StartFiles { get; set; }
	}

    static class Program
    {
        static void Main(string[] args)
        {
			// (1) default options
			var result = Parser.Default.ParseArguments<Options>(args);
			// or (2) build and configure instance
			//var parser = new Parser(with => with.HelpWriter=Console.Out);	

			result.WithParsed(options =>
			{
				Console.WriteLine("Parser Success- Creating Options with values:");
				Console.WriteLine("options.StringValues= '{0}'", String.Join(",", options.StartFiles));
			}

			).WithNotParsed(errs => Console.WriteLine("Failed with errors:\n{0}",
				String.Join("\n", errs)));


			//LogExtension.Instance.SetLevel(LogLevel.Info);
			Share.Serializer.SerializerCenter.Init();
			HostBoostrap.Start(typeof(GlobalInfoGetter),result.Value.StartFiles);
		}
    }
}

