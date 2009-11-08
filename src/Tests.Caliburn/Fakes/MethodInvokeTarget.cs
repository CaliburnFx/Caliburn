using System;
using System.Windows;

namespace Tests.Caliburn.Fakes
{
    public class MethodInvokeTarget
    {
        public static int ReturnValue = 7;

        public bool InstanceProcedureWasCalled;
        public bool InstanceFunctionWasCalled;
        public static bool StaticFunctionWasCalled;
        public static bool StaticProcedureWasCalled;

        public event EventHandler<RoutedEventArgs> AnEvent;

        public void FireAnEvent()
        {
            if (AnEvent != null)
                AnEvent(this, new RoutedEventArgs());
        }

        [FakeMetadata]
        public void AnInstanceProcedure()
        {
            InstanceProcedureWasCalled = true;
        }

        [FakeMetadata]
        public int AnInstanceFunction()
        {
            InstanceFunctionWasCalled = true;
            return ReturnValue;
        }

        [FakeMetadata]
        public static void AStaticProcedure()
        {
            StaticProcedureWasCalled = true;
        }

        [FakeMetadata]
        public static int AStaticFunction()
        {
            StaticFunctionWasCalled = true;
            return ReturnValue;
        }

        public static void Reset()
        {
            StaticProcedureWasCalled = false;
            StaticFunctionWasCalled = false;
        }
    }
}