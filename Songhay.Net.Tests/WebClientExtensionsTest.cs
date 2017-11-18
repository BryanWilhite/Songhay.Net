using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Net;
using Songhay.Net.Extensions;

namespace Songhay.Net.Tests
{
    [TestClass]
    public class WebClientExtensionsTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestProperty("resourceIndicator", "http://scripting.com/rss.xml")]
        public async Task ShouldGetStreamAsync()
        {
            var resourceIndicator = new Uri(this.TestContext.Properties["resourceIndicator"].ToString(), UriKind.Absolute);

            using (var stream = await new WebClient().WithUtf8Encoding().GetStreamAsync(resourceIndicator))
            {
                Assert.IsNotNull(stream, "The expected stream is not here.");
            }
        }
    }
}
