﻿namespace Caliburn.Coroutines
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using Core;
    using PresentationFramework.RoutedMessaging;

    public class LoadCatalog : IResult
    {
        static readonly Dictionary<string, DeploymentCatalog> Catalogs = new Dictionary<string, DeploymentCatalog>();
        readonly string uri;

        [Import]
        public AggregateCatalog Catalog { get; set; }

        [Import]
        public IAssemblySource AssemblySource { get; set; }

        public LoadCatalog(string relativeUri)
        {
            uri = relativeUri;
        }

        public void Execute(ResultExecutionContext context)
        {
            DeploymentCatalog catalog;

            if(Catalogs.TryGetValue(uri, out catalog))
                Completed(this, new ResultCompletionEventArgs());
            else
            {
                catalog = new DeploymentCatalog(new Uri("/ClientBin/" + uri, UriKind.RelativeOrAbsolute));
                catalog.DownloadCompleted += (s, e) => {
                    if(e.Error == null) {
                        Catalogs[uri] = catalog;
                        Catalog.Catalogs.Add(catalog);
                        catalog.Parts
                            .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                            .Where(assembly => !AssemblySource.Contains(assembly))
                            .Apply(x => AssemblySource.Add(x));
                    }
                    else Loader.Hide().Execute(context);

                    Completed(this, new ResultCompletionEventArgs {
                        Error = e.Error,
                        WasCancelled = false
                    });
                };

                catalog.DownloadAsync();
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}