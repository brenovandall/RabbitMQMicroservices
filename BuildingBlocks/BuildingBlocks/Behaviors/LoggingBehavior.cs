﻿using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse> (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull, IRequest<TResponse> where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={request} - Response={Response} - RequestData={RequestData}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();

        timer.Start();

        var response = await next();

        timer.Stop();
        var timeElapsed = timer.Elapsed;

        if (timeElapsed.Seconds > 3)
            logger.LogWarning("[PERFORMANCE] The request {Request} took {timeElapsed} seconds to be executed.", 
                typeof(TRequest).Name, timeElapsed.Seconds);

        logger.LogInformation("[END] Handled {Request} with {Response}",
            typeof(TRequest).Name, typeof(TResponse).Name);

        return response;
    }
}
