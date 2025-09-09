using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public static ISagasRegistrar UseConcordia(this ISagasRegistrar registrar)
    {
        registrar.UseStepRequestSender<ConcordiaSagaStepRequestSender>();
        return registrar;
    }
}

