using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DeployBot.Features.Shared.Extensions
{
    public static class ProcessExtensions
    {
        public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var processTaskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void OnProcessExited(object sender, EventArgs e)
            {
                processTaskCompletionSource.TrySetResult(true);
            }

            process.EnableRaisingEvents = true;
            process.Exited += OnProcessExited;

            try
            {
                if (process.HasExited)
                {
                    return;
                }

                using (cancellationToken.Register(() => processTaskCompletionSource.TrySetCanceled()))
                {
                    await processTaskCompletionSource.Task.ConfigureAwait(false);
                }
            }
            finally
            {
                process.Exited -= OnProcessExited;
            }
        }
    }
}
