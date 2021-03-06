﻿using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Tips.Pipeline;

namespace Tips.Middleware.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogRequest(this ILogger logger, HttpContext context, string apiKeyOwner) =>
            logger.LogInformation(
                "{RequestMethod} {Url} | ApiKey.Owner: {ApiKeyOwner} | Remote IP Address: {RemoteIpAddress} | TraceParentId: {TraceParentId} | TraceParentStateString: {TraceParentStateString}",
                context.Request?.Method, context.Request?.GetDisplayUrl(),
                apiKeyOwner, context.Connection.RemoteIpAddress,
                Tracking.TraceParentId, Tracking.TraceParentStateString);

        public static void LogResponse(this ILogger logger, HttpContext context) =>
            logger.LogInformation(
                "{RequestProtocol} {StatusCode} {StatusCodeName}",
                context.Request?.Protocol,
                context.Response?.StatusCode,
                GetHttpStatusCodeName(context.Response?.StatusCode));

        private static string GetHttpStatusCodeName(int? statusCode) =>
            statusCode != null ? Enum.GetName(typeof(HttpStatusCode), statusCode) : string.Empty;
    }
}
