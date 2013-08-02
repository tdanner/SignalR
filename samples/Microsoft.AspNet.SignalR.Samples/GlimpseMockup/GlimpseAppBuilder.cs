using System;
using System.Collections.Generic;
using Owin;
using ServerApp.GlimpseMockup.Middleware;
using ServerApp.GlimpseMockup.Model;

namespace ServerApp.GlimpseMockup
{
    public class GlimpseAppBuilder : IAppBuilder
    {
        private readonly IAppBuilder _app;
        private GlimpseAppStorage _storage;
        private readonly GlimpseModelPipeline _pipeline;
        private GlimpseModelNode _node;

        public GlimpseAppBuilder(IAppBuilder app, GlimpseAppStorage storage, GlimpseModelPipeline pipeline)
        {
            _app = app;
            _pipeline = pipeline;
            _storage = storage;
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            _node = new GlimpseModelNode
            {
                Pipeline = _pipeline,
                UseMiddleware = middleware,
                UseArgs = args,
            };
            _pipeline.Nodes.Add(_node);

            _app.Use<GlimpseObservingMiddleware>(new GlimpseObservingOptions
            {
                Storage = _storage,
                Pipeline = _pipeline,
                Node = _node,
            });

            _app.Use(middleware, args);
            return this;
        }

        public object Build(Type returnType)
        {
            _app.Use<GlimpseTrailingMiddleware>(new GlimpseTrailingOptions
            {
                Pipeline = _pipeline,
            });
            return _app.Build(returnType);
        }

        public IAppBuilder New()
        {
            var newApp = _app.New();
            var newPipeline = new GlimpseModelPipeline
            {
                Parent = _pipeline
            };
            _node.NewPipelines.Add(newPipeline);
            return new GlimpseAppBuilder(newApp, _storage, newPipeline);
        }

        public IDictionary<string, object> Properties
        {
            get { return _app.Properties; }
        }
    }

    public class GlimpseTrailingOptions
    {
        public GlimpseModelPipeline Pipeline { get; set; }
    }
}