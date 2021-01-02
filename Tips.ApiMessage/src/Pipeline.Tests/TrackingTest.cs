using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;

namespace Pipeline.Tests
{
    [TestClass]
    public class TrackingTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Test-123")]
        [DataRow("Test-123,Second-abc")]
        [DataRow("Test-123,Second-abc,Third-xyz")]
        public void NoParentActivityAdapterTest(string traceStateString)
        {
            var currentActivity = new Activity("CurrentActivity").Start();

            try
            {
                SetupAndAssertTrackingFields(traceStateString, currentActivity);
            }
            finally
            {
                currentActivity.Stop();
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Test-123")]
        [DataRow("Test-123,Second-abc")]
        [DataRow("Test-123,Second-abc,Third-xyz")]
        public void HasParentActivityAdapterTest(string traceStateString)
        {
            var parentActivity = new Activity("ParentActivity").Start();
            var currentActivity = new Activity("CurrentActivity").Start();

            try
            {
                SetupAndAssertTrackingFields(traceStateString, currentActivity);
            }
            finally
            {
                currentActivity.Stop();
                parentActivity.Stop();
            }
        }

        private static void SetupAndAssertTrackingFields(string traceStateString, Activity currentActivity)
        {
            var state = traceStateString?.Split(',').ToList().FirstOrDefault();
            var traceParentStateStringFirstValue = !string.IsNullOrEmpty(state) ? $",{state}" : string.Empty;

            currentActivity.TraceStateString = traceStateString;
            var applicationName = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower();

            const string traceStateStringValue = nameof(NoParentActivityAdapterTest);
            var expectedTraceStateString = $"{applicationName}={traceStateStringValue}{traceParentStateStringFirstValue}";

            Assert.AreEqual(Activity.Current?.ParentId, Tracking.TraceParentId);
            Assert.AreEqual(Activity.Current?.Id, Tracking.TraceId);
            Assert.AreEqual(Activity.Current?.TraceStateString, Tracking.TraceParentStateString);
            Assert.AreEqual(expectedTraceStateString, Tracking.TraceStateString(traceStateStringValue));
        }
    }
}
