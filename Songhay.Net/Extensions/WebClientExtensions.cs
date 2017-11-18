using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Songhay.Net.Extensions
{
    /// <summary>
    /// Extensions of <see cref="WebClient"/>
    /// </summary>
    public static partial class WebClientExtensions
    {
        /// <summary>
        /// Executes <see cref="System.Net.WebClient"/> GET operation.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        /// <param name="completedEventAction">The completed event action.</param>
        /// <remarks>
        /// For details, see the “Converting an Event-Based Pattern” section in
        /// “Simplify Asynchronous Programming with Tasks”
        /// by Igor Ostrovsky
        /// [http://msdn.microsoft.com/en-us/magazine/ff959203.aspx]
        /// </remarks>
        public static Task<string> GetAsync(this WebClient client, Uri resourceIndicator,
            Action<object, DownloadStringCompletedEventArgs> completedEventAction)
        {
            if (client == null) throw new ArgumentNullException("The expected Web Client is not here.");
            if (resourceIndicator == null) throw new ArgumentNullException("The expected URI is not here.");

            var completionSource = new TaskCompletionSource<string>();

            client.DownloadStringCompleted += (o, args) =>
            {
                try
                {
                    if (args.Error != null) completionSource.SetException(args.Error);
                    else if (args.Cancelled) completionSource.SetCanceled();
                    else
                    {
                        completionSource.SetResult(args.Result);
                        if (completedEventAction != null) completedEventAction.Invoke(o, args);
                    }
                }
                finally
                {
                    client.Dispose();
                }
            };

            client.DownloadStringAsync(resourceIndicator);
            return completionSource.Task;
        }

        /// <summary>
        /// Executes <see cref="System.Net.WebClient"/> GET operation for a file resource.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        /// <param name="targetPath">The download target path.</param>
        /// <param name="completedEventAction">The completed event action.</param>
        public static Task GetFileAsync(this WebClient client, Uri resourceIndicator, string targetPath,
            Action<object, AsyncCompletedEventArgs> completedEventAction)
        {
            return GetFileAsync(client, resourceIndicator, targetPath, completedEventAction, progressEventAction: null);
        }

        /// <summary>
        /// Executes <see cref="System.Net.WebClient" /> GET operation for a file resource.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        /// <param name="targetPath">The download target path.</param>
        /// <param name="completedEventAction">The completed event action.</param>
        /// <param name="progressEventAction">The progress event action.</param>
        public static Task GetFileAsync(this WebClient client, Uri resourceIndicator, string targetPath,
            Action<object, AsyncCompletedEventArgs> completedEventAction,
            Action<object, DownloadProgressChangedEventArgs> progressEventAction)
        {
            if (client == null) throw new ArgumentNullException("The expected Web Client is not here.");
            if (resourceIndicator == null) throw new ArgumentNullException("The expected URI is not here.");

            if (progressEventAction != null) client.DownloadProgressChanged += (o, args) => progressEventAction.Invoke(o, args);

            client.DownloadFileCompleted += (o, args) =>
            {
                try
                {
                    if (completedEventAction != null) completedEventAction.Invoke(o, args);
                }
                finally
                {
                    client.Dispose();
                }
            };

            return client.DownloadFileTaskAsync(resourceIndicator, targetPath);
        }

        /// <summary>
        /// Gets the specified resource as a stream.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        public static Task<Stream> GetStreamAsync(this WebClient client, Uri resourceIndicator)
        {
            return client.GetStreamAsync(resourceIndicator, getCompleteAction: null);
        }

        /// <summary>
        /// Gets the specified resource as a stream.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        /// <param name="getCompleteAction">The get complete action.</param>
        public static Task<Stream> GetStreamAsync(this WebClient client, Uri resourceIndicator,
            Action<object, OpenReadCompletedEventArgs> getCompleteAction)
        {
            if (client == null) throw new ArgumentNullException("The expected Web Client is not here.");
            if (resourceIndicator == null) throw new ArgumentNullException("The expected URI is not here.");

            var completionSource = new TaskCompletionSource<Stream>();

            client.OpenReadCompleted += (o, args) =>
            {
                try
                {
                    if (args.Error != null) completionSource.SetException(args.Error);
                    else if (args.Cancelled) completionSource.SetCanceled();
                    else
                    {
                        completionSource.SetResult(args.Result);
                        if (getCompleteAction != null) getCompleteAction.Invoke(o, args);
                    }
                }
                finally
                {
                    client.Dispose();
                }
            };

            client.OpenReadAsync(resourceIndicator);
            return completionSource.Task;
        }

        /// <summary>
        /// Executes <see cref="System.Net.WebClient" /> POST operation.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resourceIndicator">The resource indicator.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="completedEventAction">The completed event action.</param>
        public static Task<string> PostAsync(this WebClient client, Uri resourceIndicator, string postData,
            Action<object, UploadStringCompletedEventArgs> completedEventAction)
        {
            if (client == null) throw new ArgumentNullException("The expected Web Client is not here.");
            if (resourceIndicator == null) throw new ArgumentNullException("The expected URI is not here.");
            if (postData == null) throw new ArgumentNullException("The expected post data is not here.");

            var completionSource = new TaskCompletionSource<string>();

            client.UploadStringCompleted += (o, args) =>
            {
                try
                {
                    if (args.Error != null) completionSource.SetException(args.Error);
                    else if (args.Cancelled) completionSource.SetCanceled();
                    else
                    {
                        completionSource.SetResult(args.Result);
                        if (completedEventAction != null) completedEventAction.Invoke(o, args);
                    }
                }
                finally
                {
                    client.Dispose();
                }
            };

            client.UploadStringAsync(resourceIndicator, postData);
            return completionSource.Task;
        }

        /// <summary>
        /// Returns <see cref="WebClient"/> with conventional JSON headers.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static WebClient WithJsonHeaders(this WebClient client)
        {
            if (client == null) return null;

            client.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.Headers.Add("Accept-Encoding", "gzip, deflate");
            client.Headers.Add("Cache-Control", "no-cache");
            client.Headers.Add("Content-Type", "application/json; charset=utf-8");

            return client;
        }

        /// <summary>
        /// Returns <see cref="WebClient"/> with <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static WebClient WithUtf8Encoding(this WebClient client)
        {
            if (client == null) return null;

            client.Encoding = Encoding.UTF8;

            return client;
        }
    }
}
