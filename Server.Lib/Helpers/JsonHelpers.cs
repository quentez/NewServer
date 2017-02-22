﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Server.Lib.Infrastructure;
using Server.Lib.Services;

namespace Server.Lib.Helpers
{
    class JsonHelpers : IJsonHelpers, ITraceWriter
    {
        public JsonHelpers(
            ILoggingService loggingService,
            IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(loggingService, nameof(loggingService));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            this.loggingService = loggingService;
            this.configuration = configuration;

            this.defaultFormatting = Formatting.None;
            this.defaultSettings = new JsonSerializerSettings
            {
                TraceWriter = this
            };
        }

        private readonly ILoggingService loggingService;
        private readonly IConfiguration configuration;
        private readonly Formatting defaultFormatting;
        private readonly JsonSerializerSettings defaultSettings;

        public string ToJsonString(object content)
        {
            return JsonConvert.SerializeObject(content, this.defaultFormatting, this.defaultSettings);
        }

        public TObject FromJsonString<TObject>(string source)
        {
            return JsonConvert.DeserializeObject<TObject>(source, this.defaultSettings);
        }

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            // If an exception was provided, log it.
            if (ex != null)
            {
                this.loggingService.Exception(ex, message);
                return;
            }

            // Otherwise, it depends on the log level.
            switch (level)
            {
                case TraceLevel.Error:
                    this.loggingService.Error(message);
                    break;
                default:
                    this.loggingService.Info(message);
                    break;
            }
        }

        public TraceLevel LevelFilter => this.configuration.DebugJson
            ? TraceLevel.Verbose
            : TraceLevel.Off;
    }
}