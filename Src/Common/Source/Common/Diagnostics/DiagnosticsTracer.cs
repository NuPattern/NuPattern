using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuPattern.Diagnostics
{
    internal class DiagnosticsTracer
    {
        Dictionary<Type, CompositeTraceSource> cachedCompositeSources = new Dictionary<Type, CompositeTraceSource>();
        Dictionary<string, TraceSource> cachedBaseSources = new Dictionary<string, TraceSource>();

        public IEnumerable<TraceSource> UnderlyingSources
        {
            get { return cachedBaseSources.Values; }
        }

        public TraceSource DefaultUnderlyingSource
        {
            get { return GetOrCreateUnderlyingSource(@"*"); }
        }

        public TraceSource GetOrCreateUnderlyingSource(string sourceName)
        {
            TraceSource source;
            if (!cachedBaseSources.TryGetValue(sourceName, out source))
            {
                lock (cachedBaseSources)
                {
                    if (!cachedBaseSources.TryGetValue(sourceName, out source))
                    {
                        source = new TraceSource(sourceName, SourceLevels.All);
                        // Skip the default listener which generates noise.
                        source.Listeners.OfType<DefaultTraceListener>().ToList().ForEach(defaultListener => source.Listeners.Remove(defaultListener));
                        cachedBaseSources.Add(sourceName, source);
                    }
                }
            }

            return source;
        }

        public string GetSourceNameFor(Type type)
        {
            return type.ToSimpleName().Replace(@"+", @".");
        }

        public CompositeTraceSource GetSourceFor(Type type)
        {
            CompositeTraceSource source;

            if (!cachedCompositeSources.TryGetValue(type, out source))
            {
                lock (cachedCompositeSources)
                {
                    if (!cachedCompositeSources.TryGetValue(type, out source))
                    {
                        var parts = type.ToSimpleName().Split('.', '+');
                        // Always add the default source first.
                        var innerSources = new List<DiagnosticsTraceSource> { new DiagnosticsTraceSource(this.DefaultUnderlyingSource) };

                        for (int i = 1; i <= parts.Length; i++)
                        {
                            innerSources.Add(new DiagnosticsTraceSource(GetOrCreateUnderlyingSource(
                                String.Join(@".", parts, 0, i))));
                        }

                        // Add the default tracesource to the concrete source for the type, which 
                        // is the last one we created.
                        innerSources.Last().InnerSource.Listeners.Add(new TraceRecordDefaultTraceListener());

                        source = new CompositeTraceSource(innerSources.ToArray());
                        cachedCompositeSources.Add(type, source);
                    }
                }
            }

            return source;
        }

        public void AddListener(string sourceName, TraceListener listener)
        {
            GetOrCreateUnderlyingSource(sourceName).Listeners.Add(listener);
        }

        public void RemoveListener(string sourceName, TraceListener listener)
        {
            GetOrCreateUnderlyingSource(sourceName).Listeners.Remove(listener);
        }

        public void RemoveListener(string sourceName, string listenerName)
        {
            GetOrCreateUnderlyingSource(sourceName).Listeners.Remove(listenerName);
        }
    }
}
