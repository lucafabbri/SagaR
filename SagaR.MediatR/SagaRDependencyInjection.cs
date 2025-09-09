using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SagaR.MediatR;
using System;
using System.Collections.Generic;

namespace SagaR.Concordia;

public static class SagaRDependencyInjection
{
    /// <summary>
    /// Uses Concordia as the ISagaStepRequestSender implementation.
    /// </summary>
    /// <param name="registrar">The registrar</param>
    /// <returns>The registrar</returns>
    public static ISagasRegistrar UseMediatR(this ISagasRegistrar registrar)
    {
        registrar.UseStepRequestSender<MediatRSagaStepRequestSender> ();
        return registrar;
    }
}

