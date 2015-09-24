using System;
using System.Collections.Generic;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class TestableAlertAction:IAlertAction
    {
        public static List<TestableAlertAction> Instances
        {
            get;
            set;
        }

        public static Action<TestableAlertAction, Dictionary<string, string>> SetParametersFunc
        {
            get;
            set;
        }

        public static Func<TestableAlertAction, AlertInstance, IContext, AlertActionResult> PerformActionFunc
        {
            get;
            set;

        }

        static TestableAlertAction()
        {
            TestableAlertAction.Instances = new List<TestableAlertAction>();

            PerformActionFunc = (instance, alert, context) =>
                {
                    return AlertActionResult.OkContinue;
                };

            SetParametersFunc = (instance, parameters) =>
                {

                };

        }

        public TestableAlertAction()
        {
            Instances.Add(this);
        }

        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }

        public AlertInstance HandledInstance
        {
            get;
            set;
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {
            this.Parameters = parameters;
            SetParametersFunc(this, parameters);
        }

        public AlertActionResult PerformAction(AlertInstance alert, IContext config)
        {
            return PerformActionFunc(this, alert, config);
        }
    }
}
