using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Medaitor;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class MedaitorExtensions {
    public static async Task ExecuteAsync(
        this IMedaitorClient medaitorClient,
        IActivityContext medaitorContext,
        CancellationToken cancellationToken) {
        await medaitorClient.SendAsync(medaitorContext, cancellationToken);
        if (cancellationToken.IsCancellationRequested) {
            return;
        } else { 
            await medaitorClient.WaitForAsync(medaitorContext, cancellationToken);
        }
        throw new NotImplementedException();
    }
}
