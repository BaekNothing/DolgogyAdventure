using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class EditorUtility : Editor
{
    [MenuItem("Tools/Run All Tests In Edit Mode")]
    public static void RunAllTests()
    {
        var testRunnerApi = CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode
        };
        testRunnerApi.RegisterCallbacks(new TestCallbacks());
        testRunnerApi.Execute(new ExecutionSettings(filter));
    }


    private class TestCallbacks : ICallbacks
    {
        public void RunStarted(ITestAdaptor testsToRun)
        {

        }

        public void RunFinished(ITestResultAdaptor result)
        {

        }

        public void TestStarted(ITestAdaptor test)
        {

        }

        public void TestFinished(ITestResultAdaptor result)
        {
            //if (!result.HasChildren && result.ResultState != "Passed")
            {
                Debug.Log(string.Format("Test {0} {1}", result.Test.Name, result.ResultState));
            }
        }
    }
}
