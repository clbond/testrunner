﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Magnum.Extensions;
using NLog;

namespace ContinuousRunner.Console
{
    public class Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.FromArgs(args);

                using (var container = Container.Build(options))
                {
                    var loader = container.Resolve<IScriptLoader>();

                    var queue = container.Resolve<IRunQueue>();
                    
                    _logger.Info("Loading scripts");

                    foreach (var script in loader.GetScripts())
                    {
                        _logger.Info("Loaded: {0}", script.File.Name);

                        queue.Push(script);
                    }

                    var watcher = container.Resolve<IWatcher>();

                    var watchHandle = watcher.Watch();

                    var cancelling = false;

                    while (!cancelling)
                    {
                        _logger.Info("Running tests");

                        var queued = queue.Run().ToArray();
                        if (queued.Any())
                        {
                            var results = Task.WhenAll(queued);

                            results.Wait();

                            _logger.Info("Test run complete");
                        }
                        else
                        {
                            _logger.Info("No tests in queue");
                        }
                        var key = System.Console.ReadKey();
                        switch (key.KeyChar)
                        {
                            case 'q':
                            case 'Q':
                                cancelling = true;
                                break;
                        }

                        Thread.Sleep(50.Milliseconds());
                    }

                    watchHandle.Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Bootstrap of continuous runner failed: {ex.Message}");
            }
        }
    }
}
