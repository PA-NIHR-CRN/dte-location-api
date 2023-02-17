using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Addresses.V1.Queries;
using Dte.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LocationApi.HealthChecks
{
    public class PostcodeLookupHealthCheck : IHealthCheck
    {
        private readonly IMediator _mediator;

        public PostcodeLookupHealthCheck(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var data = new Dictionary<string, object>
            {
                { "timeout", context.Registration.Timeout },
            };

            var sw = Stopwatch.StartNew();

            HealthCheckResult healthCheckResult;
            try
            {
                var result = await _mediator.Send(new GetAddressesByPostcodeQuery("sw11ar"), cancellationToken);
                sw.Stop();

                data.Add("time", sw.Elapsed);
                data.Add("httpStatus", HttpStatusCode.OK);
                data.Add("httpStatusCode", (int)HttpStatusCode.OK);

                var isHealthy = result != null;

                healthCheckResult = isHealthy
                    ? HealthCheckResult.Healthy(data: data)
                    : new HealthCheckResult(context.Registration.FailureStatus, data: data);
            }
            catch (NotFoundException)
            {
                data.Add("time", sw.Elapsed);
                data.Add("httpStatus", HttpStatusCode.OK);
                data.Add("httpStatusCode", (int)HttpStatusCode.OK);

                return HealthCheckResult.Healthy(data: data);
            }
            catch (HttpServiceException ex)
            {
                data.Add("time", sw.Elapsed);
                data.Add("exception", ex.GetType().Name);
                data.Add("exceptionMessage", ex.Message);
                data.Add("httpStatus", ex.HttpStatusCode);
                data.Add("httpStatusCode", (int)ex.HttpStatusCode);

                healthCheckResult = new HealthCheckResult(context.Registration.FailureStatus, data: data);
            }
            catch (Exception ex)
            {
                data.Add("time", sw.Elapsed);
                data.Add("exception", ex.GetType().Name);
                data.Add("exceptionMessage", ex.Message);

                healthCheckResult = new HealthCheckResult(context.Registration.FailureStatus, data: data);
            }

            return healthCheckResult;
        }
    }
}